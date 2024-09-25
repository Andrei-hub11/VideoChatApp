namespace VideoChatApp.Contracts.Request;

public sealed record RoomResponseDTO(
    Guid RoomId, 
    string RoomName
    );
