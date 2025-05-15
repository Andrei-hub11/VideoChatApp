using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Contracts.Request;

public sealed record RoomResponseDTO(
    Guid Id,
    string RoomName,
    IReadOnlyList<MemberResponseDTO> Members
);
