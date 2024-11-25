using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using IntegrationTests;
using VideoChatApp.Contracts.Response;
using VideoChatApp.IntegrationTests.Builders;
using VideoChatApp.IntegrationTests.Converters;
using Xunit.Abstractions;

namespace VideoChatApp.IntegrationTests;

[Collection("Database")]
public class VideoChatAppApiTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly AppHostFixture _fixture;
    private readonly string _baseUrl = "https://localhost:7037/api/v1";
    private readonly ITestOutputHelper _output;

    public VideoChatAppApiTests(AppHostFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithUserName("profiletest")
            .WithEmail("profile@test.com")
            .WithPassword("Profile123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/account/profile");
        var result = await response.Content.ReadFromJsonAsync<UserResponseDTO>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.UserName.Should().Be("profiletest");
        result.Email.Should().Be("profile@test.com");
    }

    [Fact]
    public async Task GetProfile_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/account/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequestBuilder()
            .WithUserName("testuser")
            .WithEmail("test@example.com")
            .WithPassword("Test123!@#")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/register", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
        result.User.Email.Should().Be(request.Email);
        result.User.UserName.Should().Be(request.UserName);
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.Roles.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestBuilder()
            .WithUserName("testuser")
            .WithEmail("invalid-email")
            .WithPassword("Test123!@#")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestBuilder()
            .WithUserName("testuser")
            .WithEmail("duplicate@test.com")
            .WithPassword("Test123!@#")
            .Build();

        // First registration
        await _client.PostAsJsonAsync($"{_baseUrl}/account/register", request);

        // Second registration with same email
        var duplicateRequest = new RegisterRequestBuilder()
            .WithUserName("testuser2")
            .WithEmail("duplicate@test.com")
            .WithPassword("Test123!@#")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            duplicateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestBuilder()
            .WithUserName("testuser")
            .WithEmail("test@example.com")
            .WithPassword("weak") // Too short and missing required characters
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithUserName("logintest")
            .WithEmail("login@test.com")
            .WithPassword("Login123!@#")
            .Build();

        await _client.PostAsJsonAsync($"{_baseUrl}/account/register", registerRequest);

        var loginRequest = new LoginRequestBuilder()
            .WithEmail("login@test.com")
            .WithPassword("Login123!@#")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
        result.User.Email.Should().Be(loginRequest.Email);
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.Roles.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnNotFound()
    {
        // Arrange
        var loginRequest = new LoginRequestBuilder()
            .WithEmail("nonexistent@test.com")
            .WithPassword("WrongPass123!@#")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithUserName("invalidlogintest")
            .WithEmail("invalidlogin@test.com")
            .WithPassword("Valid123!@#")
            .Build();

        await _client.PostAsJsonAsync($"{_baseUrl}/account/register", registerRequest);

        var loginRequest = new LoginRequestBuilder()
            .WithEmail("invalidlogin@test.com")
            .WithPassword("InvalidPassword123!@#") // Invalid password
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/account/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("forgot@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var forgotRequest = new ForgotPasswordRequestBuilder().WithEmail("forgot@test.com").Build();

        // Act
        var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/forgot-password",
            forgotRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ForgotPassword_WithInvalidEmail_ShouldReturnNotFound()
    {
        // Arrange
        var request = new ForgotPasswordRequestBuilder().WithEmail("nonexistent@test.com").Build();

        // Act
        var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/forgot-password",
            request
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("update@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("updatedName")
            .WithEmail("updated@test.com")
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/profile/{authResult.User.Id}",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        result.Should().NotBeNull();
        result!.UserName.Should().Be("updatedName");
        result.Email.Should().Be("updated@test.com");
    }

    [Fact]
    public async Task UpdateProfile_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("updatedName")
            .WithEmail("updated@test.com")
            .Build();

        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PutAsJsonAsync($"{_baseUrl}/account/profile/1", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TokenRenew_WithValidToken_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("token@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        var tokenRequest = new UpdateAccessTokenRequestBuilder()
            .WithRefreshToken(authResult!.RefreshToken)
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/token-renew",
            tokenRequest
        );
        var result = await response.Content.ReadFromJsonAsync<UpdateAccessTokenResponseDTO>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task TokenRenew_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        var tokenRequest = new UpdateAccessTokenRequestBuilder()
            .WithRefreshToken("invalid_refresh_token")
            .Build();

        // Act
        var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/token-renew",
            tokenRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /*   [Fact]
      public async Task UpdatePassword_WithValidToken_ShouldReturnSuccess()
      {
          // Arrange
          var registerRequest = new RegisterRequestBuilder()
              .WithEmail("updatepass@test.com")
              .WithPassword("Password123!@#")
              .Build();
  
          var registerResponse = await _client.PostAsJsonAsync(
              $"{_baseUrl}/account/register",
              registerRequest
          );
          var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
              new JsonSerializerOptions
              {
                  PropertyNameCaseInsensitive = true,
                  Converters = { new JsonStringSetConverter() },
              }
          );
  
          // Request password reset
          var forgotRequest = new ForgotPasswordRequestBuilder()
              .WithEmail("updatepass@test.com")
              .Build();
          await _client.PostAsJsonAsync($"{_baseUrl}/account/forgot-password", forgotRequest);
  
          var updateRequest = new UpdatePasswordRequestBuilder()
              .WithUserId(authResult!.User.Id)
              .WithNewPassword("NewPassword123!@#")
              .WithToken(authResult.AccessToken) // This might need to be obtained from email service in real scenario
              .Build();
  
          // Act
          var response = await _client.PutAsJsonAsync(
              $"{_baseUrl}/account/update-password",
              updateRequest
          );
  
          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);
          var result = await response.Content.ReadFromJsonAsync<bool>();
          result.Should().BeTrue();
      } */

    [Fact]
    public async Task UpdatePassword_WithInvalidToken_ShouldReturnInternalServerError()
    {
        // Arrange
        var updateRequest = new UpdatePasswordRequestBuilder()
            .WithUserId("some-user-id")
            .WithNewPassword("NewPassword123!@#")
            .WithToken("invalid_token")
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/update-password",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Register and get token first
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("updateinvalid@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("") // Invalid empty username
            .WithEmail("invalid-email") // Invalid email format
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/profile/{authResult.User.Id}",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProfile_WithNonExistentUser_ShouldReturnNotFound()
    {
        // Arrange - Register and get token first
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("updatenotfound@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("updatedName")
            .WithEmail("updated@test.com")
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/profile/nonexistent-user-id",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProfile_WithValidPassword_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("updatepassword@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("updatedName")
            .WithEmail("updated@test.com")
            .WithPassword("NewPassword123!@#")
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/profile/{authResult.User.Id}",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponseDTO>();
        result.Should().NotBeNull();
        result!.UserName.Should().Be("updatedName");
        result.Email.Should().Be("updated@test.com");

        // Verify can login with new password
        var loginRequest = new LoginRequestBuilder()
            .WithEmail("updated@test.com")
            .WithPassword("NewPassword123!@#")
            .Build();

        var loginResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/login",
            loginRequest
        );
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequestBuilder()
            .WithEmail("updateinvalidpass@test.com")
            .WithPassword("Password123!@#")
            .Build();

        var registerResponse = await _client.PostAsJsonAsync(
            $"{_baseUrl}/account/register",
            registerRequest
        );
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringSetConverter() },
            }
        );

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authResult!.AccessToken
            );

        var updateRequest = new UpdateUserRequestBuilder()
            .WithUserName("updatedName")
            .WithEmail("updated@test.com")
            .WithPassword("weak") // Invalid password
            .Build();

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{_baseUrl}/account/profile/{authResult.User.Id}",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public async Task InitializeAsync()
    {
        await CleanupTestUsers();
    }

    public async Task DisposeAsync()
    {
        try
        {
            await CleanupTestUsers();
        }
        catch
        {
            // Log but don't throw during cleanup
        }
        finally
        {
            await _fixture.ResetAsync();
        }
    }

    private async Task CleanupTestUsers()
    {
        try
        {
            var response = await _client.DeleteAsync($"{_baseUrl}/account/test-cleanup");
            if (!response.IsSuccessStatusCode)
            {
                _output.WriteLine($"Cleanup failed with status code: {response.StatusCode}");
                _output.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Exception during cleanup: {ex.Message}");
            _output.WriteLine(ex.StackTrace);
        }
    }
}
