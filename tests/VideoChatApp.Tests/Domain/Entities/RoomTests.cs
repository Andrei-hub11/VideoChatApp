using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Tests.Domain.Entities;

public class RoomTests
{
    [Fact]
    public void Create_WithValidRoomName_ShouldSucceed()
    {
        // Arrange
        var roomName = "Test Room";

        // Act
        var result = Room.Create(roomName);

        // Assert
        Assert.True(!result.IsFailure);
        Assert.Equal(roomName, result.Value.RoomName);
        Assert.NotEqual(Guid.Empty, result.Value.RoomId);
    }

    [Theory]
    [InlineData("", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData(" ", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData(null, "ERR_IS_NULL_OR_EMPTY")]
    public void Create_WithInvalidRoomName_ShouldFail(
        string invalidRoomName,
        string expectedErrorCode
    )
    {
        // Act
        var result = Room.Create(invalidRoomName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, error => error.Code == expectedErrorCode);
    }

    [Fact]
    public void Create_WithRoomNameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longRoomName = new string('a', 101); // 101 characters

        // Act
        var result = Room.Create(longRoomName);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_WithValidRoomName_ShouldSetCreatedAtToCurrentBrasiliaTime()
    {
        // Arrange
        var roomName = "Test Room";
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = Room.Create(roomName);

        // Assert
        Assert.True(!result.IsFailure);
        Assert.True(result.Value.CreatedAt >= beforeCreate);
        Assert.True(result.Value.CreatedAt <= DateTime.UtcNow);
    }
}
