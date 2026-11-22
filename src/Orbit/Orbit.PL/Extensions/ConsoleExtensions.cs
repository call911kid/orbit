using Orbit.Console.Components;
using Orbit.PL.Configuration;
using Microsoft.Extensions.Options;

namespace Orbit.PL.Extensions;

public static class ConsoleExtensions
{
    public static WebApplication MapOrbitConsole(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ConsoleOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }

        app.MapStaticAssets();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
