using Orbit.Console.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Orbit.Console.Extensions;

public static class ConsoleServiceExtensions
{
    public static IServiceCollection AddOrbitConsole(this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        services.AddHttpClient();
        services.AddScoped<ProtectedLocalStorage>();
        services.AddScoped<OrbitApiClient>();
        services.AddScoped<ConsoleState>();
        return services;
    }
}
