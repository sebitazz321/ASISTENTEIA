using System.Text.RegularExpressions;

namespace AsistenteIA.Services;

/// <summary>
/// Detecta datos que no deberían enviarse a un proveedor de IA externo (privacidad).
/// Es una heurística: avisa al usuario, no garantiza detectar todo.
/// </summary>
public static class SensitiveDataDetector
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(200);

    private static readonly Regex Email = new(
        @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, MatchTimeout);

    // 13 a 19 dígitos, con espacios o guiones opcionales (se confirma con Luhn).
    private static readonly Regex CardCandidate = new(
        @"\b(?:\d[ \-]?){12,18}\d\b", RegexOptions.CultureInvariant, MatchTimeout);

    // Celular colombiano (3XX XXX XXXX) o número largo contiguo (cédula, teléfono, etc.).
    private static readonly Regex PhoneOrId = new(
        @"\b3\d{2}[ \-]?\d{3}[ \-]?\d{4}\b|\b\d{8,12}\b",
        RegexOptions.CultureInvariant, MatchTimeout);

    private static readonly Regex Credential = new(
        @"\b(contrase[ñn]a|password|passwd|clave\s+de\s+acceso|token|api[ _\-]?key)\b\s*[:=]",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, MatchTimeout);

    private static readonly Regex ApiKeyLike = new(
        @"\bAIza[0-9A-Za-z_\-]{20,}\b|\bsk-[A-Za-z0-9_\-]{20,}\b",
        RegexOptions.CultureInvariant, MatchTimeout);

    /// <returns>Lista (sin repetidos) de tipos de dato sensible encontrados; vacía si no hay.</returns>
    public static IReadOnlyList<string> Detect(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var found = new List<string>();

        try
        {
            if (Email.IsMatch(text))
                found.Add("correo electrónico");

            bool hasCard = CardCandidate.Matches(text)
                .Any(m => PassesLuhn(m.Value));
            if (hasCard)
                found.Add("número de tarjeta");

            if (PhoneOrId.IsMatch(text))
                found.Add("número de teléfono o identificación");

            if (Credential.IsMatch(text))
                found.Add("contraseña o credencial");

            if (ApiKeyLike.IsMatch(text))
                found.Add("clave de API");
        }
        catch (RegexMatchTimeoutException)
        {
            // Si la heurística tarda demasiado, no se bloquea al usuario.
        }

        return found;
    }

    /// <summary>Algoritmo de Luhn para validar números de tarjeta.</summary>
    public static bool PassesLuhn(string number)
    {
        int sum = 0;
        bool doubleIt = false;
        int digits = 0;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            char c = number[i];
            if (c is ' ' or '-') continue;
            if (c < '0' || c > '9') return false;

            int d = c - '0';
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
            digits++;
        }

        return digits is >= 13 and <= 19 && sum % 10 == 0;
    }
}
