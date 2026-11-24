using Orbit.Common.Enums;

namespace Orbit.DAL.Models;

public class ApiKey : BaseEntity
{
    public required string Name { get; set; }
    public required string KeyPrefix { get; set; }
    public required string KeyHash { get; set; }
    public Role Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
