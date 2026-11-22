using Orbit.PL.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orbit.PL.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));
        services.Configure<ConsoleOptions>(configuration.GetSection(ConsoleOptions.SectionName));
        services.Configure<ApiAuthenticationOptions>(configuration.GetSection(ApiAuthenticationOptions.SectionName));

        return services;
    }

    public static WebApplicationBuilder AddAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddAppConfiguration(builder.Configuration);
        return builder;
    }
}
