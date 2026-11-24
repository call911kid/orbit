using Microsoft.AspNetCore.Components;
using Orbit.Console.Models;
using Orbit.Console.Services;

namespace Orbit.Console.Components.ApiKeys;

public partial class ApiKeysManager : ComponentBase, IDisposable
{
    [Inject] private ConsoleState State { get; set; } = default!;
    [Inject] private ClipboardService Clipboard { get; set; } = default!;

    private bool isLoading;
    private bool isCreating;
    private string? loadError;
    private CreatedApiKeyModel? newlyCreatedKey;

    protected override async Task OnInitializedAsync()
    {
        State.OnChange += HandleStateChanged;
        await LoadApiKeysAsync();
    }

    private async Task LoadApiKeysAsync()
    {
        isLoading = true;
        loadError = null;

        try
        {
            await State.RefreshApiKeysAsync();
        }
        catch (Exception ex)
        {
            loadError = $"Failed to load API keys: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleCreateKeyAsync(CreateApiKeyFormSubmit submit)
    {
        if (isCreating) return;
        isCreating = true;

        try
        {
            var created = await State.CreateApiKeyAsync(submit.Name, submit.Role, submit.ValidForMinutes);
            if (created is not null)
            {
                newlyCreatedKey = created;
            }
        }
        catch (Exception ex)
        {
            State.ShowToast($"Failed to generate API key: {ex.Message}");
        }
        finally
        {
            isCreating = false;
        }
    }

    private async Task HandleRevokeKeyAsync(Guid id)
    {
        try
        {
            await State.RevokeApiKeyAsync(id);
        }
        catch (Exception ex)
        {
            State.ShowToast($"Failed to revoke API key: {ex.Message}");
        }
    }

    private Task HandleCopyRawKeyAsync(string rawKey) => CopyWithToastAsync(rawKey, "API key");

    private Task HandleCopySnippetAsync(string snippet) => CopyWithToastAsync(snippet, "Snippet");

    private async Task CopyWithToastAsync(string text, string subject)
    {
        var copied = await Clipboard.CopyTextAsync(text);
        State.ShowToast(copied ? $"{subject} copied to clipboard." : "Unable to access clipboard; copy manually.");
    }

    private void HandleCloseRevealDialog()
    {
        newlyCreatedKey = null;
    }

    private void HandleStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.OnChange -= HandleStateChanged;
    }
}
