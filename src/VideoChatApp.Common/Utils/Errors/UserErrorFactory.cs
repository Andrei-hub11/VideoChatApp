using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.Errors;

public sealed class UserErrorFactory
{
    /// <summary>
    /// Creates an error when the provided password does not match the stored password.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing a password mismatch error.</returns>
    public static ValidationError InvalidPassword(string field)
    {
        return Error.Validation(
            "The provided password is incorrect.",
            "ERR_INVALID_PASSWORD", field);
    }

    /// <summary>
    /// Creates a not found error for a user by ID.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>An <see cref="Error"/> instance representing a user not found error.</returns>
    public static Error UserNotFoundById(string id)
    {
        return Error.NotFound($"User with id = '{id}' was not found.", "ERR_USER_NOT_FOUND");
    }

    /// <summary>
    /// Creates a not found error for a user by username.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <returns>An <see cref="Error"/> instance representing a user not found error.</returns>
    public static Error UserNotFoundByName(string userName)
    {
        return Error.NotFound($"User with username = '{userName}' was not found.", "ERR_USER_NOT_FOUND");
    }

    /// <summary>
    /// Creates a not found error for a user by email.
    /// </summary>
    /// <param name="email">The user email.</param>
    /// <returns>An <see cref="Error"/> instance representing a user not found error.</returns>
    public static Error UserNotFoundByEmail(string email)
    {
        return Error.NotFound($"User with email = '{email}' was not found.", "ERR_USER_NOT_FOUND");
    }

    /// <summary>
    /// Creates a conflict error for an existing email.
    /// </summary>
    /// <param name="email">The email that already exists.</param>
    /// <returns>An <see cref="Error"/> instance representing a conflict error.</returns>
    public static Error EmailAlreadyExists(string email)
    {
        return Error.Conflict($"The email '{email}' is already registered.", "ERR_EMAIL_CONFLICT");
    }
}
