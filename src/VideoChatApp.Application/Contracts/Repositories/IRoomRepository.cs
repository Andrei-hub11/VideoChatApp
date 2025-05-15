using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IRoomRepository : IRepository
{
    Task<RoomMapping?> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberMapping>> GetRoomMembersByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    );
    Task<MemberMapping?> GetRoomMemberByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<MessageMapping>> GetRoomMessagesByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    );
    Task<Guid> CreateRoomAsync(Room room, CancellationToken cancellationToken = default);
    Task<bool> CreateRoomMemberAsync(Member member, CancellationToken cancellationToken = default);
    Task<bool> CreateMessageAsync(Message message, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoomMemberAsync(
        Guid roomId,
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
}
