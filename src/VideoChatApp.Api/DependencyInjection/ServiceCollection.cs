using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using NerdCritica.Application.Services.ImageServiceConfiguration;

using VideoChatApp.Api.Middleware;
using VideoChatApp.Application.Contracts.Services;

namespace VideoChatApp.Api.DependencyInjection;

public static class ServiceCollection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? throw new ArgumentNullException("AllowedOrigins",
            "'AllowedOrigins' cannot be null");

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
            builder => builder.WithOrigins(allowedOrigins)
                              .WithMethods("GET", "POST", "PUT", "DELETE") 
                              .AllowAnyHeader()
                              .AllowCredentials());
        });

        services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<ResultFilter>();

        services.AddProblemDetails();
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddSignalR();

        services.AddSingleton<IImageServiceConfiguration>(new ImageServiceConfiguration(AppDomain.CurrentDomain.BaseDirectory));

        return services;
    }
}
