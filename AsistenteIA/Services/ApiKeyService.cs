using AsistenteIA.Models;

namespace AsistenteIA.Services;

/// <summary>
/// Guarda las claves en el almacén seguro del dispositivo (nunca en código ni en el repositorio).
/// Cada proveedor tiene su propia entrada en SecureStorage; Gemini conserva el nombre original
/// («ai_api_key») para no perder claves guardadas con versiones anteriores.
/// </summary>
public class ApiKeyService
{
    private static string KeyName(AiProvider provider) =>
        AiProviderCatalog.Get(provider).SecureStorageKey;

    public Task SaveAsync(string key, AiProvider provider = AiProvider.Gemini) =>
        SecureStorage.Default.SetAsync(KeyName(provider), key.Trim());

    public Task<string?> GetAsync(AiProvider provider = AiProvider.Gemini) =>
        SecureStorage.Default.GetAsync(KeyName(provider));

    public bool Remove(AiProvider provider = AiProvider.Gemini) =>
        SecureStorage.Default.Remove(KeyName(provider));

    public async Task<bool> HasKeyAsync(AiProvider provider = AiProvider.Gemini) =>
        !string.IsNullOrWhiteSpace(await GetAsync(provider));
}
