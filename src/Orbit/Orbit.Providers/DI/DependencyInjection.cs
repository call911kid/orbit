using Orbit.Providers.Contracts;
using Orbit.Providers.LinkedIn;
using Microsoft.Extensions.DependencyInjection;

namespace Orbit.Providers.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddLinkedInProvider(this IServiceCollection services)
    {
        services.AddHttpClient<IPostingProvider, LinkedInProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/154.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd(
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        });

        return services;
    }
}
