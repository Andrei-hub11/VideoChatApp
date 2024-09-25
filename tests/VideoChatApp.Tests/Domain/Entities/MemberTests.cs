using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Tests.Domain.Entities;

public class MemberTests
{
    [Theory]
    [MemberData(nameof(ValidMemberData))]
    public void Create_WithValidData_ReturnsSuccess(Guid roomId, string userId, string memberName, string role)
    {
        // Act
        var result = Member.Create(roomId, userId, memberName, role);

        // Assert
        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(roomId, result.Value.RoomId);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(memberName, result.Value.MemberName);
        Assert.Equal(role, result.Value.Role);
    }

    [Theory]
    [MemberData(nameof(InvalidMemberData))]
    public void Create_WithInvalidData_ReturnsFailure(Guid roomId, string userId, string memberName, string role, string expectedErrorCode)
    {
        // Act
        var result = Member.Create(roomId, userId, memberName, role);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
    }

    public static IEnumerable<object[]> ValidMemberData()
    {
        yield return new object[] { Guid.NewGuid(), Guid.NewGuid().ToString(), "Valid Member", "member" };
        yield return new object[] { Guid.NewGuid(), Guid.NewGuid().ToString(), "Another Valid Member", "admin" };
        yield return new object[] { Guid.NewGuid(), Guid.NewGuid().ToString(), "Valid Member Name", "moderator" };
    }

    public static IEnumerable<object[]> InvalidMemberData()
    {
        yield return new object[] { Guid.Empty, "valid-user-id", "", "member", "ERR_INVALID_FIELD" }; // Invalid RoomId
        yield return new object[] { Guid.NewGuid(), "invalid-guid", "Valid Name", "member", "ERR_INVALID_FIELD" }; // Invalid UserId
        yield return new object[] { Guid.NewGuid(), Guid.NewGuid().ToString(), "Valid Name", "invalid-role", "ERR_INVALID_FIELD" }; // Invalid Role
        yield return new object[] { Guid.NewGuid(), Guid.NewGuid().ToString(), "", "invalid-role", "ERR_IS_NULL_OR_EMPTY" }; // Invalid MemberName
    }
}
