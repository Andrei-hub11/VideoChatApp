using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;
using VideoChatApp.Domain.Exceptions;

namespace VideoChatApp.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;

    public AccountService(IUnitOfWork unitOfWork, IKeycloakService keycloakService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = unitOfWork.GetRepository<IUserRepository>();
        _keycloakService = keycloakService;
    }

    public async Task<Result<AuthResponseDTO>> RegisterUserAsync(UserRegisterRequestDTO requestDTO)
    {
        UserResponseDTO user = default!;
        try
        {
            var authResult = await _keycloakService.RegisterUserAync(requestDTO);

            (user, _, _, var roles) = authResult;

            var newUser = User.Create(user.Id, user.UserName, user.Email, requestDTO.Password, roles.ToHashSetString());

            if (newUser.IsFailure)
            {
                await _keycloakService.DeleteUserByIdAsync(user.Id);
                return Result.Fail(newUser.Errors);
            }

            await _userRepository.CreateApplicationUser(newUser.Value);
            _unitOfWork.Commit();

            return authResult;
        }
        catch (Exception)
        {
            if (user is not null && !string.IsNullOrWhiteSpace(user.Id))
                await _keycloakService.DeleteUserByIdAsync(user.Id);

            _unitOfWork.Rollback();

            throw;
        }
    }

    public Task<AuthResponseDTO> LoginUserAsync(UserLoginRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
