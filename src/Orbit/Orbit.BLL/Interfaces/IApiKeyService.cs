namespace Orbit.BLL.Interfaces;

public interface IApiKeyService
{
    (string RawKey, string KeyPrefix, string KeyHash) GenerateApiKey();
    string HashApiKey(string rawKey);
}
