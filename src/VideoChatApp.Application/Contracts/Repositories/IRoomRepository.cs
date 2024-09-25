using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Contracts.Repositories;

public interface IRoomRepository: IRepository
{
    Task<bool> CreateRoomAsync(Room room, CancellationToken cancellationToken);
    Task<bool> CreateRoomMemberAsync(Member member, CancellationToken cancellationToken);
}
