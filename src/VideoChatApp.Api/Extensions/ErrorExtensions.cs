﻿using Microsoft.AspNetCore.Mvc;

using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Common.Extensions;
using VideoChatApp.Contracts.Models;

namespace VideoChatApp.Api.Extensions;

public static class ErrorExtensions
{
    /// <summary>
    /// Converts a single <see cref="IError"/> instance to a <see cref="ProblemDetails"/> result.
    /// Maps the error type to an appropriate HTTP status code and includes the error description in the response.
    /// </summary>
    /// <param name="errors">The error to convert.</param>
    /// <returns>An <see cref="IActionResult"/> representing the problem details result.</returns>
    public static IActionResult ToProblemDetailsResult(this IReadOnlyList<IError> errors)
    {
        if (errors.Count == 0)
        {
            return new ObjectResult(new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError
            });
        }

        if (errors.All(error => error.ErrorType == ErrorType.Validation))
        {
            return errors.ToValidationProblemDetailsResult();
        }

        return errors[0].ToProblemDetailsResult();
    }

    /// <summary>
    /// Converts a single <see cref="IError"/> instance to a <see cref="ProblemDetails"/> result.
    /// Maps the error type to an appropriate HTTP status code and includes the error description in the response.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>An <see cref="ObjectResult"/> representing the problem details result.</returns>
    private static ObjectResult ToProblemDetailsResult(this IError error)
    {
        var statusCode = error.ErrorType switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Description,
            Instance = Guid.NewGuid().ToString(),
            Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{statusCode}"
        };

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }

    /// <summary>
    /// Converts a list of <see cref="IError"/> instances to a <see cref="ValidationProblemDetails"/> result.
    /// Groups validation errors by their code and includes them in the response.
    /// </summary>
    /// <param name="errors">The list of errors to convert.</param>
    /// <returns>An <see cref="BadRequestObjectResult"/> representing the validation problem details result.</returns>
    private static BadRequestObjectResult ToValidationProblemDetailsResult(this IReadOnlyList<IError> errors)
    {
        Dictionary<string, ValidationErrorDetail[]> validationErrors = errors
            .OfType<ValidationError>()
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key.CapitalizeFirstLetter(),
                group => group.Select(error =>
                new ValidationErrorDetail(error.Code, error.Description)).ToArray());

        //var validationProblemDetails = new ValidationErrorDetails(validationErrors)
        //{
        //    Status = StatusCodes.Status400BadRequest,
        //    Title = "One or more validation errors occurred.",
        //    Detail = "See the errors property for details.",
        //    Instance = Guid.NewGuid().ToString(), // Identificador único
        //    Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
        //};

        var validationProblemDetails = new ValidationErrorDetails(
        Title: "One or more validation errors occurred.",
        Type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
        Status: 400,
        Detail: "See the 'Errors' property for details.",
        Instance: Guid.NewGuid().ToString(),
        Errors: validationErrors
        );

        return new BadRequestObjectResult(validationProblemDetails);
    }
}