using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Tests.Domain.Entities;

public class MessageTests
{
    [Theory]
    [MemberData(nameof(GetValidMessageData))]
    public void CreateMessage_ShouldSucceed_WhenDataIsValid(
        Guid roomId,
        Guid memberId,
        string messageContent,
        DateTime sentAt
    )
    {
        // Act
        var result = Message.Create(roomId, memberId, messageContent, sentAt);

        // Assert
        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(roomId, result.Value.RoomId);
        Assert.Equal(memberId, result.Value.MemberId);
        Assert.Equal(messageContent, result.Value.Content);
        Assert.Equal(sentAt, result.Value.SentAt);
    }

    [Theory]
    [MemberData(nameof(GetInvalidMessageData))]
    public void CreateMessage_ShouldFail_WhenDataIsInvalid(
        Guid roomId,
        Guid memberId,
        string messageContent,
        DateTime sentAt,
        List<ValidationError> expectedErrors
    )
    {
        // Act
        var result = Message.Create(roomId, memberId, messageContent, sentAt);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Errors);
        Assert.Equal(expectedErrors.Count, result.Errors.Count);

        for (int i = 0; i < expectedErrors.Count; i++)
        {
            Assert.Equal(expectedErrors[i].Description, result.Errors[i].Description);
            Assert.Equal(expectedErrors[i].Code, result.Errors[i].Code);
        }
    }

    public static IEnumerable<object[]> GetValidMessageData()
    {
        yield return new object[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            "This is a valid message",
            DateTime.UtcNow.AddMinutes(-10),
        };

        yield return new object[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Another valid message",
            DateTime.UtcNow,
        };
    }

    public static IEnumerable<object[]> GetInvalidMessageData()
    {
        yield return new object[]
        {
            Guid.Empty,
            Guid.NewGuid(),
            "Valid message",
            DateTime.UtcNow,
            new List<ValidationError>
            {
                Error.Validation("The 'RoomId' is invalid", "ERR_INVALID_FIELD", "RoomId"),
            },
        };

        yield return new object[]
        {
            Guid.NewGuid(),
            Guid.Empty,
            "Valid message",
            DateTime.UtcNow,
            new List<ValidationError>
            {
                Error.Validation("The 'MemberId' is invalid", "ERR_INVALID_FIELD", "MemberId"),
            },
        };

        yield return new object[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            DateTime.UtcNow,
            new List<ValidationError>
            {
                Error.Validation(
                    "messageContent cannot be null or empty",
                    "ERR_IS_NULL_OR_EMPTY",
                    "messageContent"
                ),
                Error.Validation(
                    "messageContent must be between 1 and 500 characters",
                    "ERR_LENGTH_OUT_OF_RANGE",
                    "messageContent"
                ),
            },
        };

        yield return new object[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Valid message",
            DateTime.UtcNow.AddMinutes(10),
            new List<ValidationError>
            {
                Error.Validation(
                    "The 'SentAt' date cannot be in the future",
                    "ERR_INVALID_FIELD",
                    "SentAt"
                ),
            },
        };
    }
}
