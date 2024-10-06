﻿using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Infrastructure.UtillityFactories;

/// <summary>
/// Handles errors and exceptions specific to account service operations, such as user registration failures.
/// </summary>
public class AccountServiceErrorHandler : IAccountServiceErrorHandler
{
    private readonly IKeycloakService _keycloakService;
    private readonly IImagesService _imagesService;

    private readonly ILoggerHelper<AccountServiceErrorHandler> _logger;

    public AccountServiceErrorHandler(IKeycloakService keycloakService, IImagesService imagesService, 
        ILoggerHelper<AccountServiceErrorHandler> logger)
    {
        _keycloakService = keycloakService;
        _imagesService = imagesService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the cleanup process when a user registration fails. 
    /// Deletes the user from Keycloak and, if applicable, deletes the profile image.
    /// </summary>
    /// <param name="user">The user information associated with the failed registration.</param>
    /// <param name="profileImagePath">The path of the profile image to delete, if applicable.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task HandleRegistrationFailureAsync(UserResponseDTO user, string? profileImagePath)
    {
        try
        {
            await _keycloakService.DeleteUserByIdAsync(user.Id);

            if (!string.IsNullOrWhiteSpace(profileImagePath))
            {
                await _imagesService.DeleteProfileImageAsync(profileImagePath);
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the cleanup process without interrupting the original flow
            _logger.LogError(ex, $"An error occurred while cleaning up the user registration for user ID: {user.Id}");
        }
    }

    /// <summary>
    /// Handles the cleanup process when an unexpected exception occurs during user registration.
    /// This method performs the following steps:
    /// <list type="bullet">
    ///     <item>
    ///         <description>If a profile image path is provided and not null or whitespace, the profile image is deleted.</description>
    ///     </item>
    ///     <item>
    ///         <description>Checks if a user with the provided email exists in Keycloak. If the user is found, the user is deleted from Keycloak.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="userEmail">The email address of the user to check for existence and delete if found.</param>
    /// <param name="profileImagePath">The path of the user's profile image to delete. If null or whitespace, no image is deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task does not return a value.</returns>
    public async Task HandleUnexpectedRegistrationExceptionAsync(string userEmail, string? profileImagePath)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(profileImagePath))
            {
                await _imagesService.DeleteProfileImageAsync(profileImagePath);
            }

            var existingUserResult = await _keycloakService.GetUserByEmailAsync(userEmail);

            if (existingUserResult.IsFailure)
            {
                throw new InvalidOperationException($"Failed to retrieve the user by email: {userEmail}");
            }

            // Delete the user from Keycloak by their ID
            await _keycloakService.DeleteUserByIdAsync(existingUserResult.Value.Id);
        }
        catch (Exception ex)
        {
            // Log the exception without rethrowing to avoid concealing the original issue
            _logger.LogError(ex, $"An error occurred during the cleanup process for user registration with email: {userEmail}");
        }
    }

    public async Task HandleUnexpectedUpdateExceptionAsync(ApplicationUserMapping user)
    {
        try
        {
            var existingUserResult = await _keycloakService.GetUserByEmailAsync(user.Email);

            if (existingUserResult.IsFailure)
            {
                throw new InvalidOperationException($"Failed to retrieve the existing user by email: {user.Email}");
            }

            var existingUser = existingUserResult.Value;

            var userDomain = User.From(user);

            if (userDomain.IsFailure)
            {
                throw new InvalidOperationException("Failed to create the domain user from the provided user mapping.");
            }

            // Restore the original username and profile image path in the domain model
            var updateResult = userDomain.Value.UpdateProfile(user.UserName, null, user.ProfileImagePath);

            if (updateResult.IsFailure)
            {
                throw new InvalidOperationException("Failed to update the profile with the original username and profile image path.");
            }

            // Delete the new profile image if it exists
            if (!string.IsNullOrWhiteSpace(user.ProfileImagePath)
                && !string.IsNullOrWhiteSpace(existingUser.ProfileImagePath)
                && existingUser.ProfileImagePath != user.ProfileImagePath)
            {
                await _imagesService.DeleteProfileImageAsync(existingUser.ProfileImagePath);
            }

            // Update Keycloak only if there are changes
            await _keycloakService.UpdateUserAsync(userDomain.Value);
        }
        catch (Exception ex)
        {
            // Log the exception but do not rethrow to avoid hiding the original exception
            _logger.LogError(ex, $"An error occurred while handling the rollback process for user: {user.Email}");
        }
    }
}


