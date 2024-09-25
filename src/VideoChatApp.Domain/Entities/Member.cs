using System.Collections.ObjectModel;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Domain.GuardClause;
using VideoChatApp.Domain.ValueObjects;

namespace VideoChatApp.Domain.Entities;

public sealed class Member
{
    public Guid MemberId { get; private set; }
    public Guid RoomId { get; private set; } 
    public string UserId { get; private set; } = string.Empty;
    public string MemberName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    private Member(Guid memberId, Guid roomId, string userId, string memberName, string role)
    {
        MemberId = memberId;
        RoomId = roomId;
        UserId = userId;
        MemberName = memberName;
        Role = role;
    }

    public static Result<Member> Create(Guid roomId, string userId, string memberName, string role)
    {
        var errors = ValidateMember(roomId, userId, memberName, role);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        return new Member(Guid.Empty, roomId, userId, memberName, role);
    }

    private static ReadOnlyCollection<ValidationError> ValidateMember(Guid roomId, string userId, 
        string memberName, string role)
    {
        var errors = new List<ValidationError>();

        var result = Guard.For().Use((guard) =>
        {
            guard.IsNullOrWhiteSpace(memberName)
            .InRange(memberName, 1, 450)
            .FailIf(roomId == Guid.Empty, 
                "The 'RoomId' is invalid", "ERR_INVALID_FIELD", "RoomId")
            .FailIf(!Guid.TryParse(userId, out _),
                "The 'UserId' is invalid", "ERR_INVALID_FIELD", "UserId")
            .FailIf(!MemberRole.IsValidRole(role),
                "The 'Role' is invalid", "ERR_INVALID_FIELD", "Role")
            .DoNotThrowOnError();
        });

        errors.AddRange(result.Errors);

        return errors.AsReadOnly();
    }
}
