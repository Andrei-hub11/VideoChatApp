using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using VideoChatApp.Api.Utils;
using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Domain.Exceptions;

namespace VideoChatApp.Api.Middleware;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILoggerHelper<ExceptionHandler> _logger;
    public ExceptionHandler(ILoggerHelper<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, 
        CancellationToken cancellationToken)
    {

        _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(exception, httpContext, 400));

        var result = GetProblemDetails(httpContext, exception);

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);

        return true;
    }

    private ProblemDetails GetProblemDetails(HttpContext context, Exception exception) =>
        exception switch
        {
            BadRequestException badRequestException => CreateProblemDetails(
                context, 
                badRequestException, 
                HttpStatusCode.BadRequest,
                "Something went wrong with your request. Sorry.",
                exception.GetType().Name,
                exception.Message
                ),
            _ => CreateProblemDetails(
                context,
                exception,
                HttpStatusCode.InternalServerError,
                "ServerError",
                "An unexpected server error occurred. Please try again later.",
                "An error occurred."
            )
        };

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, HttpStatusCode statusCode,
        string type, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = type,
            Title = title,
            Detail = detail,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        return problemDetails;
    }
}
