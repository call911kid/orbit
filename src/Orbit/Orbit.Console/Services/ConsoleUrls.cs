using Microsoft.AspNetCore.Components;

namespace Orbit.Console.Services;

/// <summary>
/// Centralizes console route constants and URL helpers for navigation components.
/// </summary>
public static class ConsoleUrls
{
    public const string Channels = "/channels";
    public const string NewChannel = "/channels/new";
    public const string Exclusions = "/exclusions";
    public const string Jobs = "/jobs";
    public const string ApiKeys = "/api-keys";

    public static string Channel(Guid id) =>
        id == Guid.Empty ? NewChannel : $"{Channels}/{id}";

    public static string GetNormalizedRelativePath(NavigationManager navigation) =>
        navigation.ToBaseRelativePath(navigation.Uri).Split('?', '#')[0].Trim('/');

    public static bool IsChannelsPath(string normalizedPath) =>
        normalizedPath is "" or "console"
        || normalizedPath.Equals("channels", StringComparison.OrdinalIgnoreCase)
        || normalizedPath.StartsWith("channels/", StringComparison.OrdinalIgnoreCase);
}
