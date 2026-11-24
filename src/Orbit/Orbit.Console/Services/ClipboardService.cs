using Microsoft.JSInterop;

namespace Orbit.Console.Services;

/// <summary>
/// Isolates browser clipboard JS interop from Blazor UI components.
/// </summary>
public sealed class ClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> CopyTextAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        try
        {
            await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
