using SQLite;

namespace AsistenteIA.Models;

public class ConversationRecord
{
    public const int NoteMaxLength = 500;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Category { get; set; } = "General";
    public bool IsVerified { get; set; }

    /// <summary>Proyecto al que pertenece (0 = sin proyecto).</summary>
    public int ProjectId { get; set; }

    /// <summary>Nota personal del estudiante sobre la respuesta (se puede editar).</summary>
    public string Note { get; set; } = string.Empty;

    // Propiedades de presentación (no se guardan en SQLite).
    [Ignore]
    public string ProjectName { get; set; } = string.Empty;

    [Ignore]
    public string CreatedAtLocal => CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    [Ignore]
    public string VerificationText => IsVerified ? "✔ Verificada" : "Sin verificar";

    [Ignore]
    public string MetaText =>
        $"{CreatedAtLocal} · {Category}" +
        (string.IsNullOrEmpty(ProjectName) ? string.Empty : $" · {ProjectName}") +
        $" · {VerificationText}";
}
