using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Services;

public interface IKeycloakService
{
    Task<Result<UserMapping>> GetUserByEmailAsync(string email);
    Task<IReadOnlyList<UserResponseDTO>> GetAllUsersAsync();
    Task<Result<UserInfoMapping>> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO>> RegisterUserAync(UserRegisterRequestDTO requestDTO, string profileImageUrl,
        CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO>> LoginUserAync(UserLoginRequestDTO request, CancellationToken cancellationToken);
    Task<KeycloakToken> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken);
    Task<bool> DeleteUserByIdAsync(string userId);
}
