namespace VideoChatApp.Contracts.Request;

public sealed record CreateMessageRequestDTO(string Content, Guid MemberId, Guid RoomId);
