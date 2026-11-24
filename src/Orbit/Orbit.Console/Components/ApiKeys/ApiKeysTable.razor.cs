using Microsoft.AspNetCore.Components;
using Orbit.Console.Models;

namespace Orbit.Console.Components.ApiKeys;

public partial class ApiKeysTable : ComponentBase
{
    [EditorRequired]
    [Parameter] public IReadOnlyList<ApiKeyItemModel> Keys { get; set; } = [];

    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string? ErrorMessage { get; set; }
    [Parameter] public TimeZoneInfo? UserTimeZone { get; set; }
    [Parameter] public EventCallback OnRetry { get; set; }
    [Parameter] public EventCallback<Guid> OnRevoke { get; set; }

    private Guid? pendingRevokeId;
    private Guid? revokingId;

    private void RequestRevoke(Guid id) => pendingRevokeId = id;

    private void CancelRevoke() => pendingRevokeId = null;

    private async Task ConfirmRevokeAsync(Guid id)
    {
        if (revokingId == id) return;
        revokingId = id;
        pendingRevokeId = null;

        try
        {
            await OnRevoke.InvokeAsync(id);
        }
        finally
        {
            revokingId = null;
        }
    }

    private string FormatDate(DateTime utcTime)
    {
        var utc = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
        var local = UserTimeZone is not null
            ? TimeZoneInfo.ConvertTimeFromUtc(utc, UserTimeZone)
            : utc.ToLocalTime();

        return local.ToString("MMM dd, yyyy");
    }
}
