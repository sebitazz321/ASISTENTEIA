namespace AsistenteIA.Services;

/// <summary>Validación de la consulta (RF02). Lógica pura, sin dependencias de MAUI, para poder probarla.</summary>
public static class PromptValidator
{
    public const int MinLength = 5;
    public const int MaxLength = 2000;

    /// <returns>Mensaje de error, o null si la consulta es válida.</returns>
    public static string? Validate(string? prompt)
    {
        string clean = (prompt ?? string.Empty).Trim();

        if (clean.Length == 0)
            return "Escriba una consulta antes de enviar.";
        if (clean.Length < MinLength)
            return $"La consulta es demasiado corta (mínimo {MinLength} caracteres).";
        if (clean.Length > MaxLength)
            return $"La consulta supera {MaxLength} caracteres.";

        return null;
    }
}
