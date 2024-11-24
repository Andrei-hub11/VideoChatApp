using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.Entities;
using VideoChatApp.Tests.Builders;

namespace VideoChatApp.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_ValidUser_ReturnsUser()
    {
        // Arrange
        var userBuilder = new UserBuilder()
            .WithId("123")
            .WithName("John Doe")
            .WithEmail("john.doe@example.com")
            .WithProfileImage(new byte[] { 1, 2, 3 })
            .WithProfileImagePath("/images/profile.jpg")
            .WithRoles(new HashSet<string> { "User" });

        // Act
        var result = userBuilder.Build();

        // Assert
        Assert.True(!result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal("123", result.Value.Id);
        Assert.Equal("John Doe", result.Value.UserName);
        Assert.Equal("john.doe@example.com", result.Value.Email);
        Assert.Equal(new byte[] { 1, 2, 3 }, result.Value.ProfileImage);
        Assert.Equal("/images/profile.jpg", result.Value.ProfileImagePath);
        Assert.Equal(new HashSet<string> { "User" }, result.Value.Roles);
    }

    [Theory]
    [InlineData("", "John Doe", "john.doe@example.com", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "", "john.doe@example.com", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "invalid-email", "ERR_EMAIL_INVALID", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "ERR_IS_NULL_OR_EMPTY", "")]
    [InlineData("123", "John Doe", "john.doe@example.com", "ERR_INVALID_PROFILE-IMAGE",
        "path/to/image.jpg", 3 * 1024 * 1024)]
    public void Create_InvalidUser_ReturnsFailure(string id, string name, string email,
        string expectedErrorCode, string profileImagePath, int? imageSize = null)
    {
        // Arrange
        var profileImage = imageSize.HasValue ? new byte[imageSize.Value] : new byte[] { 1, 2, 3 };
        var roles = new HashSet<string> { "User" };

        // Act
        var result = User.Create(id, name, email, profileImage, profileImagePath, roles);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
    }


    [Theory]
    [InlineData("123", "John Doe", "john.doe@example.com", "path/to/image.jpg", new[] { "Admin", "User" })]
    [InlineData("456", "Jane Doe", "jane.doe@example.com", "", new[] { "User" })]
    public void From_ValidApplicationUserMapping_ReturnsSuccess(string id, string userName, string email,
        string profileImagePath, string[] roles)
    {
        // Arrange
        var applicationUser = new ApplicationUserMapping
        {
            Id = id,
            UserName = userName,
            Email = email,
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
        Assert.Equal(profileImagePath, result.Value.ProfileImagePath);
        Assert.Equal(roles.ToHashSet(), result.Value.Roles);
    }

    [Theory]
    [InlineData("", "John Doe", "john.doe@example.com", "path/to/image.jpg", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData("123", "", "john.doe@example.com", "path/to/image.jpg", "ERR_IS_NULL_OR_EMPTY")]
    [InlineData("123", "John Doe", "", "path/to/image.jpg", "ERR_EMAIL_INVALID")]
    public void From_InvalidApplicationUserMapping_ReturnsFailure(string id, string userName, string email,
        string profileImagePath, string expectedErrorCode)
    {
        // Arrange
        var applicationUser = new ApplicationUserMappingBuilder()
        .WithId(id)
        .WithUserName(userName)
        .WithEmail(email)
        .WithProfileImagePath(profileImagePath)
        .WithRoles(new HashSet<string> { "Admin" })
        .Build();

        // Act
        var result = User.From(applicationUser);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == expectedErrorCode);
    }

    [Theory]
    [InlineData("New Name", null, null, null, "New Name", "john.doe@example.com", new byte[] { 1, 2, 3 }, "/images/profile.jpg")]
    [InlineData(null, null, null, "new.email@example.com", "John Doe", "new.email@example.com", new byte[] { 1, 2, 3 }, "/images/profile.jpg")]
    [InlineData(null, new byte[] { 4, 5, 6 }, "/images/new-profile.jpg", null, "John Doe", "john.doe@example.com", new byte[] { 4, 5, 6 }, "/images/new-profile.jpg")]
    [InlineData("New Name", new byte[] { 7, 8, 9 }, "/images/newer-profile.jpg", "new.email@example.com", "New Name", "new.email@example.com", new byte[] { 7, 8, 9 }, "/images/newer-profile.jpg")]
    public void UpdateProfile_ValidData_UpdatesCorrectFields(
    string? newName,
    byte[]? newProfileImage,
    string? newProfileImagePath,
    string? newEmail,
    string expectedName,
    string expectedEmail,
    byte[] expectedProfileImage,
    string expectedProfileImagePath)
    {
        // Arrange
        var user = new UserBuilder()
             .WithId("123")
             .WithName("John Doe")
             .WithEmail("john.doe@example.com")
             .WithProfileImage(new byte[] { 1, 2, 3 })
             .WithProfileImagePath("/images/profile.jpg")
             .WithRoles(new HashSet<string> { "User" })
             .Build();

        // Act
        Assert.False(user.IsFailure);
        user.Value.UpdateProfile(newName, newEmail, newProfileImage, newProfileImagePath);

        // Assert
        Assert.Equal(expectedName, user.Value.UserName);
        Assert.Equal(expectedEmail, user.Value.Email);
        Assert.Equal(expectedProfileImage, user.Value.ProfileImage);
        Assert.Equal(expectedProfileImagePath, user.Value.ProfileImagePath);
    }

    [Theory]
    [MemberData(nameof(GetInvalidProfileUpdateData))]
    public void UpdateProfile_InvalidData_ReturnsFailure(
    string? newUsername,
    byte[]? newProfileImage,
    string? newProfileImagePath,
    string? newEmail,
    string[] expectedErrorCodes)
    {
        // Arrange
        var user = new UserBuilder()
               .WithId("123")
               .Build();

        // Act
        Assert.False(user.IsFailure);
        var result = user.Value.UpdateProfile(newUsername, newEmail, newProfileImage, newProfileImagePath);

        // Assert
        Assert.True(result.IsFailure);
        foreach (var errorCode in expectedErrorCodes)
        {
            Assert.Contains(result.Errors, e => e.Code == errorCode);
        }

        Assert.Equal(expectedErrorCodes.Length, result.Errors.Count);
    }

    public static IEnumerable<object[]?> GetInvalidProfileUpdateData()
    {
        yield return new object[] { "", null!, null!, null!, new[] { "ERR_IS_NULL_OR_EMPTY" } };

        yield return new object[] { null!, new byte[3 * 1024 * 1024], "/images/profile.jpg", null!, new[] { "ERR_INVALID_PROFILE-IMAGE" } };

        yield return new object[] { "New Name", new byte[] { 1, 2, 3 }, "", null!, new[] { "ERR_IS_NULL_OR_EMPTY" } };

        yield return new object[] { "New Name", null!, null!, "", new[] { "ERR_IS_NULL_OR_EMPTY", "ERR_EMAIL_INVALID" } };

        yield return new object[] { "New Name", null!, null!, "invalid-email@", new[] { "ERR_EMAIL_INVALID" } };
    }
}
