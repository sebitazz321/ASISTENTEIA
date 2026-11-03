using System.Text.Json.Serialization;

namespace AsistenteIA.Models;

// ---------- Petición (POST .../models/{modelo}:generateContent) ----------

public class ChatRequest
{
    [JsonPropertyName("systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ChatContent? SystemInstruction { get; set; }

    [JsonPropertyName("contents")]
    public List<ChatContent> Contents { get; set; } = [];

    [JsonPropertyName("generationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GenerationConfig? GenerationConfig { get; set; }
}

/// <summary>Límites de la respuesta (uso responsable: evita respuestas desmedidas y gasto de cuota).</summary>
public class GenerationConfig
{
    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; }
}

public class ChatContent
{
    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; }

    [JsonPropertyName("parts")]
    public List<ChatPart> Parts { get; set; } = [];
}

public class ChatPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

// ---------- Respuesta ----------

public class ChatResponse
{
    [JsonPropertyName("candidates")]
    public List<ChatCandidate> Candidates { get; set; } = [];

    [JsonPropertyName("promptFeedback")]
    public PromptFeedback? PromptFeedback { get; set; }
}

public class ChatCandidate
{
    [JsonPropertyName("content")]
    public ChatContent? Content { get; set; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; set; }
}

public class PromptFeedback
{
    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; set; }
}

// ---------- Error devuelto por la API ----------

public class ApiErrorResponse
{
    [JsonPropertyName("error")]
    public ApiErrorDetail? Error { get; set; }
}

public class ApiErrorDetail
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

// =====================================================================
// Formato compatible con OpenAI (usado por Pollinations: POST /v1/chat/completions)
// =====================================================================

public class OpenAiChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<OpenAiMessage> Messages { get; set; } = [];

    [JsonPropertyName("max_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokens { get; set; }
}

public class OpenAiMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class OpenAiChatResponse
{
    [JsonPropertyName("choices")]
    public List<OpenAiChoice> Choices { get; set; } = [];
}

public class OpenAiChoice
{
    [JsonPropertyName("message")]
    public OpenAiMessage? Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}
