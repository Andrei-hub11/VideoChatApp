using VideoChatApp.Application.Common.Result;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.Contracts.Services;

public interface IRoomService
{
    Task<Result<RoomResponseDTO>> GetRoomByIdAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    );
    Task<Result<IEnumerable<MemberResponseDTO>>> GetRoomMembersByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    );
    Task<Result<MemberResponseDTO>> GetRoomMemberByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<Result<IReadOnlyCollection<MessageResponseDTO>>> GetRoomMessagesByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    );
    Task<Result<RoomResponseDTO>> CreateRoomAsync(
        CreateRoomRequestDTO request,
        CreateMemberRequestDTO memberRequest,
        CancellationToken cancellationToken = default
    );
    Task<Result<bool>> AddMemberToRoomAsync(
        Guid roomId,
        CreateMemberRequestDTO memberRequest,
        CancellationToken cancellationToken = default
    );
    Task<Result<bool>> AddMessageToRoomAsync(
        Guid roomId,
        CreateMessageRequestDTO messageRequest,
        CancellationToken cancellationToken = default
    );
    Task<Result<bool>> RemoveMemberFromRoomAsync(
        Guid roomId,
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<Result<bool>> DeleteRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}
