using FluentValidation;
using Orbit.BLL.Behaviors;
using Orbit.BLL.Interfaces;
using Orbit.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Orbit.BLL.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddHttpClient<ITelegramService, TelegramService>();
        services.AddScoped<IPostingAlertFormatter, TelegramPostingAlertFormatter>();
        services.AddScoped<IPostingMatcher, PostingMatcher>();
        services.AddSingleton<IApiKeyService, ApiKeyService>();

        return services;
    }
}
