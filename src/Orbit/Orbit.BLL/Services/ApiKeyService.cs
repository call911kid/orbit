using System;
using System.Security.Cryptography;
using System.Text;
using Orbit.BLL.Interfaces;

namespace Orbit.BLL.Services;

public class ApiKeyService : IApiKeyService
{
    private const string Prefix = "orbit_";
    private const int KeyLengthBytes = 32;

    public (string RawKey, string KeyPrefix, string KeyHash) GenerateApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(KeyLengthBytes);

        var base64 = Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        var rawKey = $"{Prefix}{base64}";
        var hash = HashApiKey(rawKey);
        var keyPrefix = rawKey[..Math.Min(16, rawKey.Length)];

        return (rawKey, keyPrefix, hash);
    }

    public string HashApiKey(string rawKey)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToHexStringLower(hashBytes);
    }
}
