using Microsoft.Extensions.Configuration;
using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Email;
using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Contracts.TokenJWT;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Common.Helpers;
using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Common.Utils.Validation;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;
    private readonly ITokenService _tokenService;
    private readonly IImagesService _imagesService;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly IAccountServiceErrorHandler _accountServiceErrorHandler;
    private readonly ILoggerHelper<AccountService> _logger;

    public AccountService(
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService,
        ITokenService tokenService,
        IImagesService imagesService,
        IEmailSender emailSender,
        IConfiguration configuration,
        IAccountServiceErrorHandler accountServiceErrorHandler,
        ILoggerHelper<AccountService> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = unitOfWork.GetRepository<IUserRepository>();
        _keycloakService = keycloakService;
        _tokenService = tokenService;
        _imagesService = imagesService;
        _emailSender = emailSender;
        _configuration = configuration;
        _accountServiceErrorHandler = accountServiceErrorHandler;
        _logger = logger;
    }

    public async Task<Result<UserResponseDTO>> GetUserAsync(
        string accessToken,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Result.Fail(
                    Error.Unauthorized("Access token is missing.", "ERR_UNAUTHORIZED_ACCESS")
                );
            }

            var userInfo = await _keycloakService.GetUserInfoAsync(accessToken, cancellationToken);

            if (userInfo.IsFailure)
            {
                return Result.Fail(userInfo.Errors);
            }

            return userInfo.Value.ToResponseDTO();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<AuthResponseDTO>> RegisterUserAsync(
        UserRegisterRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        ProfileImage profileImage = default!;
        try
        {
            var userExisting = await _userRepository.GetUserByEmailAsync(
                request.Email,
                cancellationToken
            );

            if (userExisting != null)
            {
                return Result.Fail(UserErrorFactory.EmailAlreadyExists());
            }

            var passwordValidation = PasswordValidator.ValidatePassword(request.Password);
            if (passwordValidation.Any())
            {
                return Result.Fail(passwordValidation);
            }

            profileImage = await _imagesService.GetProfileImageAsync(request.ProfileImage);

            var preliminaryUser = User.Create(
                id: Guid.NewGuid().ToString(),
                name: request.UserName,
                email: request.Email,
                profileImage: profileImage.ProfileImageBytes,
                profileImagePath: profileImage.ProfileImagePath,
                roles: new HashSet<string> { "User" }
            );

            if (preliminaryUser.IsFailure)
            {
                return Result.Fail(preliminaryUser.Errors);
            }

            var authResult = await _keycloakService.RegisterUserAync(
                request,
                profileImage.ProfileImagePath,
                cancellationToken
            );

            if (authResult.IsFailure)
            {
                return Result.Fail(authResult.Errors);
            }

            var (user, _, _, roles) = authResult.Value;

            var newUser = User.Create(
                user.Id,
                user.UserName,
                user.Email,
                profileImage.ProfileImageBytes,
                profileImage.ProfileImagePath,
                roles
            );

            if (newUser.IsFailure)
            {
                await _accountServiceErrorHandler.HandleRegistrationFailureAsync(
                    user,
                    user.ProfileImagePath
                );

                return Result.Fail(newUser.Errors);
            }

            await _userRepository.CreateApplicationUser(newUser.Value, cancellationToken);
            await _userRepository.AddRolesToUser(
                newUser.Value.Id,
                newUser.Value.Roles,
                cancellationToken
            );

            _unitOfWork.Commit();

            return authResult;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                await _accountServiceErrorHandler.HandleUnexpectedRegistrationExceptionAsync(
                    request.Email,
                    profileImage?.ProfileImagePath
                );
            }

            _unitOfWork.Rollback();

            throw;
        }
    }

    public async Task<Result<AuthResponseDTO>> LoginUserAsync(
        UserLoginRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var userExisting = await _userRepository.GetUserByEmailAsync(
                request.Email,
                cancellationToken
            );

            if (userExisting == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundByEmail(request.Email));
            }

            var user = User.From(userExisting);

            if (user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            var auth = await _keycloakService.LoginUserAync(request, cancellationToken);

            if (auth.IsFailure)
            {
                return Result.Fail(auth.Errors);
            }

            return auth.Value;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<bool>> ForgotPasswordAsync(
        ForgetPasswordRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var applicationUser = await _userRepository.GetUserByEmailAsync(
                request.Email,
                cancellationToken
            );

            if (applicationUser == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundByEmail(request.Email));
            }

            var user = User.From(applicationUser);

            if (user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            var token = _tokenService.GeneratePasswordResetToken(user.Value);

            var allowedOrigins =
                _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? throw new NullReferenceException("'AllowedOrigins' cannot be null");

            var clientUrl = allowedOrigins[0];

            if (string.IsNullOrWhiteSpace(clientUrl))
            {
                throw new ArgumentNullException(
                    nameof(clientUrl),
                    "'ApplicationUrl' cannot be null or empty."
                );
            }

            var resetLink =
                $"{clientUrl}/forgot-password?token={token}&userId={Uri.EscapeDataString(user.Value.Id)}";

            await _emailSender.SendPasswordResetEmail(
                request.Email,
                resetLink,
                TimeSpan.FromMinutes(15)
            );

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<UpdateAccessTokenResponseDTO>> UpdateAccessTokenAsync(
        UpdateAccessTokenRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result.Fail(
                    ErrorFactory.CreateValidationError(
                        "RefreshToken not provider",
                        nameof(request.RefreshToken)
                    )
                );
            }

            var result = await _keycloakService.RefreshAccessTokenAsync(
                request.RefreshToken,
                cancellationToken
            );

            return new UpdateAccessTokenResponseDTO(result.AccessToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<UserResponseDTO>> UpdateUserAsync(
        string userId,
        UpdateUserRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        ApplicationUserMapping? userExisting = default;
        bool isRollback = true;

        try
        {
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var passwordValidation = PasswordValidator.ValidatePassword(request.NewPassword);
                if (passwordValidation.Any())
                {
                    return Result.Fail(passwordValidation);
                }
            }

            var userWithEmailExists = await _userRepository.GetUserByEmailAsync(
                request.NewEmail,
                cancellationToken
            );

            if (userWithEmailExists is not null && userWithEmailExists.Id != userId)
            {
                return Result.Fail(UserErrorFactory.EmailAlreadyExists());
            }

            userExisting = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            if (userExisting == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundById(userId));
            }

            var user = User.From(userExisting);

            if (user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            var newImageBytes = Base64Helper.ConvertFromBase64String(request.NewProfileImage);
            bool imagesAreDifferent = !newImageBytes.SequenceEqual(user.Value.ProfileImage);

            var imageUpdateResult = await TryUpdateProfileImageAsync(
                user.Value,
                request.NewProfileImage,
                imagesAreDifferent
            );

            if (imageUpdateResult.IsFailure)
            {
                return Result.Fail(imageUpdateResult.Errors);
            }

            var nameUpdated = user.Value.UpdateProfile(
                newUsername: request.NewUserName,
                newEmail: request.NewEmail
            );

            if (nameUpdated.IsFailure)
            {
                return Result.Fail(nameUpdated.Errors);
            }

            await _keycloakService.UpdateUserAsync(user.Value, cancellationToken);

            await _userRepository.UpdateApplicationUser(user.Value, cancellationToken);

            await _keycloakService.UpdateUserPasswordAsync(
                user.Value.Id,
                request.NewPassword,
                cancellationToken
            );

            _unitOfWork.Commit();

            isRollback = false;

            if (imagesAreDifferent && !string.IsNullOrWhiteSpace(userExisting.ProfileImagePath))
            {
                await _imagesService.DeleteProfileImageAsync(userExisting.ProfileImagePath);
            }

            var updatedUser = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            if (updatedUser == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundById(userId));
            }

            return updatedUser.ToResponseDTO();
        }
        catch (Exception)
        {
            if (userExisting is not null && isRollback)
            {
                await _accountServiceErrorHandler.HandleUnexpectedUpdateExceptionAsync(
                    userExisting
                );
            }

            _unitOfWork.Rollback();

            throw;
        }
    }

    public async Task<Result<bool>> UpdateUserPasswordAsync(
        UpdatePasswordRequestDTO request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            bool isTokenValid = _tokenService.ValidatePasswordResetToken(request.Token);

            if (!isTokenValid)
            {
                return Result.Fail(UserErrorFactory.InvalidTokenError());
            }

            var passwordValidation = PasswordValidator.ValidatePassword(request.NewPassword);
            if (passwordValidation.Any())
            {
                return Result.Fail(passwordValidation);
            }

            var userExisting = await _userRepository.GetUserByIdAsync(
                request.UserId,
                cancellationToken
            );

            if (userExisting == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundById(request.UserId));
            }

            var user = User.From(userExisting);

            if (user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            await _keycloakService.UpdateUserPasswordAsync(
                user.Value.Id,
                request.NewPassword,
                cancellationToken
            );

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<Result<bool>> TryUpdateProfileImageAsync(
        User user,
        string profileImageBase64,
        bool imagesAreDifferent
    )
    {
        if (!imagesAreDifferent)
        {
            return Result.Ok(true);
        }

        var newProfileImage = await _imagesService.GetProfileImageAsync(profileImageBase64);
        return user.UpdateProfile(
            newProfileImage: newProfileImage.ProfileImageBytes,
            newProfileImagePath: newProfileImage.ProfileImagePath
        );
    }

    public async Task CleanupTestUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var allKeycloakUsers = await _keycloakService.GetAllUsersAsync();
            var testKeycloakUsers = allKeycloakUsers.Where(u =>
                u.Email.EndsWith("@test.com")
                || u.Email.EndsWith("@example.com")
                || u.Email.StartsWith("test")
                || u.Email.StartsWith("login")
                || u.Email.StartsWith("update")
                || u.Email.StartsWith("forgot")
                || u.Email.StartsWith("duplicate")
            );

            foreach (var user in testKeycloakUsers)
            {
                try
                {
                    await _keycloakService.DeleteUserByIdAsync(user.Id);
                    _logger.LogInformation($"Deleted Keycloak user: {user.Email} ({user.Id})");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"Failed to delete Keycloak user: {user.Email} ({user.Id})"
                    );
                }
            }

            // Then clean up database users
            var dbTestUsers = await _userRepository.GetTestUsersAsync(cancellationToken);
            foreach (var user in dbTestUsers)
            {
                try
                {
                    await _userRepository.DeleteUserAsync(user.Id, cancellationToken);
                    _logger.LogInformation($"Deleted database user: {user.Email} ({user.Id})");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"Failed to delete database user: {user.Email} ({user.Id})"
                    );
                }
            }

            _unitOfWork.Commit();
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}
