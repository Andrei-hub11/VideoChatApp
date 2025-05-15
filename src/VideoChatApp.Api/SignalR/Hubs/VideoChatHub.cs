using Microsoft.AspNetCore.SignalR;
using VideoChatApp.Application.Contracts.Contexts;
using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Domain.ValueObjects;

namespace VideoChatApp.Api.SignalR.Hubs;

public sealed class VideoChatHub : BaseHub
{
    private readonly IAccountService _accountService;

    public VideoChatHub(
        IUserContext userContext,
        IRoomService roomService,
        IAccountService accountService,
        ILoggerHelper<BaseHub> logger
    )
        : base(roomService, userContext, logger)
    {
        _accountService = accountService;
    }

    public async Task CreateRoom(string roomName)
    {
        try
        {
            var memberExistsOnRoom = await _roomService.GetRoomMemberByUserIdAsync(
                UserContext.UserId
            );

            if (memberExistsOnRoom.Value is not null)
            {
                await _roomService.DeleteRoomAsync(memberExistsOnRoom.Value.RoomId);
            }

            var memberRequest = new CreateMemberRequestDTO(
                UserContext.UserId,
                MemberRole.Admin.Value
            );
            var result = await _roomService.CreateRoomAsync(
                new CreateRoomRequestDTO(roomName),
                memberRequest
            );

            if (result.IsFailure)
            {
                await Clients.Caller.SendAsync(
                    "HandleError",
                    $"Error creating room: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
                return;
            }

            await AddToGroup(result.Value.Id.ToString());

            var peerId = PeerIds.TryGetValue(UserContext.UserId, out var peerIdValue)
                ? peerIdValue
                : null;

            if (peerId == null)
            {
                throw new InvalidOperationException("Peer ID not found");
            }

            var roomResponse = new
            {
                id = result.Value.Id,
                roomName = result.Value.RoomName,
                members = result
                    .Value.Members.Select(m => new
                    {
                        memberId = m.MemberId,
                        roomId = m.RoomId,
                        userId = m.UserId,
                        role = m.Role,
                        peerId,
                    })
                    .ToList(),
            };

            await Clients.Caller.SendAsync("RoomCreated", roomResponse);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while creating a room.");
            await Clients.Caller.SendAsync(
                "HandleError",
                $"An error occurred while creating a room: {ex.Message}"
            );
        }
    }

    public async Task JoinRoom(Guid roomId)
    {
        try
        {
            var members = await _roomService.GetRoomMembersByRoomIdAsync(roomId);

            if (members.IsFailure)
            {
                await Clients.Caller.SendAsync("HandleError", "Room not found");
                return;
            }

            var admin = members.Value.FirstOrDefault(m => m.Role == MemberRole.Admin.Value);

            if (admin == null)
            {
                await Clients.Caller.SendAsync("HandleError", "Room has no admin");
                return;
            }

            var adminConnectionId = UserConnections.TryGetValue(admin.UserId, out var connectionId)
                ? connectionId
                : null;

            if (adminConnectionId == null)
            {
                throw new InvalidOperationException("Admin connection ID not found");
            }

            var requester = await _accountService.GetUserByIdAsync(UserContext.UserId);

            if (requester.IsFailure)
            {
                throw new InvalidOperationException("User not found");
            }

            var request = new
            {
                requesterId = UserContext.UserId,
                requesterName = requester.Value.UserName,
            };

            await Clients.Client(adminConnectionId).SendAsync("RequestJoinRoom", request);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while requesting to join a room.");
        }
    }

    public async Task RespondToJoinRequest(Guid roomId, string requesterId, bool accept)
    {
        try
        {
            var requesterConnectionId = UserConnections.TryGetValue(
                requesterId,
                out var connectionId
            )
                ? connectionId
                : null;

            if (requesterConnectionId == null)
            {
                throw new InvalidOperationException("Requester connection ID not found");
            }

            if (!accept)
            {
                await Clients.Client(requesterConnectionId).SendAsync("JoinDenied");
                return;
            }

            var memberRequest = new CreateMemberRequestDTO(requesterId, MemberRole.Member.Value);

            var result = await _roomService.AddMemberToRoomAsync(roomId, memberRequest);

            if (result.IsFailure)
            {
                await Clients.Caller.SendAsync("HandleError", "Error adding member to room");
                return;
            }

            var member = await _roomService.GetRoomMemberByUserIdAsync(requesterId);

            if (member.IsFailure)
            {
                await Clients.Caller.SendAsync("HandleError", "Error getting member from room");
                return;
            }

            var requesterPeerId = PeerIds.TryGetValue(requesterId, out var peerId) ? peerId : null;

            if (requesterPeerId == null)
            {
                throw new InvalidOperationException("Requester peer ID not found");
            }

            var requester = await _accountService.GetUserByIdAsync(requesterId);

            if (requester.IsFailure)
            {
                throw new InvalidOperationException("Requester not found");
            }

            var response = new
            {
                memberId = member.Value.MemberId,
                roomId = member.Value.RoomId,
                peerId = requesterPeerId,
                userId = requesterId,
                role = member.Value.Role,
            };

            await AddToGroup(roomId.ToString());

            await Clients.Group(roomId.ToString()).SendAsync("MemberJoined", response);

            await Clients.Client(requesterConnectionId).SendAsync("JoinAccepted");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while responding to a join request.");
        }
    }

    public async Task SendMessageToRoom(CreateMessageRequestDTO request)
    {
        try
        {
            var result = await _roomService.AddMessageToRoomAsync(request.RoomId, request);

            if (result.IsFailure)
            {
                await Clients.Caller.SendAsync(
                    "HandleError",
                    $"Error sending message to room: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
                return;
            }

            var messages = await _roomService.GetRoomMessagesByRoomIdAsync(request.RoomId);

            if (messages.IsFailure)
            {
                await Clients.Caller.SendAsync(
                    "HandleError",
                    $"Error getting messages from room: {string.Join(", ", messages.Errors.Select(e => e.Description))}"
                );
                return;
            }

            var lastMessage = messages.Value.Last();

            await Clients.Group(request.RoomId.ToString()).SendAsync("MessageSent", lastMessage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while sending a message to a room.");
        }
    }

    public async Task LeaveRoom(Guid roomId, string userId)
    {
        await Clients.Group(roomId.ToString()).SendAsync("MemberLeft", userId);
        await _roomService.RemoveMemberFromRoomAsync(roomId, userId);
        var roomExists = await _roomService.GetRoomByIdAsync(roomId);

        if (!roomExists.IsFailure && roomExists.Value.Members.Count == 0)
        {
            await _roomService.DeleteRoomAsync(roomId);
        }

        await RemoveFromGroup(roomId.ToString());
    }
}
