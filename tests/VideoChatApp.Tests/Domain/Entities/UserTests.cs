using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;

namespace VideoChatApp.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_ValidUser_ReturnsUser()
    {
        // Arrange
        var id = "123";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "P@ssw0rd!";
        var profileImage = new byte[] { 1, 2, 3 }; 
        var profileImagePath = "/images/profile.jpg";
        var roles = new HashSet<string> { "User" };

        // Act
        var result = User.Create(id, name, email, password, profileImage, profileImagePath, roles);

        // Assert
        Assert.True(!result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(name, result.Value.UserName);
        Assert.Equal(email, result.Value.Email);
        Assert.NotEmpty(result.Value.PasswordHash);
        Assert.Equal(profileImage, result.Value.ProfileImage);
        Assert.Equal(profileImagePath, result.Value.ProfileImagePath);
        Assert.Equal(roles, result.Value.Roles);
    }

    [Theory]
    [InlineData("", "John Doe", "john.doe@example.com", "P@ssw0rd!", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "", "john.doe@example.com", "P@ssw0rd!", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "", "P@ssw0rd!", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "invalid-email", "P@ssw0rd!", "ERR_EMAIL_INVALID", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "short", "ERR_TOO_SHORT", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "P@sswwwwwwwwww", "ERR_INVALID_PASSWORD", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "P@ssw0rd!", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "P@ssw0rd!", "ERR_INVALID_PROFILE-IMAGE", 
        "path/to/image.jpg", 3 * 1024 * 1024)]
    public void Create_InvalidUser_ReturnsFailure(string id, string name, string email, string password, 
        string expectedErrorCode, string profileImagePath, int? imageSize = null)
    {
        // Arrange
        var profileImage = imageSize.HasValue ? new byte[imageSize.Value] : new byte[] { 1, 2, 3 };
        var roles = new HashSet<string> { "User" };

        // Act
        var result = User.Create(id, name, email, password, profileImage, profileImagePath, roles);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
    }

    [Theory]
    [InlineData("123", "John Doe", "john.doe@example.com", "hashedPassword", "path/to/image.jpg", new[] { "Admin", "User" })]
    [InlineData("456", "Jane Doe", "jane.doe@example.com", "hashedPassword", "", new[] { "User" })]
    public void From_ValidApplicationUserMapping_ReturnsSuccess(string id, string userName, string email, string passwordHash, string profileImagePath, string[] roles)
    {
        // Arrange
        var applicationUser = new ApplicationUserMapping
        {
            Id = id,
            UserName = userName,
            Email = email,
            PasswordHash = passwordHash,
            ProfileImagePath = profileImagePath,
            Roles = roles.ToHashSet()
        };

        // Act
        var result = User.From(applicationUser);

        // Assert
        Assert.True(!result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(userName, result.Value.UserName);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(passwordHash, result.Value.PasswordHash);
        Assert.Equal(profileImagePath, result.Value.ProfileImagePath);
        Assert.Equal(roles.ToHashSet(), result.Value.Roles);
    }

    [Theory]
    [InlineData("", "John Doe", "john.doe@example.com", "hashedPassword", "path/to/image.jpg", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData("123", "", "john.doe@example.com", "hashedPassword", "path/to/image.jpg", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData("123", "John Doe", "", "hashedPassword", "path/to/image.jpg", "ERR_EMAIL_INVALID")]
    public void From_InvalidApplicationUserMapping_ReturnsFailure(string id, string userName, string email, string passwordHash, string profileImagePath, string expectedErrorCode)
    {
        // Arrange
        var applicationUser = new ApplicationUserMapping
        {
            Id = id,
            UserName = userName,
            Email = email,
            PasswordHash = passwordHash,
            ProfileImagePath = profileImagePath,
            Roles = new HashSet<string> { "Admin" } 
        };

        // Act
        var result = User.From(applicationUser);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var id = "123";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "P@ssw0rd!";
        var profileImage = new byte[] { 1, 2, 3 };
        var profileImagePath = "/images/profile.jpg";
        var roles = new HashSet<string> { "User" };
        var user = User.Create(id, name, email, password, profileImage, profileImagePath, roles);

        // Act
        Assert.False(user.IsFailure);
        var result = user.Value.VerifyPassword(password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var id = "123";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "P@ssw0rd!";
        var profileImage = new byte[] { 1, 2, 3 };
        var profileImagePath = "/images/profile.jpg";
        var roles = new HashSet<string> { "User" };
        var user = User.Create(id, name, email, password, profileImage, profileImagePath, roles);

        // Act
        Assert.False(user.IsFailure);

        var result = user.Value.VerifyPassword("wrongpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateProfile_ValidData_UpdatesImage()
    {
        // Arrange
        var id = "123";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "P@ssw0rd!";
        var profileImage = new byte[] { 1, 2, 3 };
        var profileImagePath = "/images/profile.jpg";
        var roles = new HashSet<string> { "User" };
        var user = User.Create(id, name, email, password, profileImage, profileImagePath, roles);
        var newProfileImage = new byte[] { 4, 5, 6 };
        var newProfileImagePath = "/images/new-profile.jpg";

        // Act
        Assert.False(user.IsFailure);
        user.Value.UpdateProfile(name, newProfileImage, newProfileImagePath);

        Assert.Equal(newProfileImage, user.Value.ProfileImage);
        Assert.Equal(newProfileImagePath, user.Value.ProfileImagePath);
    }

    [Fact]
    public void UpdateProfileImage_InvalidData_ReturnsFailure()
    {
        // Arrange
        var id = "123";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "P@ssw0rd!";
        var profileImage = new byte[] { 1, 2, 3 };
        var profileImagePath = "/images/profile.jpg";
        var roles = new HashSet<string> { "User" };
        var user = User.Create(id, name, email, password, profileImage, profileImagePath, roles);
        var newName = "";
        var invalidProfileImage = new byte[3 * 1024 * 1024]; 
        var invalidProfileImagePath = string.Empty;

        // Act
        Assert.False(user.IsFailure);
        var result = user.Value.UpdateProfile(newName, invalidProfileImage, invalidProfileImagePath);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == "ERR_INVALID_PROFILE-IMAGE");
        Assert.Contains(result.Errors, e => e.Code == "ERR_IS_NULL_OR_EMPTY");
        Assert.Equal(3, result.Errors.Count);
    }

}
