namespace AsistenteIA.Models;

/// <summary>Datos de perfil del estudiante. Se guardan solo en el dispositivo (Preferences).</summary>
public class UserProfile
{
    public const int FieldMaxLength = 60;

    public string Name { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;

    public string Initials
    {
        get
        {
            string[] parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length switch
            {
                0 => "?",
                1 => parts[0][..1].ToUpperInvariant(),
                _ => (parts[0][..1] + parts[^1][..1]).ToUpperInvariant()
            };
        }
    }
}
