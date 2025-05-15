using VideoChatApp.Common.Utils.Validation;

namespace VideoChatApp.Tests.Utillities;

public class PasswordValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidatePassword_WithNullOrEmptyPassword_ShouldReturnError(string password)
    {
        // Act
        var result = PasswordValidator.ValidatePassword(password);

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Contains(result, error =>
            error.Code == "ERR_IS_NULL_OR_EMPTY" &&
            error.Description.Contains("cannot be null or empty")
        );

        Assert.Contains(result, error =>
            error.Code == "ERR_TOO_SHORT" &&
            error.Description.Contains("must have at least 8 characters")
        );

        Assert.Contains(result, error =>
            error.Code == "ERR_INVALID_PASSWORD" &&
            error.Description.Contains("must have at least two special characters")
        );
    }

    [Theory]
    [InlineData("abc!@")] // Too short
    [InlineData("abcd!@")] // Still too short
    public void ValidatePassword_WithTooShortPassword_ShouldReturnError(string password)
    {
        // Act
        var result = PasswordValidator.ValidatePassword(password);

        // Assert
        Assert.Contains(result, e => e.Code == "ERR_TOO_SHORT");
        Assert.Contains(result, e => e.Description.Contains("must have at least 8 characters"));
    }

    [Theory]
    [InlineData("password123")] // No special characters
    [InlineData("password!")] // Only one special character
    public void ValidatePassword_WithInsufficientSpecialCharacters_ShouldReturnError(
        string password
    )
    {
        // Act
        var result = PasswordValidator.ValidatePassword(password);

        // Assert
        Assert.Contains(result, e => e.Code == "ERR_INVALID_PASSWORD");
        Assert.Contains(
            result,
            e => e.Description.Contains("must have at least two special characters")
        );
    }

    [Theory]
    [InlineData("abc")] // Multiple violations: too short, no special chars
    [InlineData("")] // Multiple violations: empty, too short, no special chars
    public void ValidatePassword_WithMultipleViolations_ShouldReturnMultipleErrors(string password)
    {
        // Act
        var result = PasswordValidator.ValidatePassword(password);

        // Assert
        Assert.True(result.Count > 1);
        if (string.IsNullOrEmpty(password))
        {
            Assert.Contains(result, e => e.Code == "ERR_IS_NULL_OR_EMPTY");
        }
        Assert.Contains(result, e => e.Code == "ERR_TOO_SHORT");
        Assert.Contains(result, e => e.Code == "ERR_INVALID_PASSWORD");
    }

    [Theory]
    [InlineData("Password!@123")] // Valid with !@
    [InlineData("SecurePass#$99")] // Valid with #$
    [InlineData("MyP@ss!word123")] // Valid with @!
    public void ValidatePassword_WithValidPassword_ShouldReturnNoErrors(string password)
    {
        // Act
        var result = PasswordValidator.ValidatePassword(password);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ValidatePassword_WithCustomFieldName_ShouldUseCustomFieldNameInErrors()
    {
        // Arrange
        var customFieldName = "UserPassword";
        var password = "short!"; // Too short and only one special character

        // Act
        var result = PasswordValidator.ValidatePassword(password, customFieldName);

        // Assert
        Assert.True(result.Count > 1);
        Assert.All(result, error => Assert.Equal(customFieldName, error.Field));
    }
}
