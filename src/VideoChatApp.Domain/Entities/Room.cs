using System.Collections.ObjectModel;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Common;
using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Domain.GuardClause;

namespace VideoChatApp.Domain.Entities;

public sealed class Room
{
    public Guid RoomId { get; private set; }
    public string RoomName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Room(Guid roomId, string roomName, DateTime createdAt)
    {
        RoomId = roomId;
        RoomName = roomName;
        CreatedAt = createdAt;
    }

    public static Result<Room> Create(string roomName)
    {
        var errors = ValidateRoom(roomName);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        return new Room(Guid.Empty, roomName, DateTimeHelper.NowInBrasilia());
    }

    private static ReadOnlyCollection<ValidationError> ValidateRoom(string roomName)
    {
        var errors = new List<ValidationError>();

        var result = Guard.For().Use((guard) =>
        {
            guard.IsNullOrWhiteSpace(roomName)
            .InRange(roomName, 1, 100)
            .DoNotThrowOnError();
        });

        errors.AddRange(result.Errors);

        return errors.AsReadOnly();
    }
}
