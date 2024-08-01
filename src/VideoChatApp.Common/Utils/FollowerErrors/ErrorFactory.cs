using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.FollowerErrors;

public sealed class ErrorFactory
{
    public static Error Failure(string description) => Error.Failure(description, "ERR_FAILURE");

    public static Error UserNotFound(string id) => Error.NotFound("ERR_USER_NOT_FOUND",
        $"User with id = '{id}' was not found.");
}
