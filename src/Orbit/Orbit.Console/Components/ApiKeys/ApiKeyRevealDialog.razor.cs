using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Orbit.Console.Models;

namespace Orbit.Console.Components.ApiKeys;

public partial class ApiKeyRevealDialog : ComponentBase
{
    [Parameter] public CreatedApiKeyModel? CreatedKey { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<string> OnCopy { get; set; }

    private Guid? lastKeyId;
    private bool isCopied;
    private bool pulseGuard;
    private string? guardMessage;

    protected override void OnParametersSet()
    {
        if (CreatedKey?.Id != lastKeyId)
        {
            lastKeyId = CreatedKey?.Id;
            isCopied = false;
            pulseGuard = false;
            guardMessage = null;
        }
    }

    private async Task CopyKeyOnlyAsync()
    {
        if (CreatedKey is null) return;

        await OnCopy.InvokeAsync(CreatedKey.RawKey);
        isCopied = true;
        pulseGuard = false;
        guardMessage = null;
    }

    private async Task HandlePrimaryActionAsync()
    {
        if (CreatedKey is null) return;

        if (!isCopied)
        {
            await OnCopy.InvokeAsync(CreatedKey.RawKey);
            isCopied = true;
        }

        await CloseAsync();
    }

    private async Task HandleBackdropClickAsync()
    {
        if (isCopied)
        {
            await CloseAsync();
            return;
        }

        TriggerGuardPulse();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Key != "Escape") return;

        if (isCopied)
        {
            await CloseAsync();
            return;
        }

        TriggerGuardPulse();
    }

    private void TriggerGuardPulse()
    {
        pulseGuard = true;
        guardMessage = "Copy key before closing, or click Close";
    }

    private async Task CloseAsync()
    {
        isCopied = false;
        pulseGuard = false;
        guardMessage = null;
        lastKeyId = null;
        await OnClose.InvokeAsync();
    }
}
