using System.Net;

using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Infrastructure.UtillityFactories;

public class ErrorMapper : IErrorMapper
{
    public Error MapHttpErrorToAppError(HttpStatusCode statusCode, string errorContent)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => ErrorFactory.CreateInvalidRequestError(errorContent),
            HttpStatusCode.Conflict => ErrorFactory.BusinessRuleViolation(errorContent),
            _ => ErrorFactory.UnknownError(),
        };
    }
}

