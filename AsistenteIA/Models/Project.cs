using SQLite;

namespace AsistenteIA.Models;

/// <summary>Proyecto académico que agrupa consultas (por ejemplo una materia o un trabajo).</summary>
public class Project
{
    public const int NameMinLength = 3;
    public const int NameMaxLength = 60;
    public const int DescriptionMaxLength = 300;

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Propiedades de presentación (no se guardan en SQLite).
    [Ignore]
    public int ConversationCount { get; set; }

    [Ignore]
    public string CountText => ConversationCount == 1 ? "1 consulta" : $"{ConversationCount} consultas";

    [Ignore]
    public string DescriptionText =>
        string.IsNullOrWhiteSpace(Description) ? "Sin descripción" : Description;

    /// <summary>Opción "sin proyecto" del selector de la pantalla de consulta.</summary>
    public static Project None { get; } = new() { Id = 0, Name = "Sin proyecto" };

    public override string ToString() => Name;
}
