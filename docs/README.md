# AsistenteIA – Documentación del proyecto final

**Asignatura:** Programación Avanzada y Aplicada · VI semestre · Tecnología de Desarrollo de Software
**Integrantes:** _(completar con nombre completo de cada integrante)_

Aplicación .NET MAUI (Windows / Android) que consulta un modelo de IA mediante una API REST, guarda el
historial en SQLite, organiza el trabajo en proyectos y aplica criterios de seguridad, privacidad y uso responsable.

## Índice

| Documento | Evidencia final que cubre |
|---|---|
| [01-Requisitos-Arquitectura.md](01-Requisitos-Arquitectura.md) | Documento de requisitos, arquitectura y decisiones técnicas |
| [02-Consumo-REST.md](02-Consumo-REST.md) | Consumo REST documentado con solicitud, respuesta y códigos HTTP |
| [03-Plan-de-pruebas.md](03-Plan-de-pruebas.md) | Tabla de diez casos de prueba diligenciada |
| [04-Matriz-de-riesgos.md](04-Matriz-de-riesgos.md) | Matriz de riesgos de privacidad, seguridad y uso responsable |
| [05-Manual-de-usuario.md](05-Manual-de-usuario.md) | Manual de usuario |
| [06-Manual-tecnico.md](06-Manual-tecnico.md) | Manual técnico breve |
| [07-Sustentacion-y-entrega.md](07-Sustentacion-y-entrega.md) | Lista de entrega, guion del video, preguntas de sustentación, declaración de uso de IA |

## Cumplimiento de la rúbrica (50 puntos)

| Criterio | Pts | Dónde se cumple |
|---|---|---|
| Análisis, requisitos y diseño | 6 | `01-Requisitos-Arquitectura.md`: problema delimitado, 12 historias de usuario con criterios de aceptación, arquitectura y decisiones argumentadas |
| Interfaz, navegación y accesibilidad | 8 | 8 pantallas (`Views/`), tema gris con contraste ≥ 4.5:1, `SemanticProperties`, objetivos táctiles ≥ 46, ancho máximo adaptable, `ScrollView` en formularios |
| Arquitectura MVVM e inyección | 8 | `ViewModels/` (CommunityToolkit.Mvvm: `[ObservableProperty]`, `[RelayCommand]`), bindings, `INavigationService`/`IDialogService` inyectables, todo registrado en `MauiProgram.cs` |
| Consumo de API REST de IA | 10 | `Services/AiService.cs`: POST + JSON, clave en encabezado, `async/await`, `CancellationToken`, errores por código HTTP, límite de tokens |
| SQLite e historial CRUD | 6 | `Services/DatabaseService.cs`: crear (guardar consulta/proyecto), consultar (historial, búsqueda, filtros), actualizar (verificar, nota, editar proyecto), eliminar (con confirmación) |
| Seguridad y uso responsable | 5 | `SecureStorage`, detector de datos sensibles, límite de uso, aviso de uso responsable, verificación manual, borrado de datos, sin copias de seguridad en Android |
| Pruebas y calidad | 4 | `AsistenteIA.Tests/` (xUnit) + `03-Plan-de-pruebas.md` |
| Repositorio, evidencias y sustentación | 3 | `07-Sustentacion-y-entrega.md` |

## Lo que el equipo debe completar (no se puede hacer desde el código)

1. Nombres de los integrantes (este archivo, `SettingsPage.xaml` → «Autoría», `07-…`).
2. Ejecutar los diez casos de `03-Plan-de-pruebas.md` y anotar el resultado real con su captura.
3. Capturas de las pantallas en el dispositivo elegido (lista en `07-…`).
4. Commits de todos los integrantes (flujo sugerido en `07-…`).
5. Video de funcionamiento y sustentación individual.
