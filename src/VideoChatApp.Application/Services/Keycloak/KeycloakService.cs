using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

using VideoChatApp.Domain.Exceptions;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Utils;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Application.Extensions;
using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Domain.Entities;
using VideoChatApp.Application.Contracts.Logging;

namespace VideoChatApp.Application.Services.Keycloak;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILoggerHelper<KeycloakService> _logger;
    private readonly IErrorMapper _errorMapper;
    private readonly IKeycloakServiceErrorHandler _keycloakServiceErrorHandler;
    private readonly TimeSpan _tokenExpiryBuffer = TimeSpan.FromMinutes(1);
    private KeycloakToken _cachedToken = default!;
    private DateTimeOffset _tokenExpiration = DateTimeOffset.MinValue;

    public KeycloakService(HttpClient httpClient, IConfiguration configuration, ILoggerHelper<KeycloakService> logger,
        IErrorMapper errorMapper, IKeycloakServiceErrorHandler keycloakServiceErrorHandler)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _errorMapper = errorMapper;
        _keycloakServiceErrorHandler = keycloakServiceErrorHandler;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await GetUsersAsync();

        return users.ToReponseDTO();
    }

    public async Task<Result<AuthResponseDTO>> RegisterUserAync(UserRegisterRequestDTO request, string profileImageUrl, CancellationToken cancellationToken)
    {
        Result<UserMapping> newUser = default!;
        bool isRollback = true;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tokenResponse = await GetAdminTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var user = new
            {
                username = request.UserName,
                email = request.Email,
                enabled = true,
                credentials = new[]
                {
                new
                {
                    type = "password",
                    value = request.Password,
                    temporary = false
                }
            },
                attributes = new Dictionary<string, string>
                {
                    ["profileImagePath"] = profileImageUrl,
                    ["normalizedUserName"] = request.UserName
                }
            };

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"http://localhost:8080/admin/realms/chat-app/users", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict)
                {
                    var errorObj = JObject.Parse(errorContent);
                    var errorMessage = errorObj["errorMessage"]?.ToString();
                    var error = _errorMapper.MapHttpErrorToAppError(response.StatusCode, errorMessage ?? errorContent);

                    return Result.Fail(error);
                }

                throw new BadRequestException(errorContent);
            }

            newUser = await GetUserByNameAsync(request.UserName);

            if (newUser.IsFailure)
            {
                return Result.Fail(newUser.Errors);
            }

            var result = await AddUserToGroupByNameAsync(newUser.Value.Id, "Users");

            if (result.IsFailure)
            {
                return Result.Fail(result.Errors);
            }

            var groupId = await GetGroupIdByNameAsync("Users");

            if (groupId.IsFailure)
            {
                return Result.Fail(groupId.Errors);
            }

            await AddGroupRoleToUserAsync(newUser.Value.Id, groupId.Value, "User");

            var userToken = await GetUserTokenAsync(request.UserName, request.Password);

            var roles = await GetRolesAsync(newUser.Value.Id);

            if (roles.IsFailure)
            {
                return Result.Fail(roles.Errors);
            }

            isRollback = false;

            return new AuthResponseDTO(newUser.Value.ToResponseDTO(), userToken.AccessToken, userToken.RefreshToken,
                roles.Value.ToResponseDTO());
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            // If the user was successfully created in Keycloak, try to delete it in case of failure
            if (isRollback && newUser != null && !newUser.IsFailure && !string.IsNullOrEmpty(newUser.Value.Id))
            {
                try
                {
                    await DeleteUserByIdAsync(newUser.Value.Id);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, $"Failed to delete user with ID: {newUser.Value.Id} after a registration failure.");
                }
            }
        }
    }


    public async Task<Result<AuthResponseDTO>> LoginUserAync(UserLoginRequestDTO request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userToken = await GetUserTokenAsync(request.Email, request.Password);

            var user = await GetUserByEmailAsync(request.Email);

            if (user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            var roles = await GetRolesAsync(user.Value.Id);

            if (roles.IsFailure)
            {
                return Result.Fail(roles.Errors);
            }

            return new AuthResponseDTO(user.Value.ToResponseDTO(), AccessToken: userToken.AccessToken,
                RefreshToken: userToken.RefreshToken, Roles: roles.Value.ToResponseDTO());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<KeycloakToken> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var formData = new Dictionary<string, string>
    {
        { "client_id", _configuration.GetRequiredValue("UserKeycloakClient:client_id")},
        { "client_secret", _configuration.GetRequiredValue("UserKeycloakClient:client_secret") },
        { "grant_type", "refresh_token" },
        { "refresh_token", refreshToken}
    };

        var tokenEndpoint = _configuration.GetRequiredValue("UserKeycloakClient:TokenEndpoint");

        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Request failed: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<KeycloakToken>(jsonResponse);

        ThrowHelper.ThrowIfNull(tokenResponse);

        return tokenResponse;
    }

    private async Task<IEnumerable<UserMapping>> GetUsersAsync()
    {
        var apiUrl = "http://localhost:8080/admin/realms/chat-app/users";

        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve user details: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<UserMapping>>(jsonResponse);

        if (users == null)
        {
            throw new NotFoundException("Users not found");
        }

        return users;
    }

    private async Task<Result<UserMapping>> GetUserByNameAsync(string userName)
    {
        var apiUrl = $"http://localhost:8080/admin/realms/chat-app/users/?username={userName}";

        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve user details: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        // However, it will return a unique user.
        var users = JsonConvert.DeserializeObject<List<UserMapping>>(jsonResponse);

        if (users == null || users.Count == 0)
        {
            return Result.Fail(UserErrorFactory.UserNotFoundByName(userName));
        }

        return users.First();
    }

    public async Task<Result<UserMapping>> GetUserByEmailAsync(string email)
    {
        var apiUrl = $"http://localhost:8080/admin/realms/chat-app/users/?email={email}";

        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve user details: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        // However, it will return a unique user.
        var users = JsonConvert.DeserializeObject<List<UserMapping>>(jsonResponse);

        if (users == null || users.Count == 0)
        {
            return Result.Fail(UserErrorFactory.UserNotFoundByEmail(email));
        }

        return users.First();
    }

    public async Task<Result<UserInfoMapping>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var apiUrl = "http://localhost:8080/realms/chat-app/protocol/openid-connect/userinfo";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result.Fail(UserErrorFactory.InvalidOrExpiredToken("/userinfo"));
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve user details: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var user = JsonConvert.DeserializeObject<UserInfoMapping>(jsonResponse);

        ThrowHelper.ThrowIfNull(user);

        return user;
    }

    private async Task<KeycloakToken> GetAdminTokenAsync()
    {
        if (_cachedToken != null && DateTimeOffset.UtcNow < _tokenExpiration - _tokenExpiryBuffer)
        {
            return _cachedToken;
        }

        var formData = new Dictionary<string, string>
        {
            { "client_id", _configuration.GetRequiredValue("UserKeycloakAdmin:client_id")},
            { "client_secret", _configuration.GetRequiredValue("UserKeycloakAdmin:client_secret") },
            { "grant_type", "password" },
            { "username", _configuration.GetRequiredValue("UserKeycloakAdmin:username") },
            { "password", _configuration.GetRequiredValue("UserKeycloakAdmin:password") }
        };

        var tokenEndpoint = _configuration.GetRequiredValue("UserKeycloakAdmin:TokenEndpoint");

        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Request failed: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonConvert.DeserializeObject<KeycloakToken>(jsonResponse);

        ThrowHelper.ThrowIfNull(tokenResponse);

        _cachedToken = tokenResponse;
        _tokenExpiration = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        return tokenResponse;
    }

    private async Task<KeycloakToken> GetUserTokenAsync(string username, string password)
    {
        var formData = new Dictionary<string, string>
    {
        { "client_id", _configuration.GetRequiredValue("UserKeycloakClient:client_id")},
        { "client_secret", _configuration.GetRequiredValue("UserKeycloakClient:client_secret") },
        { "grant_type", "password" },
        { "username", username },
        { "password", password }
    };

        var tokenEndpoint = _configuration.GetRequiredValue("UserKeycloakClient:TokenEndpoint");

        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Request failed: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<KeycloakToken>(jsonResponse);

        ThrowHelper.ThrowIfNull(tokenResponse);

        return tokenResponse;
    }

    private async Task<Result<string>> GetGroupIdByNameAsync(string groupName)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var groupUrl = $"http://localhost:8080/admin/realms/chat-app/groups?search={groupName}";

        var response = await _httpClient.GetAsync(groupUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve groups: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var groups = JsonConvert.DeserializeObject<List<GroupResponseDTO>>(jsonResponse);

        var group = groups?.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));

        if (group?.Id == null)
        {
            return Result.Fail(ErrorFactory.ResourceNotFound("User group", groupName));
        }

        return group.Id;
    }

    private async Task<List<UserResponseDTO>> GetUsersInGroupAsync(string groupId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var usersUrl = $"http://localhost:8080/admin/realms/chat-app/groups/{groupId}/members";

        var response = await _httpClient.GetAsync(usersUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve users in group: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<UserResponseDTO>>(jsonResponse);

        ThrowHelper.ThrowIfNull(users);

        return users;
    }

    private async Task<ClientMapping> GetClientByClientIdAsync(string clientId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var usersUrl = $"http://localhost:8080/admin/realms/chat-app/clients/?clientId={clientId}";

        var response = await _httpClient.GetAsync(usersUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve client with clientId = '{clientId}': {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var clients = JsonConvert.DeserializeObject<List<ClientMapping>>(jsonResponse);

        ThrowHelper.ThrowIfNull(clients);

        return clients[0];
    }

    private async Task<List<RoleMappingDTO>> GetRolesByGroupIdAsync(string groupId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var roleUrl = $"http://localhost:8080/admin/realms/chat-app/groups/{groupId}/role-mappings";

        var response = await _httpClient.GetAsync(roleUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve roles for group {groupId}: {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var clientMappingsResponse = JsonConvert.DeserializeObject<ResourceMappingsResponseDTO>(jsonResponse);

        if (clientMappingsResponse?.ClientMappings == null || !clientMappingsResponse.ClientMappings.TryGetValue("chat-app-client", out ResourceMappingDTO? value))
        {
            throw new BadRequestException($"Client not found or mappings empty.");
        }

        var mappings = value.Mappings;

        return mappings;
    }

    private async Task<Result<HashSet<RoleMappingDTO>>> GetRolesAsync(string userId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var clientId = _configuration.GetRequiredValue("UserKeycloakAdmin:client_id");

        var client = await GetClientByClientIdAsync(clientId);

        var rolesUrl = $"http://localhost:8080/admin/realms/chat-app/users/{userId}/role-mappings/clients/{client.Id}";

        var response = await _httpClient.GetAsync(rolesUrl);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var result = await _keycloakServiceErrorHandler.HandleNotFoundErrorAsync(response, userId);

            return Result.Fail(result.Errors);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to retrieve roles for userId = '{userId}': {response.StatusCode}, {error}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var roles = JsonConvert.DeserializeObject<HashSet<RoleMappingDTO>>(jsonResponse);

        ThrowHelper.ThrowIfNull(roles);

        return roles;
    }

    private async Task<Result<bool>> AddUserToGroupByNameAsync(string userId, string groupName)
    {
        var groupId = await GetGroupIdByNameAsync(groupName);

        if (groupId.IsFailure)
        {
            return Result.Fail(groupId.Errors);
        }

        await AddUserToGroupAsync(userId, groupId.Value);

        return true;
    }

    private async Task AddUserToGroupAsync(string userId, string groupId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var addGroupUrl = $"http://localhost:8080/admin/realms/chat-app/users/{userId}/groups/{groupId}";

        var response = await _httpClient.PutAsync(addGroupUrl, null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to add user to group: {response.StatusCode}, {error}");
        }
    }

    public async Task AddGroupRoleToUserAsync(string userId, string groupId, string roleName)
    {
        var roles = await GetRolesByGroupIdAsync(groupId);

        var role = roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

        if (role == null)
        {
            throw new BadRequestException($"Role '{roleName}' not found in group '{groupId}'");
        }

        await AddRoleToUserAsync(userId, role);
    }


    private async Task AddRoleToUserAsync(string userId, RoleMappingDTO role)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var addRoleUrl = $"http://localhost:8080/admin/realms/chat-app/users/{userId}/role-mappings/clients/{role.ContainerId}";
        var content = new StringContent(JsonConvert.SerializeObject(new[] { new { id = role.Id, name = role.Name } }), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(addRoleUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to add role to user: {response.StatusCode}, {error}");
        }
    }

    public async Task UpdateUserAsync(User user, CancellationToken? cancellationToken = null)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var updateUserUrl = $"http://localhost:8080/admin/realms/chat-app/users/{user.Id}";

        // warning: sending email, even if it is not actually being updated, to avoid deleting this field in keycloak

        var updatedUser = new
        {
            username = user.UserName,
            email = user.Email,
            attributes = new Dictionary<string, string>
            {
                ["profileImagePath"] = user.ProfileImagePath,
                ["normalizedUserName"] = user.UserName
            }
        };

        var json = JsonConvert.SerializeObject(updatedUser);

        var content = new StringContent(json, Encoding.UTF8, "application/json");


        var response = await _httpClient.PutAsync(updateUserUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update user {user.Id}: {response.StatusCode}, {errorContent}");
        }
    }

    public async Task<bool> DeleteUserByIdAsync(string userId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var deleteUserUrl = $"http://localhost:8080/admin/realms/chat-app/users/{userId}";
        var response = await _httpClient.DeleteAsync(deleteUserUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Failed to add role to user: {response.StatusCode}, {error}");
        }

        return true;
    }
}
