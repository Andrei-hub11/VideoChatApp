namespace VideoChatApp.Contracts.Response;

public sealed record MessageResponseDTO(
    Guid MessageId,
    Guid RoomId,
    Guid MemberId,
    string Content,
    DateTime SentAt
);
