﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;
using VideoChatApp.Application.Contracts.Contexts;
using VideoChatApp.Application.Contracts.Data;
using VideoChatApp.Application.Contracts.Email;
using VideoChatApp.Application.Contracts.Logging;
using VideoChatApp.Application.Contracts.Repositories;
using VideoChatApp.Application.Contracts.UtillityFactories;
using VideoChatApp.Infrastructure.Contexts;
using VideoChatApp.Infrastructure.Data;
using VideoChatApp.Infrastructure.Email;
using VideoChatApp.Infrastructure.Extensions;
using VideoChatApp.Infrastructure.Logging;
using VideoChatApp.Infrastructure.Persistence;
using VideoChatApp.Infrastructure.Security;
using VideoChatApp.Infrastructure.UtillityFactories;

namespace VideoChatApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHttpContextAccessor()
            .AddPersistence()
            .AddFluentEmail(configuration)
            .AddSerilog(configuration)
            .AddHttpClient()
            .AddKeycloakAuthentication(configuration)
            .AddKeycloakPolicy()
            .AddFactories();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IUserContext, UserContexts>();

        return services;
    }

    public static IServiceCollection AddFluentEmail(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var emailSettings = configuration.GetSection("Smtp");

        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        var host = emailSettings["Host"];
        var port = emailSettings.GetValue<int>("Port");
        var userName = emailSettings["UserName"];
        var password = emailSettings["Password"];

        services
            .AddFluentEmail(defaultFromEmail)
            .AddRazorRenderer()
            .AddSmtpSender(host, port, userName, password);

        return services;
    }

    private static IServiceCollection AddSerilog(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Filter.ByExcluding(
                Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware")
            )
            .CreateLogger();

        services.AddSingleton(typeof(ILoggerHelper<>), typeof(LoggerHelper<>));

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;
    }

    private static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));

        // Register KeycloakTokenValidationConfiguration
        services.AddSingleton<
            IConfigureOptions<JwtBearerOptions>,
            KeycloakTokenValidationConfiguration
        >();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        return services;
    }

    private static IServiceCollection AddKeycloakPolicy(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                "Admin",
                policy => policy.RequireAssertion(context => context.User.HasRole("Admin"))
            );

        return services;
    }

    private static IServiceCollection AddFactories(this IServiceCollection services)
    {
        services.AddScoped<IAccountServiceErrorHandler, AccountServiceErrorHandler>();
        services.AddScoped<IKeycloakServiceErrorHandler, KeycloakServiceErrorHandler>();

        return services;
    }
}
