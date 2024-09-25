using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using System.Collections.Concurrent;

using VideoChatApp.Application.Contracts.Contexts;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Contracts.Request;

namespace VideoChatApp.Api.SignalR.Hubs;

[Authorize]
public sealed class VideoChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> userConnections
     = new ConcurrentDictionary<string, string>();

    private readonly IUserContext _userContext;
    private readonly IRoomService _roomService;

    public VideoChatHub(IUserContext userContext, IRoomService roomService)
    {
        _userContext = userContext;
        _roomService = roomService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _userContext.UserId;
        var connectionId = Context.ConnectionId;

        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                userConnections.AddOrUpdate(userId, connectionId, (_, existingConnectionId) => connectionId);
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("HandleError", "Ocorreu um erro ao estabelecer a conexão.");
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _userContext.UserId;


        if (!string.IsNullOrEmpty(userId))
        {
            userConnections.TryRemove(userId, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(CreateRoomRequestDTO request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roomService.CreateRoomAsync(request, cancellationToken);

            if (result.IsFailure)
            {
                await Clients.Caller.SendAsync("HandleError", $"Erro ao criar sala: {result.Error}", 
                    cancellationToken: cancellationToken);
            }

            await Clients.Caller.SendAsync("RoomCreated", result.Value, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("HandleError", $"Erro ao criar sala: {ex.Message}", 
                cancellationToken: cancellationToken);
        }
    }
}
