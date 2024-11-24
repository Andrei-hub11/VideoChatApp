using System.Net;

using Newtonsoft.Json.Linq;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Infrastructure.UtillityFactories;

public class KeycloakServiceErrorHandler : IKeycloakServiceErrorHandler
{
    public async Task<Result> ExtractErrorFromResponse(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result.Fail(GetErrorFromResponse(errorContent));
        }

        if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict)
        {
            return Result.Fail(GetErrorFromResponse(errorContent));
        }

        throw new HttpRequestException($"Unexpected error: {response.StatusCode}, {errorContent}");
    }

    private static IError GetErrorFromResponse(string errorContent)
    {
        var errorObj = JObject.Parse(errorContent);
        var errorCode = errorObj["error"]?.ToString();
        var errorMessage = errorObj["errorMessage"]?.ToString();

        var errorMapping = new Dictionary<string, Func<IError>>()
    {
        //{ "User not found", () => UserErrorFactory.UserNotFoundById(identifier) },
        { "invalid_grant", () => UserErrorFactory.InvalidEmailOrPassword() },
        { "User exists with same email", () => UserErrorFactory.EmailAlreadyExists() },
        { "User exists with same username", () => UserErrorFactory.UsernameAlreadyExists() }
    };

        if (!string.IsNullOrWhiteSpace(errorCode) && errorMapping.ContainsKey(errorCode))
        {
            return errorMapping[errorCode]();
        }

        if (!string.IsNullOrWhiteSpace(errorMessage) && errorMapping.ContainsKey(errorMessage))
        {
            return errorMapping[errorMessage]();
        }

        throw new InvalidOperationException($"Unexpected error: {errorContent}");
    }
}

