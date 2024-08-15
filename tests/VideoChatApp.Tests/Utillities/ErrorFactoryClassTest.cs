using VideoChatApp.Common.Utils.Errors;

namespace VideoChatApp.Tests.Utillities;

public class ErrorFactoryTests
{
    [Fact]
    public void Failure_ShouldReturnErrorWithFailureCode()
    {
        // Arrange
        var description = "Failure occurred";

        // Act
        var error = ErrorFactory.Failure(description);

        // Assert
        Assert.Equal("ERR_FAILURE", error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void CreateInvalidRequestError_ShouldReturnBadRequestError()
    {
        // Arrange
        var description = "Invalid request";

        // Act
        var error = ErrorFactory.CreateInvalidRequestError(description);

        // Assert
        Assert.Equal("ERR_BAD_REQUEST", error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void CreateValidationErrorRequest_ShouldReturnValidationError()
    {
        // Arrange
        var description = "Token not provider";

        // Act
        var error = ErrorFactory.CreateValidationError(description, "Token");

        // Assert
        Assert.Equal("ERR_VALIDATION_FAILURE", error.Code);
        Assert.Equal("Token", error.Field);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void CreateClientNotFound_ShouldReturnNotFoundError()
    {
        // Act
        var error = ErrorFactory.ClientNotFound();

        // Assert
        Assert.Equal("ERR_CLIENT_NOT_FOUND", error.Code);
        Assert.Equal("O keycloak client was not found.", error.Description);
    }

    [Fact]
    public void UnknownError_ShouldReturnUnexpectedError()
    {
        // Act
        var error = ErrorFactory.UnknownError();

        // Assert
        Assert.Equal("ERR_UNKNOWN", error.Code);
        Assert.Equal("An unknown error has occurred.", error.Description);
    }

    [Fact]
    public void ResourceNotFound_ShouldReturnResourceNotFoundError()
    {
        // Arrange
        var resourceName = "Resource";
        var identifier = "456";

        // Act
        var error = ErrorFactory.ResourceNotFound(resourceName, identifier);

        // Assert
        Assert.Equal("ERR_RESOURCE_NOT_FOUND", error.Code);
        Assert.Equal($"{resourceName} with identifier '{identifier}' was not found.", error.Description);
    }

    [Fact]
    public void NotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var description = "Item not found";

        // Act
        var error = ErrorFactory.NotFound(description);

        // Assert
        Assert.Equal("ERR_NOT_FOUND", error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void BusinessRuleViolation_ShouldReturnConflictError()
    {
        // Arrange
        var ruleDescription = "Business rule violation";

        // Act
        var error = ErrorFactory.BusinessRuleViolation(ruleDescription);

        // Assert
        Assert.Equal("ERR_BUSINESS_RULE_VIOLATION", error.Code);
        Assert.Equal(ruleDescription, error.Description);
    }

    [Fact]
    public void Unauthorized_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var description = "Unauthorized access";
        var code = "ERR_UNAUTHORIZED";

        // Act
        var error = ErrorFactory.Unauthorized(description, code);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }
}
