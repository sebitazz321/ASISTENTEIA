namespace AsistenteIA.Models;

/// <summary>Proveedores de IA soportados. El usuario elige uno manualmente en Configuración.</summary>
public enum AiProvider
{
    Gemini = 0,
    Pollinations = 1
}

/// <summary>
/// Datos fijos de cada proveedor (lógica pura, sin MAUI, para poder probarla).
/// Agregar un proveedor nuevo = una entrada aquí + su método de envío en AiService.
/// </summary>
public sealed class AiProviderInfo
{
    public AiProvider Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    /// <summary>URL absoluta del endpoint. En Gemini «{model}» se reemplaza por el modelo.</summary>
    public string Endpoint { get; init; } = string.Empty;

    public string DefaultModel { get; init; } = string.Empty;
    public IReadOnlyList<string> SuggestedModels { get; init; } = [];

    /// <summary>Encabezado HTTP en el que viaja la clave (nunca en la URL).</summary>
    public string KeyHeader { get; init; } = string.Empty;

    public bool KeyRequired { get; init; } = true;

    /// <summary>Nombre de la entrada en SecureStorage (una clave por proveedor).</summary>
    public string SecureStorageKey { get; init; } = string.Empty;

    /// <summary>Nombre de la preferencia donde se guarda el modelo elegido para este proveedor.</summary>
    public string ModelPreferenceKey { get; init; } = string.Empty;

    public string KeyHelpUrl { get; init; } = string.Empty;

    public override string ToString() => DisplayName;
}

public static class AiProviderCatalog
{
    public const string ProviderPreferenceKey = "SelectedProvider";

    public static AiProviderInfo Gemini { get; } = new()
    {
        Id = AiProvider.Gemini,
        DisplayName = "Google Gemini",
        Description = "API generateContent de Google. Requiere clave creada en Google AI Studio.",
        Endpoint = "https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent",
        DefaultModel = "gemini-3.5-flash-lite",
        SuggestedModels = ["gemini-3.5-flash-lite", "gemini-3.1-flash-lite", "gemini-3.6-flash", "gemini-3.8-flash"],
        KeyHeader = "x-goog-api-key",
        KeyRequired = true,
        SecureStorageKey = "ai_api_key",          // nombre original: conserva las claves ya guardadas
        ModelPreferenceKey = "SelectedModel",     // nombre original: conserva el modelo ya elegido
        KeyHelpUrl = "https://aistudio.google.com/apikey"
    };

    public static AiProviderInfo Pollinations { get; } = new()
    {
        Id = AiProvider.Pollinations,
        DisplayName = "Pollinations",
        Description = "API compatible con OpenAI (chat/completions). Requiere clave creada en enter.pollinations.ai.",
        Endpoint = "https://gen.pollinations.ai/v1/chat/completions",
        DefaultModel = "openai",
        SuggestedModels = ["openai", "openai-fast", "mistral"],   // lista vigente: https://gen.pollinations.ai/models
        KeyHeader = "Authorization",              // Authorization: Bearer <clave>
        KeyRequired = true,
        SecureStorageKey = "pollinations_api_key",
        ModelPreferenceKey = "SelectedModel_Pollinations",
        KeyHelpUrl = "https://enter.pollinations.ai"
    };

    public static IReadOnlyList<AiProviderInfo> All { get; } = [Gemini, Pollinations];

    public static AiProviderInfo Get(AiProvider provider) =>
        All.FirstOrDefault(p => p.Id == provider) ?? Gemini;

    /// <summary>Convierte el valor guardado en preferencias; si no es válido vuelve a Gemini.</summary>
    public static AiProvider Parse(string? value) =>
        Enum.TryParse(value?.Trim(), ignoreCase: true, out AiProvider p) && Enum.IsDefined(p)
            ? p
            : AiProvider.Gemini;

    /// <summary>Limpia el identificador del modelo («models/x» → «x»); vacío → modelo por defecto.</summary>
    public static string NormalizeModel(AiProviderInfo provider, string? model)
    {
        string id = string.IsNullOrWhiteSpace(model) ? provider.DefaultModel : model.Trim();
        if (id.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
            id = id["models/".Length..];
        return id.Length == 0 ? provider.DefaultModel : id;
    }

    /// <summary>URL final del endpoint para el modelo indicado.</summary>
    public static string BuildEndpoint(AiProviderInfo provider, string model) =>
        provider.Endpoint.Replace("{model}", Uri.EscapeDataString(model));
}
