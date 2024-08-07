using VideoChatApp.Application.Common.Result;

namespace VideoChatApp.Application.Contracts.UtillityFactories;

public interface IKeycloakServiceErrorHandler
{
    Task<Result> HandleNotFoundErrorAsync(HttpResponseMessage response, string identifier, string? clientId = null);
}
