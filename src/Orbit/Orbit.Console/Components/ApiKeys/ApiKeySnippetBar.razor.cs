using Microsoft.AspNetCore.Components;

namespace Orbit.Console.Components.ApiKeys;

public partial class ApiKeySnippetBar : ComponentBase, IDisposable
{
    private static readonly (string Key, string Label)[] Modes =
    [
        ("curl", "cURL"),
        ("mcp", "MCP"),
        ("header", "Header")
    ];

    [Parameter] public string BaseUrl { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnCopy { get; set; }

    private string activeMode = "curl";
    private bool justCopied;
    private CancellationTokenSource? copyResetCts;

    private string NormalizedBaseUrl => string.IsNullOrWhiteSpace(BaseUrl)
        ? "https://localhost:7220"
        : BaseUrl.TrimEnd('/');

    private string CurrentSnippet => activeMode switch
    {
        "mcp" => $"{{ \"url\": \"{NormalizedBaseUrl}/mcp\", \"headers\": {{ \"X-Api-Key\": \"<api-key>\" }} }}",
        "header" => "X-Api-Key: <api-key>",
        _ => $"curl -H \"X-Api-Key: <api-key>\" {NormalizedBaseUrl}/api/job-postings"
    };

    private void SelectMode(string mode)
    {
        activeMode = mode;
        justCopied = false;
    }

    private async Task CopyCurrentAsync()
    {
        await OnCopy.InvokeAsync(CurrentSnippet);
        justCopied = true;

        copyResetCts?.Cancel();
        copyResetCts?.Dispose();
        copyResetCts = new CancellationTokenSource();
        var token = copyResetCts.Token;

        try
        {
            await Task.Delay(1600, token);
            if (!token.IsCancellationRequested)
            {
                justCopied = false;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    public void Dispose()
    {
        copyResetCts?.Cancel();
        copyResetCts?.Dispose();
    }
}
