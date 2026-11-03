using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AsistenteIA.Models;

namespace AsistenteIA.Services;

/// <summary>
/// Consume la API REST de IA mediante POST + JSON. Soporta dos proveedores que el usuario
/// elige manualmente en Configuración: Google Gemini (generateContent) y Pollinations
/// (formato compatible con OpenAI: chat/completions). Las vistas y ViewModels no cambian
/// al cambiar de proveedor: solo llaman a AskAsync.
/// </summary>
public class AiService
{
    /// <summary>Dirección base del HttpClient (Gemini). Pollinations usa URL absoluta.</summary>
    public const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/";
    public const string DefaultModel = "gemini-3.5-flash-lite";
    public const string ModelPreferenceKey = "SelectedModel";
    public const int MinPromptLength = PromptValidator.MinLength;
    public const int MaxPromptLength = PromptValidator.MaxLength;
    public const int MaxOutputTokens = 4096;

    private const string SystemPrompt =
        "Eres un asistente académico. Responde de forma clara, indica la incertidumbre " +
        "cuando exista y no inventes fuentes ni citas.";

    private readonly HttpClient _client;
    private readonly ApiKeyService _keyService;
    private readonly RequestLimiter _limiter;

    public AiService(HttpClient client, ApiKeyService keyService, RequestLimiter limiter)
    {
        _client = client;
        _keyService = keyService;
        _limiter = limiter;
    }

    // ------------------------------------------------------------ selección de proveedor

    /// <summary>Proveedor elegido por el usuario (por defecto Gemini).</summary>
    public static AiProvider GetSelectedProvider() =>
        AiProviderCatalog.Parse(Preferences.Get(AiProviderCatalog.ProviderPreferenceKey, nameof(AiProvider.Gemini)));

    public static void SetSelectedProvider(AiProvider provider) =>
        Preferences.Set(AiProviderCatalog.ProviderPreferenceKey, provider.ToString());

    public static AiProviderInfo CurrentProviderInfo => AiProviderCatalog.Get(GetSelectedProvider());

    /// <summary>Modelo guardado del proveedor indicado (o del actual).</summary>
    public static string GetSavedModel(AiProvider? provider = null)
    {
        AiProviderInfo info = AiProviderCatalog.Get(provider ?? GetSelectedProvider());
        return Preferences.Get(info.ModelPreferenceKey, info.DefaultModel);
    }

    public static void SaveModel(AiProvider provider, string model)
    {
        AiProviderInfo info = AiProviderCatalog.Get(provider);
        Preferences.Set(info.ModelPreferenceKey, AiProviderCatalog.NormalizeModel(info, model));
    }

    // ------------------------------------------------------------ consulta

    /// <param name="model">Modelo a usar; null = el guardado para el proveedor.</param>
    /// <param name="projectContext">Nombre/descripción del proyecto activo (opcional): da contexto a la respuesta.</param>
    /// <param name="provider">Proveedor a usar; null = el elegido en Configuración.</param>
    public async Task<string> AskAsync(string prompt, string? model = null,
        CancellationToken token = default, string? projectContext = null, AiProvider? provider = null)
    {
        // RF02: validación de contenido y longitud
        if (PromptValidator.Validate(prompt) is { } validationError)
            throw new ArgumentException(validationError);
        string cleanPrompt = prompt.Trim();

        AiProviderInfo info = AiProviderCatalog.Get(provider ?? GetSelectedProvider());

        // RF04: ausencia de conexión
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new InvalidOperationException("Sin conexión a internet. Verifique su red e intente de nuevo.");

        string? apiKey = (await _keyService.GetAsync(info.Id))?.Trim();
        if (info.KeyRequired && string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                $"Configure la clave de {info.DisplayName} en Perfil › Configuración.");

        // Límite de uso del lado del cliente (protege la cuota de la clave).
        if (!_limiter.TryAcquire(out TimeSpan wait))
            throw new InvalidOperationException(
                $"Límite de uso alcanzado: espere {Math.Ceiling(wait.TotalSeconds)} s antes de consultar de nuevo.");

        string modelId = AiProviderCatalog.NormalizeModel(info, model ?? GetSavedModel(info.Id));
        string system = BuildSystemPrompt(projectContext);

        return info.Id switch
        {
            AiProvider.Pollinations => await AskPollinationsAsync(info, modelId, system, cleanPrompt, apiKey, token),
            _ => await AskGeminiAsync(info, modelId, system, cleanPrompt, apiKey!, token)
        };
    }

    // ------------------------------------------------------------ Google Gemini

    private async Task<string> AskGeminiAsync(AiProviderInfo info, string modelId, string system,
        string prompt, string apiKey, CancellationToken token)
    {
        var body = new ChatRequest
        {
            SystemInstruction = new ChatContent { Parts = [new ChatPart { Text = system }] },
            Contents = [new ChatContent { Role = "user", Parts = [new ChatPart { Text = prompt }] }],
            GenerationConfig = new GenerationConfig { MaxOutputTokens = MaxOutputTokens }
        };

        // La clave viaja en un encabezado (no en la URL) para que no quede en registros.
        using var request = new HttpRequestMessage(HttpMethod.Post, AiProviderCatalog.BuildEndpoint(info, modelId));
        request.Headers.Add(info.KeyHeader, apiKey);
        request.Content = JsonContent.Create(body);

        using HttpResponseMessage response = await _client.SendAsync(request, token);
        await EnsureSuccessAsync(response, token);

        ChatResponse? result = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: token);

        string? answer = string.Concat(
            result?.Candidates.FirstOrDefault()?.Content?.Parts.Select(p => p.Text) ?? []);

        if (string.IsNullOrWhiteSpace(answer))
        {
            return result?.PromptFeedback?.BlockReason is { } reason
                ? $"El servicio bloqueó la consulta ({reason}). Reformule la pregunta."
                : "El servicio no devolvió contenido.";
        }

        if (string.Equals(result?.Candidates.FirstOrDefault()?.FinishReason, "MAX_TOKENS",
                StringComparison.OrdinalIgnoreCase))
            answer += TruncatedNotice;

        return answer.Trim();
    }

    // ------------------------------------------------------------ Pollinations (formato OpenAI)

    private async Task<string> AskPollinationsAsync(AiProviderInfo info, string modelId, string system,
        string prompt, string? apiKey, CancellationToken token)
    {
        var body = new OpenAiChatRequest
        {
            Model = modelId,
            Messages =
            [
                new OpenAiMessage { Role = "system", Content = system },
                new OpenAiMessage { Role = "user", Content = prompt }
            ],
            MaxTokens = MaxOutputTokens
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, info.Endpoint);
        if (!string.IsNullOrWhiteSpace(apiKey))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(body);

        using HttpResponseMessage response = await _client.SendAsync(request, token);
        await EnsureSuccessAsync(response, token);

        OpenAiChatResponse? result = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>(cancellationToken: token);
        OpenAiChoice? choice = result?.Choices.FirstOrDefault();
        string? answer = choice?.Message?.Content;

        if (string.IsNullOrWhiteSpace(answer))
            return "El servicio no devolvió contenido.";

        if (string.Equals(choice?.FinishReason, "length", StringComparison.OrdinalIgnoreCase))
            answer += TruncatedNotice;

        return answer.Trim();
    }

    // ------------------------------------------------------------ utilidades

    private const string TruncatedNotice =
        "\n\n[Respuesta recortada por el límite de longitud. Pida un resumen o divida la consulta.]";

    private static string BuildSystemPrompt(string? projectContext)
    {
        if (string.IsNullOrWhiteSpace(projectContext))
            return SystemPrompt;

        string ctx = projectContext.Trim();
        if (ctx.Length > 400) ctx = ctx[..400];
        return $"{SystemPrompt} Contexto del proyecto del estudiante (úsalo solo como contexto): {ctx}";
    }

    /// <summary>Si la respuesta no es 2xx lanza HttpRequestException con el código y el detalle del proveedor.</summary>
    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (response.IsSuccessStatusCode) return;
        string detail = await ReadErrorAsync(response, token);
        throw new HttpRequestException(detail, null, response.StatusCode);
    }

    /// <summary>Lee el mensaje de error del proveedor ({"error":{"message":..}} o {"error":"..."}).</summary>
    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken token)
    {
        string raw = string.Empty;
        try
        {
            raw = await response.Content.ReadAsStringAsync(token);
            if (!string.IsNullOrWhiteSpace(raw))
            {
                using JsonDocument doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("error", out JsonElement error))
                {
                    if (error.ValueKind == JsonValueKind.String)
                        return error.GetString() ?? string.Empty;
                    if (error.ValueKind == JsonValueKind.Object &&
                        error.TryGetProperty("message", out JsonElement msg) &&
                        msg.ValueKind == JsonValueKind.String)
                        return msg.GetString() ?? string.Empty;
                }
            }
        }
        catch (JsonException)
        {
            // Algunos proveedores (p. ej. Pollinations en el error 402) responden texto plano.
            if (!string.IsNullOrWhiteSpace(raw))
                return raw.Length > 200 ? raw[..200] : raw.Trim();
        }

        return response.ReasonPhrase ?? "Error desconocido";
    }

    /// <summary>Convierte cualquier excepción en un mensaje comprensible (sin exponer la clave).</summary>
    public static string GetFriendlyMessage(Exception ex, bool cancelledByUser = false) =>
        AiErrorMapper.GetFriendlyMessage(ex, cancelledByUser, CurrentProviderInfo.DisplayName);
}
