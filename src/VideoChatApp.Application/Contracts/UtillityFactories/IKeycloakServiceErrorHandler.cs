using VideoChatApp.Application.Common.Result;

namespace VideoChatApp.Application.Contracts.UtillityFactories;

public interface IKeycloakServiceErrorHandler
{
    Task<Result> ExtractErrorFromResponse(HttpResponseMessage response);
}
