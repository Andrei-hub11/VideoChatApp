using VideoChatApp.Common.Utils.GuardClause;

namespace VideoChatApp.Tests.Utillities;

public class GuardClauseTests
{
    [Fact]
    public void Guard_WithoutThrowOrDoNotThrow_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var guard = Guard.For();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            guard.Use(g => g.IsNullOrWhiteSpace("valid"));
        });

        Assert.Equal(
            "'ThrowIfInvalid' or 'DoNotThrowOnError' must be called to complete the chain.",
            exception.Message
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void IsNullOrWhiteSpace_WithInvalidInput_ShouldReturnError(string input)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.IsNullOrWhiteSpace(input).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_IS_NULL_OR_EMPTY", result.Errors[0].Code);
        Assert.Contains("cannot be null or empty", result.Errors[0].Description);
    }

    [Fact]
    public void IsNullOrWhiteSpace_WithValidInput_ShouldNotReturnError()
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.IsNullOrWhiteSpace("valid input").DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("test", 3)]
    [InlineData("toolong", 5)]
    public void MaxLength_ExceedsLimit_ShouldReturnError(string input, int maxLength)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.MaxLength(input, maxLength).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_TOO_LONG", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("test", 5)]
    [InlineData("short", 10)]
    public void MaxLength_WithinLimit_ShouldNotReturnError(string input, int maxLength)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.MaxLength(input, maxLength).DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("a", 2)]
    [InlineData("test", 5)]
    public void MinLength_BelowLimit_ShouldReturnError(string input, int minLength)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.MinLength(input, minLength).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_TOO_SHORT", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("test", 3)]
    [InlineData("long", 2)]
    public void MinLength_AboveLimit_ShouldNotReturnError(string input, int minLength)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.MinLength(input, minLength).DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("abc123", @"^[0-9]+$", "Must be numeric", "ERR_NOT_NUMERIC")]
    [InlineData("test@", @"^[a-zA-Z0-9]+$", "Must be alphanumeric", "ERR_NOT_ALPHANUMERIC")]
    public void MatchesPattern_WithInvalidPattern_ShouldReturnError(
        string input,
        string pattern,
        string errorMessage,
        string errorCode
    )
    {
        // Arrange & Act
        var result = Guard
            .For()
            .Use(g =>
                g.MatchesPattern(input, pattern, errorMessage, errorCode).DoNotThrowOnError()
            );

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal(errorCode, result.Errors[0].Code);
        Assert.Equal(errorMessage, result.Errors[0].Description);
    }

    [Theory]
    [InlineData("123", @"^[0-9]+$")]
    [InlineData("abc123", @"^[a-zA-Z0-9]+$")]
    public void MatchesPattern_WithValidPattern_ShouldNotReturnError(string input, string pattern)
    {
        // Arrange & Act
        var result = Guard
            .For()
            .Use(g =>
                g.MatchesPattern(input, pattern, "Error message", "ERROR_CODE").DoNotThrowOnError()
            );

        // Assert
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void AllNumeric_WithNonNumericInput_ShouldReturnError()
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.AllNumeric("abc123").DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_NOT_NUMERIC", result.Errors[0].Code);
    }

    [Fact]
    public void AllNumeric_WithNumericInput_ShouldNotReturnError()
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.AllNumeric("12345").DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void IsAlphanumeric_WithNonAlphanumericInput_ShouldReturnError()
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.IsAlphanumeric("test@123").DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_NOT_ALPHANUMERIC", result.Errors[0].Code);
    }

    [Fact]
    public void IsAlphanumeric_WithAlphanumericInput_ShouldNotReturnError()
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.IsAlphanumeric("Test123").DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void MultipleValidations_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange & Act
        var result = Guard
            .For()
            .Use(g =>
                g.IsNullOrWhiteSpace("")
                    .MaxLength("toolong", 5)
                    .AllNumeric("abc")
                    .DoNotThrowOnError()
            );

        // Assert
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Code == "ERR_IS_NULL_OR_EMPTY");
        Assert.Contains(result.Errors, e => e.Code == "ERR_TOO_LONG");
        Assert.Contains(result.Errors, e => e.Code == "ERR_NOT_NUMERIC");
    }

    [Theory]
    [InlineData("testing", 3, 5)]
    [InlineData("toolong", 2, 4)]
    public void InRange_OutOfRange_ShouldReturnError(string input, int min, int max)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.InRange(input, min, max).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_LENGTH_OUT_OF_RANGE", result.Errors[0].Code);
        Assert.Contains(
            $"must be between {min} and {max} characters",
            result.Errors[0].Description
        );
    }

    [Theory]
    [InlineData("test", 2, 5)]
    [InlineData("abc", 2, 4)]
    public void InRange_WithinRange_ShouldNotReturnError(string input, int min, int max)
    {
        // Arrange & Act
        var result = Guard.For().Use(g => g.InRange(input, min, max).DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Contains_EmptyCollection_ShouldReturnError()
    {
        // Arrange
        var collection = Array.Empty<int>();

        // Act
        var result = Guard.For().Use(g => g.Contains(collection, x => x > 0).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_MISSING_VALID_ITEM", result.Errors[0].Code);
    }

    [Fact]
    public void Contains_NoValidItems_ShouldReturnError()
    {
        // Arrange
        var collection = new[] { -1, -2, -3 };

        // Act
        var result = Guard.For().Use(g => g.Contains(collection, x => x > 0).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal("ERR_MISSING_VALID_ITEM", result.Errors[0].Code);
    }

    [Fact]
    public void Contains_HasValidItems_ShouldNotReturnError()
    {
        // Arrange
        var collection = new[] { -1, 2, -3 };

        // Act
        var result = Guard.For().Use(g => g.Contains(collection, x => x > 0).DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(1024 * 1024 + 1, 1024 * 1024)] // 1MB + 1 byte, max 1MB
    [InlineData(2048, 1024)] // 2KB, max 1KB
    public void MaxSize_ExceedsLimit_ShouldReturnError(int size, int maxSize)
    {
        // Arrange
        var data = new byte[size];
        var errorMessage = "File too large";
        var errorCode = "ERR_FILE_TOO_LARGE";

        // Act
        var result = Guard
            .For()
            .Use(g => g.MaxSize(data, maxSize, errorMessage, errorCode).DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal(errorCode, result.Errors[0].Code);
        Assert.Equal(errorMessage, result.Errors[0].Description);
    }

    [Theory]
    [InlineData(1024, 1024)] // 1KB, max 1KB
    [InlineData(512, 1024)] // 512B, max 1KB
    public void MaxSize_WithinLimit_ShouldNotReturnError(int size, int maxSize)
    {
        // Arrange
        var data = new byte[size];

        // Act
        var result = Guard
            .For()
            .Use(g => g.MaxSize(data, maxSize, "Error", "ERROR_CODE").DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(true, "Error occurred", "ERR_TEST")]
    public void FailIf_ConditionTrue_ShouldReturnError(bool condition, string message, string code)
    {
        // Arrange & Act
        var result = Guard
            .For()
            .Use(g => g.FailIf(condition, message, code, "testField").DoNotThrowOnError());

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal(code, result.Errors[0].Code);
        Assert.Equal(message, result.Errors[0].Description);
    }

    [Theory]
    [InlineData(false, "Error occurred", "ERR_TEST")]
    public void FailIf_ConditionFalse_ShouldNotReturnError(
        bool condition,
        string message,
        string code
    )
    {
        // Arrange & Act
        var result = Guard
            .For()
            .Use(g => g.FailIf(condition, message, code, "testField").DoNotThrowOnError());

        // Assert
        Assert.Empty(result.Errors);
    }
}
