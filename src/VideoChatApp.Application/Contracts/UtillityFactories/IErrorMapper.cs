using System.Net;

using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Application.Contracts.UtillityFactories;

public interface IErrorMapper
{
    Error MapHttpErrorToAppError(HttpStatusCode statusCode, string errorContent);
}
