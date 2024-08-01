using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using VideoChatApp.Api.Utils;
using VideoChatApp.Domain.Exceptions;
using VideoChatApp.Infrastructure.Logging;

namespace VideoChatApp.Api.Middleware;

public class ExceptionHandler : IExceptionHandler
{
    private readonly LoggerHelper<ExceptionHandler> _logger;
    public ExceptionHandler(LoggerHelper<ExceptionHandler> logger)
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
                "Something went wrong with your request. Sorry."
                ),
            ArgumentNullException argumentNullException => CreateProblemDetails(
                context,
                argumentNullException,
                HttpStatusCode.NotFound,
                "An argument null exception occurred"
            ),
            _ => CreateProblemDetails(
                context,
                exception,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred"
            )
        };

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, HttpStatusCode statusCode, string title)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = exception.GetType().Name,
            Title = title,
            Detail = exception.Message,
            Instance = $"{context.Request.Method} {context.Request.Path}"
        };

        return problemDetails;
    }
}
