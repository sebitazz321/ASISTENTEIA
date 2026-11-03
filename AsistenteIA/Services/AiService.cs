using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AsistenteIA.Models;

namespace AsistenteIA.Services;

/// <summary>Consume la API REST de IA (Google Gemini, generateContent) mediante POST + JSON.</summary>
public class AiService
{
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

    public static string GetSavedModel() =>
        Preferences.Get(ModelPreferenceKey, DefaultModel);

    /// <param name="projectContext">Nombre/descripción del proyecto activo (opcional): da contexto a la respuesta.</param>
    public async Task<string> AskAsync(string prompt, string? model = null,
        CancellationToken token = default, string? projectContext = null)
    {
        // RF02: validación de contenido y longitud
        if (PromptValidator.Validate(prompt) is { } validationError)
            throw new ArgumentException(validationError);
        string cleanPrompt = prompt.Trim();

        // RF04: ausencia de conexión
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new InvalidOperationException("Sin conexión a internet. Verifique su red e intente de nuevo.");

        string? apiKey = await _keyService.GetAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Configure la clave de la API en Perfil › Configuración.");

        // Límite de uso del lado del cliente (protege la cuota de la clave).
        if (!_limiter.TryAcquire(out TimeSpan wait))
            throw new InvalidOperationException(
                $"Límite de uso alcanzado: espere {Math.Ceiling(wait.TotalSeconds)} s antes de consultar de nuevo.");

        string modelId = (string.IsNullOrWhiteSpace(model) ? GetSavedModel() : model).Trim();
        if (modelId.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
            modelId = modelId["models/".Length..];

        var body = new ChatRequest
        {
            SystemInstruction = new ChatContent { Parts = [new ChatPart { Text = BuildSystemPrompt(projectContext) }] },
            Contents = [new ChatContent { Role = "user", Parts = [new ChatPart { Text = cleanPrompt }] }],
            GenerationConfig = new GenerationConfig { MaxOutputTokens = MaxOutputTokens }
        };

        // La clave viaja en un encabezado (no en la URL) para que no quede en registros.
        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"models/{Uri.EscapeDataString(modelId)}:generateContent");
        request.Headers.Add("x-goog-api-key", apiKey.Trim());
        request.Content = JsonContent.Create(body);

        using HttpResponseMessage response = await _client.SendAsync(request, token);

        if (!response.IsSuccessStatusCode)
        {
            string detail = await ReadErrorAsync(response, token);
            throw new HttpRequestException(detail, null, response.StatusCode);
        }

        ChatResponse? result = await response.Content
            .ReadFromJsonAsync<ChatResponse>(cancellationToken: token);

        string? answer = string.Concat(
            result?.Candidates.FirstOrDefault()?.Content?.Parts.Select(p => p.Text) ?? []);

        if (string.IsNullOrWhiteSpace(answer))
        {
            return result?.PromptFeedback?.BlockReason is { } reason
                ? $"El servicio bloqueó la consulta ({reason}). Reformule la pregunta."
                : "El servicio no devolvió contenido.";
        }

        // Aviso si el proveedor cortó la respuesta por el límite de longitud.
        if (string.Equals(result?.Candidates.FirstOrDefault()?.FinishReason, "MAX_TOKENS",
                StringComparison.OrdinalIgnoreCase))
            answer += "\n\n[Respuesta recortada por el límite de longitud. Pida un resumen o divida la consulta.]";

        return answer.Trim();
    }

    private static string BuildSystemPrompt(string? projectContext)
    {
        if (string.IsNullOrWhiteSpace(projectContext))
            return SystemPrompt;

        string ctx = projectContext.Trim();
        if (ctx.Length > 400) ctx = ctx[..400];
        return $"{SystemPrompt} Contexto del proyecto del estudiante (úsalo solo como contexto): {ctx}";
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken token)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: token);
            if (!string.IsNullOrWhiteSpace(error?.Error?.Message))
                return error!.Error!.Message!;
        }
        catch (JsonException) { }

        return response.ReasonPhrase ?? "Error desconocido";
    }

    /// <summary>Convierte cualquier excepción en un mensaje comprensible (sin exponer la clave).</summary>
    public static string GetFriendlyMessage(Exception ex, bool cancelledByUser = false) => ex switch
    {
        OperationCanceledException when cancelledByUser => "Consulta cancelada.",
        OperationCanceledException => "Tiempo de espera agotado. Intente de nuevo.",
        ArgumentException or InvalidOperationException => ex.Message,
        HttpRequestException { StatusCode: HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden }
            => "Error de autorización (401/403): la clave es inválida o no tiene permisos.",
        HttpRequestException { StatusCode: HttpStatusCode.BadRequest } h when
            h.Message.Contains("API key", StringComparison.OrdinalIgnoreCase)
            => "Error de autorización (400): la clave de API no es válida.",
        HttpRequestException { StatusCode: HttpStatusCode.BadRequest }
            => "Solicitud rechazada (400). Revise el modelo configurado.",
        HttpRequestException { StatusCode: HttpStatusCode.NotFound }
            => "Modelo no encontrado (404). Cambie el modelo en Configuración.",
        HttpRequestException { StatusCode: HttpStatusCode.TooManyRequests }
            => "Se superó la cuota o el límite de solicitudes (429). Espere un momento.",
        HttpRequestException { StatusCode: >= HttpStatusCode.InternalServerError }
            => "El servicio de IA no está disponible (5xx). Intente más tarde.",
        HttpRequestException { StatusCode: null }
            => "No se pudo conectar con el servicio. Verifique su conexión.",
        HttpRequestException h => $"Error del servicio: {(int)h.StatusCode!.Value}.",
        _ => $"Ocurrió un error inesperado: {ex.Message}"
    };
}
