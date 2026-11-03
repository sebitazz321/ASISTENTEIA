using System.Net;

namespace AsistenteIA.Services;

/// <summary>
/// Traduce excepciones y códigos HTTP a mensajes comprensibles en español, sin exponer la clave.
/// Lógica pura (sin MAUI) para poder probarla con xUnit.
/// </summary>
public static class AiErrorMapper
{
    public static string GetFriendlyMessage(Exception ex, bool cancelledByUser = false, string provider = "el proveedor") => ex switch
    {
        OperationCanceledException when cancelledByUser => "Consulta cancelada.",
        OperationCanceledException or TimeoutException => "Tiempo de espera agotado. Intente de nuevo.",
        ArgumentException or InvalidOperationException => ex.Message,
        HttpRequestException h => FromStatus(h.StatusCode, h.Message, provider),
        _ => $"Ocurrió un error inesperado: {ex.Message}"
    };

    /// <summary>Mensaje para un código HTTP (null = no hubo respuesta: problema de conexión/DNS/TLS).</summary>
    public static string FromStatus(HttpStatusCode? status, string? detail = null, string provider = "el proveedor")
    {
        if (status is null)
            return "No se pudo conectar con el servicio. Verifique su conexión.";

        int code = (int)status.Value;
        return code switch
        {
            400 when Mentions(detail, "API key", "api_key", "API_KEY")
                => "Error de autorización (400): la clave de API no es válida.",
            400 => "Solicitud rechazada (400). Revise el modelo configurado.",
            401 or 403 => $"Error de autorización ({code}): la clave de {provider} es inválida o no tiene permisos.",
            402 => $"{provider} rechazó la solicitud por saldo o límite gratuito (402). Revise su cuenta o cambie de proveedor.",
            404 => "Modelo no encontrado (404). Cambie el modelo en Configuración.",
            408 => "El servidor tardó demasiado en responder (408). Intente de nuevo.",
            429 => "Se superó la cuota o el límite de solicitudes (429). Espere un momento.",
            >= 500 and <= 599 => $"El servicio de IA no está disponible ({code}). Intente más tarde.",
            _ => $"Error del servicio: {code}."
        };
    }

    private static bool Mentions(string? text, params string[] words) =>
        !string.IsNullOrEmpty(text) && words.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));
}
