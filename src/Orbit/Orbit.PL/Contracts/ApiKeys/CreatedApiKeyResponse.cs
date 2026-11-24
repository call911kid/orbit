using Orbit.Common.Enums;

namespace Orbit.PL.Contracts.ApiKeys;

public sealed record CreatedApiKeyResponse(
    Guid Id,
    string Name,
    string RawKey,
    string KeyPrefix,
    Role Role,
    DateTime ExpiresAt,
    DateTime CreatedAt
);
