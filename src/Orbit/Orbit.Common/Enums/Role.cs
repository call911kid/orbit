using System.Text.Json.Serialization;

namespace Orbit.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Admin = 1,
    Guest = 2,
    Agent = 3
}

public static class Roles
{
    public const string Admin = nameof(Role.Admin);
    public const string Guest = nameof(Role.Guest);
    public const string Agent = nameof(Role.Agent);
}
