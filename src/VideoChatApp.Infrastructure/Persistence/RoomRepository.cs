using System.Data;

using Dapper;

using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Infrastructure.Persistence;

public class RoomRepository : IRoomRepository
{
    private IDbConnection? _connection = null;
    private IDbTransaction? _transaction = null;

    private IDbConnection Connection =>
        _connection ?? throw new InvalidOperationException("Connection has not been initialized.");
    private IDbTransaction Transaction =>
        _transaction
        ?? throw new InvalidOperationException("Transaction has not been initialized.");

    public void Initialize(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<RoomMapping?> GetRoomByIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"SELECT 
            Room.RoomId, Room.RoomName, 
            Member.MemberId, Member.RoomId AS MemberRoomId, Member.UserId, Member.Role
          FROM Room
          LEFT JOIN Member ON Room.RoomId = Member.RoomId
          WHERE Room.RoomId = @RoomId";

        var roomResult = await Connection.QueryAsync<RoomMapping, MemberMapping, RoomMapping>(
            query,
            (room, member) =>
            {
                if (room == null)
                {
                    return null!;
                }

                if (room.Members == null)
                {
                    room.Members = [];
                }

                if (member != null)
                {
                    room.Members.ToList().Add(member);
                }

                return room;
            },
            new { RoomId = roomId },
            transaction: Transaction,
            splitOn: "MemberId"
        );

        return roomResult.FirstOrDefault();
    }

    public async Task<IEnumerable<MemberMapping>> GetRoomMembersByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"SELECT * FROM Member WHERE RoomId = @RoomId ORDER BY SentAt DESC";

        return await Connection.QueryAsync<MemberMapping>(
            query,
            new { roomId },
            transaction: Transaction
        );
    }

    public async Task<MemberMapping?> GetRoomMemberByUserIdAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"SELECT * FROM Member WHERE UserId = @UserId";

        return await Connection.QuerySingleOrDefaultAsync<MemberMapping>(
            query,
            new { UserId = userId },
            transaction: Transaction
        );
    }

    public async Task<IEnumerable<MessageMapping>> GetRoomMessagesByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"SELECT * FROM Message WHERE RoomId = @RoomId";

        return await Connection.QueryAsync<MessageMapping>(
            query,
            new { RoomId = roomId },
            transaction: Transaction
        );
    }

    public async Task<Guid> CreateRoomAsync(Room room, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"
            INSERT INTO Room (RoomName)
            OUTPUT INSERTED.RoomId
            VALUES (@RoomName);
            ";

        Guid newId = await Connection.QuerySingleAsync<Guid>(
            query,
            new { room.RoomName },
            transaction: Transaction
        );

        return newId;
    }

    public async Task<bool> CreateRoomMemberAsync(
        Member member,
        CancellationToken cancellationToken
    )
    {
        const string query =
            @"
            INSERT INTO Member (RoomId, UserId, Role)
            VALUES (@RoomId, @UserId, @Role);
            ";

        int result = await Connection.ExecuteAsync(
            query,
            new
            {
                member.RoomId,
                member.UserId,
                member.Role,
            },
            transaction: Transaction
        );

        return result > 0;
    }

    public async Task<bool> CreateMessageAsync(Message message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query =
            @"INSERT INTO Message (RoomId, MemberId, Content, SentAt) VALUES (@RoomId, @MemberId, @Content, @SentAt)";

        int result = await Connection.ExecuteAsync(
            query,
            new
            {
                message.RoomId,
                message.MemberId,
                message.Content,
                message.SentAt,
            },
            transaction: Transaction
        );

        return result > 0;
    }

    public async Task<bool> DeleteRoomMemberAsync(
        Guid roomId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        const string query = @"DELETE FROM Member WHERE RoomId = @RoomId AND UserId = @UserId";

        int result = await Connection.ExecuteAsync(
            query,
            new { RoomId = roomId, UserId = userId },
            transaction: Transaction
        );

        return result > 0;
    }

    public async Task<bool> DeleteRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string query = @"DELETE FROM Room WHERE RoomId = @RoomId";

        int result = await Connection.ExecuteAsync(
            query,
            new { RoomId = roomId },
            transaction: Transaction
        );

        return result > 0;
    }
}
