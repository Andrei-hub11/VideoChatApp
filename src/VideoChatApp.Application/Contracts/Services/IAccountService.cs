using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.Services;

public interface IAccountService
{
    Task<Result<AuthResponseDTO>> RegisterUserAsync(UserRegisterRequestDTO requestDTO);
    Task<AuthResponseDTO> LoginUserAsync(UserLoginRequestDTO request);
}
