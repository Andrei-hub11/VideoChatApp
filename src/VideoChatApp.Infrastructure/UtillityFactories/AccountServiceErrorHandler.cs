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

    public AccountServiceErrorHandler(IKeycloakService keycloakService, IImagesService imagesService)
    {
        _keycloakService = keycloakService;
        _imagesService = imagesService;
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
        await _keycloakService.DeleteUserByIdAsync(user.Id);

        if (!string.IsNullOrWhiteSpace(profileImagePath))
        {
            await _imagesService.DeleteProfileImageAsync(profileImagePath);
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
        if (!string.IsNullOrWhiteSpace(profileImagePath))
        {
            await _imagesService.DeleteProfileImageAsync(profileImagePath);
        }

        var existingUserResult = await _keycloakService.GetUserByEmailAsync(userEmail);

        if (existingUserResult.IsFailure)
        {
            return;
        }

        await _keycloakService.DeleteUserByIdAsync(existingUserResult.Value.Id);
    }

    /// <summary>
    /// Handles the rollback process when an unexpected exception occurs during a user update operation.
    /// This method performs the following steps:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Restores the original username and profile image path in the domain model if they were changed during the update operation.</description>
    ///     </item>
    ///     <item>
    ///         <description>If a new profile image was uploaded, this image is deleted from the storage, and the original image path is restored in the domain model.</description>
    ///     </item>
    ///     <item>
    ///         <description>Updates the user in Keycloak with the restored details if there were any changes in the profile image path or username.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="user">An instance of <see cref="ApplicationUserMapping"/> representing the user details including email, username, and profile image path.
    /// This instance is used to restore the original profile details and handle the cleanup process.</param>
    /// <returns>A task that represents the asynchronous operation. The task does not return a value. If an error occurs during the rollback process, it is logged or handled appropriately. 
    /// This method does not propagate exceptions but ensures that cleanup actions are performed to maintain consistency.</returns>
    public async Task HandleUnexpectedUpdateExceptionAsync(ApplicationUserMapping user)
    {
        var existingUserResult = await _keycloakService.GetUserByEmailAsync(user.Email);

        if (existingUserResult.IsFailure)
        {
            return;
        }

        var existingUser = existingUserResult.Value;

        var userDomain = User.From(user);

        if (userDomain.IsFailure)
        {
            return;
        }

        // Restore the original username and profile image path in the domain model
        var updateResult = userDomain.Value.UpdateProfile(user.UserName, [1], user.ProfileImagePath);

        if (updateResult.IsFailure)
        {
            // Log or handle the failure to update the domain model if necessary
            return;
        }

        // Delete the new profile image if it exists and revert to the original path
        if (!string.IsNullOrWhiteSpace(user.ProfileImagePath) &&
            existingUser.ProfileImagePath != user.ProfileImagePath)
        {
            if (!string.IsNullOrWhiteSpace(existingUser.ProfileImagePath))
            {
                await _imagesService.DeleteProfileImageAsync(existingUser.ProfileImagePath);
            }
        }

        // Update Keycloak only if there are changes
        await _keycloakService.UpdateUserAsync(userDomain.Value);
    }
}


