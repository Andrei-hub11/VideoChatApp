using System.Collections.ObjectModel;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Common.Utils.GuardClause;
using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Domain.Entities;

public sealed class Message
{
    public Guid MessageId { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid MemberId { get; private set; }
    public string MessageContent { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }

    private Message(Guid messageId, Guid roomId, Guid memberId, string messageContent, DateTime sentAt)
    {
        MessageId = messageId;
        RoomId = roomId;
        MemberId = memberId;
        MessageContent = messageContent;
        SentAt = sentAt;
    }

    public static Result<Message> Create(Guid roomId, Guid memberId, string messageContent, DateTime sentAt)
    {
        var errors = ValidateMessage(roomId, memberId, messageContent, sentAt);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        return new Message(Guid.NewGuid(), roomId, memberId, messageContent, sentAt);
    }

    private static ReadOnlyCollection<ValidationError> ValidateMessage(Guid roomId, Guid memberId,
        string messageContent, DateTime sentAt)
    {
        var errors = new List<ValidationError>();

        var result = Guard.For().Use((guard) =>
        {
            guard.FailIf(roomId == Guid.Empty,
                "The 'RoomId' is invalid", "ERR_INVALID_FIELD", "RoomId")
            .FailIf(memberId == Guid.Empty,
                "The 'MemberId' is invalid", "ERR_INVALID_FIELD", "MemberId")
            .IsNullOrWhiteSpace(messageContent)
            .InRange(messageContent, 1, 500)
            .FailIf(sentAt > DateTime.UtcNow,
                "The 'SentAt' date cannot be in the future", "ERR_INVALID_FIELD", "SentAt")
            .DoNotThrowOnError();
        });

        errors.AddRange(result.Errors);

        return errors.AsReadOnly();
    }
}
