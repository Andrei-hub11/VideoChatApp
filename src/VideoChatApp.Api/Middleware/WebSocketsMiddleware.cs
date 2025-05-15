namespace VideoChatApp.Api.Middleware;

public class WebSocketsMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var request = httpContext.Request;

        // web sockets cannot pass headers so we must take the access token from query param and
        // add it to the header before authentication middleware runs
        if (
            request.Path.HasValue
            && request.Path.Value.Contains("/videoChatHub")
            && request.Query.TryGetValue("access_token", out var accessToken)
        )
        {
            request.Headers.Append("Authorization", $"Bearer {accessToken}");
        }

        await _next(httpContext);
    }
}
