namespace AsistenteIA.Services;

/// <summary>Guarda la clave en el almacén seguro del dispositivo (nunca en código ni en el repositorio).</summary>
public class ApiKeyService
{
    private const string KeyName = "ai_api_key";

    public Task SaveAsync(string key) =>
        SecureStorage.Default.SetAsync(KeyName, key.Trim());

    public Task<string?> GetAsync() =>
        SecureStorage.Default.GetAsync(KeyName);

    public bool Remove() =>
        SecureStorage.Default.Remove(KeyName);

    public async Task<bool> HasKeyAsync() =>
        !string.IsNullOrWhiteSpace(await GetAsync());
}
