namespace Orbit.Console.Components.ApiKeys;

/// <summary>
/// Shared display formatting helpers for API key expiration and activity timestamps.
/// </summary>
internal static class ApiKeyDisplayHelper
{
    public static string FormatTimeLeft(DateTime expiresAtUtc)
    {
        var diff = DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc) - DateTime.UtcNow;
        if (diff <= TimeSpan.Zero)
        {
            return "Expired";
        }

        if (diff.TotalDays >= 1)
        {
            return $"{(int)diff.TotalDays}d {diff.Hours}h left";
        }

        if (diff.TotalHours >= 1)
        {
            return $"{(int)diff.TotalHours}h {diff.Minutes}m left";
        }

        return $"{Math.Max(1, diff.Minutes)}m left";
    }

    public static string FormatLastUsed(DateTime? lastUsedAtUtc)
    {
        if (!lastUsedAtUtc.HasValue)
        {
            return "Never used";
        }

        var diff = DateTime.UtcNow - DateTime.SpecifyKind(lastUsedAtUtc.Value, DateTimeKind.Utc);
        if (diff.TotalMinutes < 1)
        {
            return "Just now";
        }

        if (diff.TotalHours < 1)
        {
            return $"{(int)diff.TotalMinutes}m ago";
        }

        if (diff.TotalDays < 1)
        {
            return $"{(int)diff.TotalHours}h ago";
        }

        return $"{(int)diff.TotalDays}d ago";
    }
}
