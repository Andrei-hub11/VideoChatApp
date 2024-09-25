using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Application.Services.Room;

public class RoomService : IRoomService
{
    public Task<Result<RoomResponseDTO>> CreateRoomAsync(CreateRoomRequestDTO request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
