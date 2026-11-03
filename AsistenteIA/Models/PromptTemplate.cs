namespace AsistenteIA.Models;

/// <summary>Plantilla de prompt académico editable (RF07).</summary>
public class PromptTemplate
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = "General";
    public string Text { get; init; } = string.Empty;

    public static IReadOnlyList<PromptTemplate> Defaults { get; } =
    [
        new() { Name = "Pregunta libre", Category = "General", Text = "" },
        new() { Name = "Explicar un concepto", Category = "Explicación",
                Text = "Explica con un lenguaje claro y un ejemplo el concepto de: " },
        new() { Name = "Resumir un texto", Category = "Resumen",
                Text = "Resume en cinco ideas principales el siguiente texto: " },
        new() { Name = "Comparar dos temas", Category = "Comparación",
                Text = "Compara ventajas, desventajas y casos de uso de [tema A] y [tema B]." },
        new() { Name = "Preguntas de práctica", Category = "Estudio",
                Text = "Crea cinco preguntas de práctica con su respuesta sobre: " },
        new() { Name = "Revisar código", Category = "Programación",
                Text = "Revisa el siguiente código C#, explica posibles errores y sugiere mejoras: " },
    ];

    public override string ToString() => Name;
}
