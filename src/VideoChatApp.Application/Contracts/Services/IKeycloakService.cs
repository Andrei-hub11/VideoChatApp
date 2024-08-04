using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.Services;

public interface IKeycloakService
{
    Task<Result<UserMapping>> GetUserByEmailAsync(string email);
    Task<IReadOnlyList<UserResponseDTO>> GetAllUsersAsync();
    Task<Result<AuthResponseDTO>> RegisterUserAync(UserRegisterRequestDTO requestDTO, string profileImageUrl,
        CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO>> LoginUserAync(UserLoginRequestDTO request, CancellationToken cancellationToken);
    Task<bool> DeleteUserByIdAsync(string userId);
}
