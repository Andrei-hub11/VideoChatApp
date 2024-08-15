using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.Services;

public interface IAccountService
{
    Task<Result<UserResponseDTO>> GetUserAsync(string accessToken, CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO>> RegisterUserAsync(UserRegisterRequestDTO request, CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO>> LoginUserAsync(UserLoginRequestDTO request, CancellationToken cancellationToken);
    Task<Result<UpdateAccessTokenResponseDTO>> UpdateAccessTokenAsync(UpdateAccessTokenRequestDTO request, 
        CancellationToken cancellationToken);
    Task<Result<UserResponseDTO>> UpdateUserAsync(string userId, UpdateUserRequestDTO request, 
        CancellationToken cancellationToken);
}
