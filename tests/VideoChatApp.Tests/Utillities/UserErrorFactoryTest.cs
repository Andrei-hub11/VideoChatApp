using VideoChatApp.Common.Utils.Errors;

namespace VideoChatApp.Tests.Utillities;

public class UserErrorFactoryTest
{
    [Fact]
    public void UserNotFoundById_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var userId = "123";

        // Act
        var error = UserErrorFactory.UserNotFoundById(userId);

        // Assert
        Assert.Equal("ERR_USER_NOT_FOUND", error.Code);
        Assert.Equal($"User with id = '{userId}' was not found.", error.Description);
    }

    [Fact]
    public void UserNotFoundByName_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var userName = "john_doe";

        // Act
        var error = UserErrorFactory.UserNotFoundByName(userName);

        // Assert
        Assert.Equal("ERR_USER_NOT_FOUND", error.Code);
        Assert.Equal($"User with username = '{userName}' was not found.", error.Description);
    }

    [Fact]
    public void UserNotFoundByEmail_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var email = "john@example.com";

        // Act
        var error = UserErrorFactory.UserNotFoundByEmail(email);

        // Assert
        Assert.Equal("ERR_USER_NOT_FOUND", error.Code);
        Assert.Equal($"User with email = '{email}' was not found.", error.Description);
    }

    [Fact]
    public void EmailAlreadyExists_ShouldReturnConflictError()
    {
        // Arrange
        var email = "john@example.com";

        // Act
        var error = UserErrorFactory.EmailAlreadyExists(email);

        // Assert
        Assert.Equal("ERR_EMAIL_CONFLICT", error.Code);
        Assert.Equal($"The email '{email}' is already registered.", error.Description);
    }
}
