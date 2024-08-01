using Microsoft.Extensions.DependencyInjection;

using VideoChatApp.Application.Contracts.Services;
using VideoChatApp.Application.Services.Account;
using VideoChatApp.Application.Services.Keycloak;

namespace VideoChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IKeycloakService, KeycloakService>();
        return services;
    }
}
