using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.Errors;

public static class RoomErrorFactory
{
    /// <summary>
    /// Creates a not found error for a room by ID.
    /// </summary>
    /// <param name="roomId">The room identifier.</param>
    /// <returns>An <see cref="Error"/> instance representing a room not found error.</returns>
    public static Error RoomNotFoundById(Guid roomId)
    {
        return Error.NotFound($"Room with id = '{roomId}' was not found.", "ERR_ROOM_NOT_FOUND");
    }

    /// <summary>
    /// Creates a not found error for a room member by user ID.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>An <see cref="Error"/> instance representing a room member not found error.</returns>
    public static Error MemberNotFoundByUserId(string userId)
    {
        return Error.NotFound(
            $"Member with id = '{userId}' was not found.",
            "ERR_MEMBER_NOT_FOUND"
        );
    }
}
