using Microsoft.AspNetCore.Diagnostics;

using VideoChatApp.Api.Middleware;

namespace VideoChatApp.Api.DependencyInjection;

public static class ServiceCollection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddExceptionHandler<ExceptionHandler>();

        return services;
    }
}
