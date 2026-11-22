using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Orbit.Console.Services;

namespace Orbit.Console.Components.Pages;

public partial class OrbitConsole : ComponentBase, IDisposable
{
    [Inject] private ConsoleState State { get; set; } = default!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool isCheckingAuth = true;

    protected override void OnInitialized()
    {
        State.OnChange += HandleStateChanged;
        isCheckingAuth = !State.IsUnlocked;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !State.IsUnlocked)
        {
            try
            {
                var result = await ProtectedLocalStorage.GetAsync<string>("orbit_secret_key");
                if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
                {
                    State.SecretKey = result.Value;
                    State.RememberKey = true;
                    var success = await State.UnlockAsync();
                    if (!success)
                    {
                        await ProtectedLocalStorage.DeleteAsync("orbit_secret_key");
                    }
                }
            }
            catch
            {
            }
            finally
            {
                isCheckingAuth = false;
                StateHasChanged();
            }
        }
    }

    private async Task HandleUnlockedAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("destroyOrbitTopography", "loginTopoCanvas");
        }
        catch
        {
        }
    }

    private async Task HandleLockedAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("destroyOrbitTopography", "homeTopoCanvas");
        }
        catch
        {
        }
    }

    private void HandleStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.OnChange -= HandleStateChanged;
    }
}
