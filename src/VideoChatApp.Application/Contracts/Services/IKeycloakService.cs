using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.Services;

public interface IKeycloakService
{
    Task<IReadOnlyList<UserResponseDTO>> GetAllUsersAsync();
    Task<AuthResponseDTO> RegisterUserAync(UserRegisterRequestDTO requestDTO);
    Task<AuthResponseDTO> LoginUserAync(UserLoginRequestDTO request);
    Task<bool> DeleteUserByIdAsync(string userId);
}
