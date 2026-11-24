using System;
using Orbit.Common.Enums;

namespace Orbit.PL.Contracts.ApiKeys;

public sealed record ApiKeyResponse(
    Guid Id,
    string Name,
    string KeyPrefix,
    Role Role,
    DateTime ExpiresAt,
    DateTime? LastUsedAt,
    DateTime CreatedAt
);
