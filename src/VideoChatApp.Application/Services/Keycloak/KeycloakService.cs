using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

using VideoChatApp.Domain.Exceptions;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Utils;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Application.Extensions;

namespace VideoChatApp.Application.Services.Keycloak;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public KeycloakService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await GetUsersAsync();

        return users.ToDTO();
    }

    public async Task<AuthResponseDTO> RegisterUserAync(UserRegisterRequestDTO request)
    {
        UserMapping newUser = default!;
        try
        {
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
                    ["profile-image-url"] = request.ProfileImageUrl
                }
            };

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"http://localhost:8080/admin/realms/chat-app/users", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new BadRequestException(error);
            }

            newUser = await GetUserByNameAsync(request.UserName);

            await AddUserToGroupByNameAsync(newUser.Id, "Users");
            var groupId = await GetGroupIdByNameAsync("Users");
            await AddGroupRoleToUserAsync(newUser.Id, groupId, "User");

            var userToken = await GetUserTokenAsync(request.UserName, request.Password);
            var roles = await GetRolesAsync(newUser.Id);

            return new AuthResponseDTO(newUser.ToDTO(), userToken.AccessToken, userToken.RefreshToken, roles.ToDTO());
        }
        catch (Exception)
        {
            // If the user was successfully created in Keycloak, try to delete it in case of failure
            if (newUser != null && !string.IsNullOrEmpty(newUser.Id))
            {
                try
                {
                    await DeleteUserByIdAsync(newUser.Id);
                }
                catch(Exception)
                {
                    throw;
                }
            }

            throw; 
        }
    }


    public async Task<AuthResponseDTO> LoginUserAync(UserLoginRequestDTO request)
    {
        var userToken = await GetUserTokenAsync(request.Email, request.Password);

        var user = await GetUserByEmailAsync(request.Email);

        var roles = await GetRolesAsync(user.Id);

        return new AuthResponseDTO(user.ToDTO(), AccessToken: userToken.AccessToken,
            RefreshToken: userToken.RefreshToken, Roles: roles.ToDTO());
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

        if (users == null || users.Count == 0)
        {
            throw new NotFoundException("User not found");
        }

        return users;
    }

    private async Task<UserMapping> GetUserByNameAsync(string username)
    {
        var apiUrl = $"http://localhost:8080/admin/realms/chat-app/users/?username={username}";

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
            throw new NotFoundException("User not found");
        }

        return users.First();
    }

    private async Task<UserMapping> GetUserByEmailAsync(string email)
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
            throw new NotFoundException("User not found");
        }

        return users.First();
    }

    private async Task<KeycloakToken> GetAdminTokenAsync()
    {
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

    private async Task<string> GetGroupIdByNameAsync(string groupName)
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

        return group?.Id ?? throw new NotFoundException("Group not found");
    }

    private async Task AddUserToGroupByNameAsync(string userId, string groupName)
    {
        var groupId = await GetGroupIdByNameAsync(groupName);
        await AddUserToGroupAsync(userId, groupId);
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

        if (clientMappingsResponse?.ClientMappings == null || !clientMappingsResponse.ClientMappings.ContainsKey("chat-app-client"))
        {
            throw new BadRequestException($"Client chat-app-client not found or mappings empty.");
        }

        var mappings = clientMappingsResponse.ClientMappings["chat-app-client"].Mappings;

        return mappings;
    }

    private async Task<HashSet<RoleMappingDTO>> GetRolesAsync(string userId)
    {
        var tokenResponse = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var clientId = _configuration.GetRequiredValue("UserKeycloakAdmin:client_id");

        var client = await GetClientByClientIdAsync(clientId);

        var rolesUrl = $"http://localhost:8080/admin/realms/chat-app/users/{userId}/role-mappings/clients/{client.Id}";

        var response = await _httpClient.GetAsync(rolesUrl);

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
