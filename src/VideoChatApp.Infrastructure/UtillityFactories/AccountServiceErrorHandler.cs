using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Contracts.Models;
using VideoChatApp.Contracts.Response;

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

}


