using Newtonsoft.Json.Linq;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Common.Utils.Errors;

namespace VideoChatApp.Infrastructure.UtillityFactories;
public class KeycloakServiceErrorHandler : IKeycloakServiceErrorHandler
{
    public async Task<Result> HandleNotFoundErrorAsync(HttpResponseMessage response, string identifier, 
        string? clientId = null)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        var errorObj = JObject.Parse(errorContent);
        var errorMessage = errorObj["error"]?.ToString();

        return errorMessage switch
        {
            "User not found" => Result.Fail(UserErrorFactory.UserNotFoundById(identifier)),
            "Client not found" => Result.Fail(ErrorFactory.ClientNotFound()),
            _ => Result.Fail(ErrorFactory.Failure($"Resource not found: {errorMessage}"))
        };
    }

}