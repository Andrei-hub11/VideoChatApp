using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsRoom
{
    public static RoomResponseDTO ToDTO(this RoomMapping room)
    {
        return new RoomResponseDTO(
            room.RoomId,
            room.RoomName,
            room.Members.Select(m => m.ToDTO()).ToList()
        );
    }
}
