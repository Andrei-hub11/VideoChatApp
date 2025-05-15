using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Contexts;
using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Api.SignalR.Hubs;

[Authorize]
public abstract class BaseHub : Hub
{
    protected static readonly ConcurrentDictionary<string, string> UserConnections = new();
    protected static readonly ConcurrentDictionary<string, string> PeerIds = new();

    protected readonly IRoomService _roomService;
    protected readonly IUserContext UserContext;
    protected readonly ILoggerHelper<BaseHub> Logger;

    protected BaseHub(
        IRoomService roomService,
        IUserContext userContext,
        ILoggerHelper<BaseHub> logger
    )
    {
        _roomService = roomService;
        UserContext = userContext;
        Logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = UserContext.UserId;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            UserConnections.AddOrUpdate(
                userId,
                Context.ConnectionId,
                (_, _) => Context.ConnectionId
            );
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = UserContext.UserId;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            UserConnections.TryRemove(userId, out _);
        }

        var member = await _roomService.GetRoomMemberByUserIdAsync(userId);

        if (!member.IsFailure)
        {
            await RemoveFromGroup(member.Value.RoomId.ToString());
            await _roomService.RemoveMemberFromRoomAsync(member.Value.RoomId, userId);
            var roomExists = await _roomService.GetRoomByIdAsync(member.Value.RoomId);

            await DeleteRoomIfEmpty(member.Value, roomExists);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SetPeerId(string peerConnectionId)
    {
        var userId = UserContext.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User ID is required to set the peer ID.");
        }

        PeerIds.AddOrUpdate(userId, peerConnectionId, (_, _) => peerConnectionId);

        await Task.CompletedTask;
    }

    protected async Task AddToGroup(string groupName, CancellationToken cancellationToken = default)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName, cancellationToken);
    }

    protected async Task RemoveFromGroup(
        string groupName,
        CancellationToken cancellationToken = default
    )
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName, cancellationToken);
    }

    private async Task DeleteRoomIfEmpty(
        MemberResponseDTO member,
        Result<RoomResponseDTO> roomExists
    )
    {
        if (roomExists.IsFailure || roomExists.Value.Members.Count == 0)
        {
            return;
        }

        await _roomService.DeleteRoomAsync(member.RoomId);
    }
}
