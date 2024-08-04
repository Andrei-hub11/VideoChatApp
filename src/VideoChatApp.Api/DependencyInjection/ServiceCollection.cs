using Microsoft.AspNetCore.Diagnostics;

using NerdCritica.Application.Services.ImageServiceConfiguration;

using VideoChatApp.Api.Middleware;
using VideoChatApp.Application.Contracts.Services;

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

        services.AddSingleton<IImageServiceConfiguration>(new ImageServiceConfiguration(AppDomain.CurrentDomain.BaseDirectory));

        return services;
    }
}
