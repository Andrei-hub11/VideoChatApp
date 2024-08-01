using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using VideoChatApp.Infrastructure.Logging;
using VideoChatApp.Infrastructure.Data;
using VideoChatApp.Infrastructure.Security;
using VideoChatApp.Infrastructure.Extensions;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Infrastructure.Persistence;
using Serilog.Filters;
using VideoChatApp.Application.Contracts.Data;

namespace VideoChatApp.Infrastructure;

public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPersistence()
            .AddSerilog(configuration)
            .AddHttpClient()
            .AddKeycloakAuthentication(configuration)
            .AddKeycloakPolicy();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, 
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"))
            .CreateLogger();

        services.AddSingleton(typeof(LoggerHelper<>));

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;
    }

    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));

        // Register KeycloakTokenValidationConfiguration
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, KeycloakTokenValidationConfiguration>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        return services;
    }

    public static void AddKeycloakPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.RequireAssertion(context => context.User.HasRole("Admin")));
        });
    }
}
