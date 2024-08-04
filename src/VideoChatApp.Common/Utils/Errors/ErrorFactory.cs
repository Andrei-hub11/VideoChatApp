﻿using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.Errors;

public sealed class ErrorFactory
{
    public static Error Failure(string description) => Error.Failure(description, "ERR_FAILURE");

    /// <summary>
    /// Creates an error indicating that the request was invalid or malformed.
    /// </summary>
    /// <param name="description">The detailed error description explaining why the request is considered bad.</param>
    /// <returns>An <see cref="Error"/> instance representing a bad request error with the appropriate code and description.</returns>
    public static Error CreateInvalidRequestError(string description)
    {
        return Error.Failure(description, "ERR_BAD_REQUEST");
    }

    /// <summary>
    /// Creates an unexpected error for an unknown issue.
    /// </summary>
    /// <returns>An <see cref="Error"/> instance representing an unexpected error.</returns>
    public static Error UnknownError()
    {
        return Error.Unexpected("An unknown error has occurred.", "ERR_UNKNOWN");
    }

    /// <summary>
    /// Creates a not found error for a specific resource.
    /// </summary>
    /// <param name="resourceName">The name of the resource that was not found.</param>
    /// <param name="identifier">The identifier of the resource.</param>
    /// <returns>An <see cref="Error"/> instance representing a resource not found error.</returns>
    public static Error ResourceNotFound(string resourceName, string identifier)
    {
        return Error.NotFound($"{resourceName} with identifier '{identifier}' was not found.", "ERR_RESOURCE_NOT_FOUND");
    }

    /// <summary>
    /// Creates a generic not found error.
    /// </summary>
    /// <param name="description">The description of the error.</param>
    /// <returns>An <see cref="Error"/> instance representing a generic not found error.</returns>
    public static Error NotFound(string description)
    {
        return Error.NotFound(description, "ERR_NOT_FOUND");
    }

    /// <summary>
    /// Creates a conflict error for an operation that violates business rules.
    /// </summary>
    /// <param name="ruleDescription">A description of the violated rule.</param>
    /// <returns>An <see cref="Error"/> instance representing a business rule conflict error.</returns>
    public static Error BusinessRuleViolation(string ruleDescription)
    {
        return Error.Conflict(ruleDescription, "ERR_BUSINESS_RULE_VIOLATION");
    }

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="description">The error description.</param>
    /// <param name="code">The error code.</param>
    /// <returns>An <see cref="Error"/> instance representing an unauthorized error.</returns>
    public static Error Unauthorized(string description, string code)
    {
        return Error.Unauthorized(description, code);
    }
}
