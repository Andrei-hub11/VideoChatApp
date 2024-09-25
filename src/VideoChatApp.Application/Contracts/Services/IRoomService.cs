using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Application.Contracts.Services;

public interface IRoomService
{
    Task<Result<RoomResponseDTO>> CreateRoomAsync(CreateRoomRequestDTO request, 
        CancellationToken cancellationToken);
}
