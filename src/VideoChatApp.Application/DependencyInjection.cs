using Microsoft.Extensions.DependencyInjection;

using NerdCritica.Application.Services.ImageServiceConfiguration;

using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Contracts.TokenJWT;
using VideoChatApp.Application.Services.Account;
using VideoChatApp.Application.Services.Images;
using VideoChatApp.Application.Services.Keycloak;
using VideoChatApp.Application.Services.RoomService;
using VideoChatApp.Application.Services.TokenJWT;

namespace VideoChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IKeycloakService, KeycloakService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IImagesService, ImageService>();
        services.AddScoped<IRoomService, RoomService>();

        services.AddSingleton<IImageServiceConfiguration>(new ImageServiceConfiguration(AppDomain.CurrentDomain.BaseDirectory));
        return services;
    }
}
