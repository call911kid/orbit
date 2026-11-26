using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Orbit.Console.Services;

namespace Orbit.Console.Components.Layout;

public partial class ConsoleHeader : ComponentBase, IDisposable
{
    private sealed record NavTab(string Href, string Title, Func<ConsoleState, int> GetCount, Func<string, bool> Matches);

    private static readonly NavTab[] NavTabs =
    [
        new(ConsoleUrls.Channels, "Channels", s => s.Profiles.Count, ConsoleUrls.IsChannelsPath),
        new(ConsoleUrls.Exclusions, "Excluded", s => s.ExcludedCompanies.Count, p => p.Equals("exclusions", StringComparison.OrdinalIgnoreCase)),
        new(ConsoleUrls.Jobs, "Jobs", s => s.JobStats.TotalCount, p => p.Equals("jobs", StringComparison.OrdinalIgnoreCase)),
        new(ConsoleUrls.ApiKeys, "API Keys", s => s.ApiKeys.Count, p => p.Equals("api-keys", StringComparison.OrdinalIgnoreCase))
    ];

    [Inject] private ConsoleState State { get; set; } = default!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ILogger<ConsoleHeader> Logger { get; set; } = default!;

    [Parameter] public EventCallback OnLocked { get; set; }

    private string CurrentPath => ConsoleUrls.GetNormalizedRelativePath(Navigation);

    protected override void OnInitialized()
    {
        State.OnChange += HandleStateChanged;
        Navigation.LocationChanged += HandleLocationChanged;
    }

    private async Task HandleLockAsync()
    {
        try
        {
            await ProtectedLocalStorage.DeleteAsync("orbit_secret_key");
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            Logger.LogDebug(ex, "Skipping secret cleanup during disconnected or non-interactive lock.");
        }

        State.Lock();
        await OnLocked.InvokeAsync();
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(StateHasChanged);

    private void HandleStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.OnChange -= HandleStateChanged;
        Navigation.LocationChanged -= HandleLocationChanged;
    }
}
