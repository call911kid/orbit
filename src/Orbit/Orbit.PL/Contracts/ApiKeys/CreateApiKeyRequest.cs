using Orbit.Common.Enums;

namespace Orbit.PL.Contracts.ApiKeys;

public record CreateApiKeyRequest(string Name, Role Role, int ValidForMinutes);
