using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsMember
{
    public static MemberResponseDTO ToDTO(this MemberMapping member)
    {
        return new MemberResponseDTO(member.MemberId, member.MemberRoomId, member.UserId, member.Role);
    }
}
