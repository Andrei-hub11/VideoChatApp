namespace VideoChatApp.Contracts.Response;

public sealed record MemberResponseDTO(Guid MemberId, Guid RoomId, string UserId, string Role);
