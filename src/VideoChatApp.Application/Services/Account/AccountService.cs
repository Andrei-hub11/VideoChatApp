﻿using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Common.Utils.Errors;
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
    private readonly IImagesService _imagesService;
    private readonly IAccountServiceErrorHandler _accountServiceErrorHandler;

    public AccountService(IUnitOfWork unitOfWork, IKeycloakService keycloakService, IImagesService imagesService,
        IAccountServiceErrorHandler accountServiceErrorHandler)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = unitOfWork.GetRepository<IUserRepository>();
        _keycloakService = keycloakService;
        _imagesService = imagesService;
        _accountServiceErrorHandler = accountServiceErrorHandler;
    }

    public async Task<Result<AuthResponseDTO>> RegisterUserAsync(UserRegisterRequestDTO request, CancellationToken cancellationToken)
    {
        ProfileImage profileImage = default!;
        try
        {
            var userExisting = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

            if (userExisting != null)
            {
                return Result.Fail(UserErrorFactory.EmailAlreadyExists(request.Email));
            }

            profileImage = await _imagesService.GetProfileImageAsync(request.ProfileImage);

            var preliminaryUser = User.Create(
                id: Guid.NewGuid().ToString(), 
                name: request.UserName,
                email: request.Email,
                password: request.Password,
                profileImage: profileImage.ProfileImageBytes,
                profileImagePath: profileImage.ProfileImagePath,
                roles: new HashSet<string> { "User"}
            );

            if (preliminaryUser.IsFailure)
            {
                return Result.Fail(preliminaryUser.Errors);
            }

            var authResult = await _keycloakService.RegisterUserAync(request, profileImage.ProfileImagePath, cancellationToken);

            if (authResult.IsFailure)
            {
                return Result.Fail(authResult.Errors);
            }

            var (user, _, _, roles) = authResult.Value;

            var newUser = User.Create(user.Id, user.UserName, user.Email, request.Password,
                profileImage.ProfileImageBytes, profileImage.ProfileImagePath, roles.ToHashSetString());

            if (newUser.IsFailure)
            {
                await _accountServiceErrorHandler.HandleRegistrationFailureAsync(user, user.ProfileImageUrl);

                return Result.Fail(newUser.Errors);
            }

            await _userRepository.CreateApplicationUser(newUser.Value, cancellationToken);
            await _userRepository.AddRolesToUser(newUser.Value.Id, newUser.Value.Roles, cancellationToken);

            _unitOfWork.Commit();

            return authResult;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                await _accountServiceErrorHandler.HandleUnexpectedRegistrationExceptionAsync(request.Email,
                    profileImage?.ProfileImagePath);
            }

            _unitOfWork.Rollback();

            throw;
        }
    }

    public async Task<Result<AuthResponseDTO>> LoginUserAsync(UserLoginRequestDTO request, CancellationToken cancellationToken)
    {
        try
        {
            var userExisting = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

            if (userExisting == null)
            {
                return Result.Fail(UserErrorFactory.UserNotFoundByEmail(request.Email));
            }

            var user = User.From(userExisting);

            if(user.IsFailure)
            {
                return Result.Fail(user.Errors);
            }

            if (!user.Value.VerifyPassword(request.Password))
            {
                return Result.Fail(UserErrorFactory.InvalidPassword(nameof(request.Password)));
            }

            var auth = await _keycloakService.LoginUserAync(request, cancellationToken);

            if(auth.IsFailure)
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
}