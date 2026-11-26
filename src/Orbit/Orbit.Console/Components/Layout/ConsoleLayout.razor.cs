using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Orbit.Console.Services;

namespace Orbit.Console.Components.Layout;

public partial class ConsoleLayout : LayoutComponentBase, IDisposable
{
    private const string SecretStorageKey = "orbit_secret_key";

    [Inject] private ConsoleState State { get; set; } = default!;
    [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ILogger<ConsoleLayout> Logger { get; set; } = default!;

    private bool isCheckingAuth = true;

    protected override void OnInitialized()
    {
        State.OnChange += HandleStateChanged;
        isCheckingAuth = !State.IsUnlocked;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || State.IsUnlocked)
        {
            return;
        }

        try
        {
            var result = await ProtectedLocalStorage.GetAsync<string>(SecretStorageKey);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
            {
                State.SecretKey = result.Value;
                State.RememberKey = true;

                if (!await State.UnlockAsync())
                {
                    await ProtectedLocalStorage.DeleteAsync(SecretStorageKey);
                }
            }
        }
        catch (CryptographicException ex)
        {
            Logger.LogWarning(ex, "Stored console secret could not be decrypted; clearing saved secret.");
            await ClearStoredSecretAsync();
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            Logger.LogDebug(ex, "Skipped stored secret lookup because browser storage is unavailable.");
        }
        finally
        {
            isCheckingAuth = false;
            StateHasChanged();
        }
    }

    private Task HandleUnlockedAsync() => DestroyTopographyCanvasAsync("loginTopoCanvas");

    private Task HandleLockedAsync() => DestroyTopographyCanvasAsync("homeTopoCanvas");

    private async Task DestroyTopographyCanvasAsync(string canvasId)
    {
        try
        {
            await JS.InvokeVoidAsync("destroyOrbitTopography", canvasId);
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or InvalidOperationException or TaskCanceledException)
        {
            Logger.LogDebug(ex, "Skipping canvas teardown for {CanvasId}.", canvasId);
        }
    }

    private async Task ClearStoredSecretAsync()
    {
        try
        {
            await ProtectedLocalStorage.DeleteAsync(SecretStorageKey);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            Logger.LogDebug(ex, "Unable to clear stored console secret.");
        }
    }

    private void HandleStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.OnChange -= HandleStateChanged;
    }
}
