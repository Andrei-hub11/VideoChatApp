using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.DTOMappers;
using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Contracts.Request;
using VideoChatApp.Contracts.Response;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Application.Services.RoomService;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _roomRepository = unitOfWork.GetRepository<IRoomRepository>();
    }

    public async Task<Result<RoomResponseDTO>> GetRoomByIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId, cancellationToken);

            if (room is null)
            {
                return Result.Fail(RoomErrorFactory.RoomNotFoundById(roomId));
            }

            return Result.Ok(room.ToDTO());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<IEnumerable<MemberResponseDTO>>> GetRoomMembersByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var members = await _roomRepository.GetRoomMembersByRoomIdAsync(
                roomId,
                cancellationToken
            );

            return Result.Ok(members.Select(m => m.ToDTO()));
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<MemberResponseDTO>> GetRoomMemberByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var member = await _roomRepository.GetRoomMemberByUserIdAsync(
                userId,
                cancellationToken
            );

            if (member is null)
            {
                return Result.Fail(RoomErrorFactory.MemberNotFoundByUserId(userId));
            }

            return Result.Ok(member.ToDTO());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<IReadOnlyCollection<MessageResponseDTO>>> GetRoomMessagesByRoomIdAsync(
        Guid roomId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var messages = await _roomRepository.GetRoomMessagesByRoomIdAsync(
                roomId,
                cancellationToken
            );

            return Result.Ok(messages.ToDTO());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Result<RoomResponseDTO>> CreateRoomAsync(
        CreateRoomRequestDTO request,
        CreateMemberRequestDTO memberRequest,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var room = Room.Create(request.RoomName);

            if (room.IsFailure)
            {
                return Result.Fail(room.Errors);
            }

            var newRoomId = await _roomRepository.CreateRoomAsync(room.Value, cancellationToken);

            var member = Member.Create(newRoomId, memberRequest.UserId, memberRequest.Role);

            if (member.IsFailure)
            {
                return Result.Fail(member.Errors);
            }

            await _roomRepository.CreateRoomMemberAsync(member.Value, cancellationToken);

            _unitOfWork.Commit();

            var newRoom = await _roomRepository.GetRoomByIdAsync(newRoomId, cancellationToken);

            if (newRoom is null)
            {
                return Result.Fail(RoomErrorFactory.RoomNotFoundById(newRoomId));
            }

            return Result.Ok(newRoom.ToDTO());
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<Result<bool>> AddMemberToRoomAsync(
        Guid roomId,
        CreateMemberRequestDTO memberRequest,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var member = Member.Create(roomId, memberRequest.UserId, memberRequest.Role);

            if (member.IsFailure)
            {
                return Result.Fail(member.Errors);
            }

            await _roomRepository.CreateRoomMemberAsync(member.Value, cancellationToken);

            _unitOfWork.Commit();

            return Result.Ok(true);
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<Result<bool>> AddMessageToRoomAsync(
        Guid roomId,
        CreateMessageRequestDTO messageRequest,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var message = Message.Create(
                roomId,
                messageRequest.MemberId,
                messageRequest.Content,
                DateTime.UtcNow
            );

            if (message.IsFailure)
            {
                return Result.Fail(message.Errors);
            }

            await _roomRepository.CreateMessageAsync(message.Value, cancellationToken);

            _unitOfWork.Commit();

            return Result.Ok(true);
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<Result<bool>> RemoveMemberFromRoomAsync(
        Guid roomId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _roomRepository.DeleteRoomMemberAsync(roomId, userId, cancellationToken);

            _unitOfWork.Commit();

            return Result.Ok(true);
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<Result<bool>> DeleteRoomAsync(
        Guid roomId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _roomRepository.DeleteRoomAsync(roomId, cancellationToken);

            _unitOfWork.Commit();

            return Result.Ok(true);
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}
