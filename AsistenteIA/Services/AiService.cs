using System.Net.Http.Json;
using System.Text.Json;

namespace AsistenteIA.Services;

public class AiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiKeyService _keyService;

    public AiService(HttpClient httpClient, ApiKeyService keyService)
    {
        _httpClient = httpClient;
        _keyService = keyService;
    }

    public async Task<string> AskAsync(string prompt, string model = "gemini-2.5-flash", CancellationToken cancellationToken = default)
    {
        string? apiKey = await _keyService.GetAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("No se ha configurado la clave API de Gemini.");

        // Si el modelo tiene espacios o trae un valor antiguo de OpenAI, asigna gemini-2.5-flash
        model = model.Trim();
        if (model.Contains("openai") || string.IsNullOrWhiteSpace(model))
        {
            model = "gemini-2.5-flash";
        }

        apiKey = apiKey.Trim();

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        string fullUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var response = await _httpClient.PostAsJsonAsync(fullUrl, requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"[{response.StatusCode}] {errorBody}");
        }

        using var doc = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}