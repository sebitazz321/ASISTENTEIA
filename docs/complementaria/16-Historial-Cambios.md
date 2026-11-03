# 16. Historial de cambios del proyecto

Construido a partir del historial Git (`git log`) y del contenido de cada versión.

| Versión | Fecha | Commit | Autor registrado en Git | Cambios | Archivos afectados |
|---|---|---|---|---|---|
| 0.1 | 22/10/2026 | `4fc6329` | Juan Rodriguez | Agrega `.gitattributes` y `.gitignore`. | 2 archivos |
| 0.2 | 22/10/2026 | `5844a08` | Juan Rodriguez | Agrega los archivos del proyecto (plantilla .NET MAUI). | 38 archivos |
| 0.3 | 22/10/2026 | `20b0bb9` | Juan Rodriguez | «Trabajo completo»: modelos, servicios, ViewModels y vistas iniciales. | 56 archivos |
| 1.0 | 03/11/2026 | `17ab990` | Sebastian Grande Moreno | Integra la versión mejorada entregada en el zip (no estaba versionada): Proyectos, Detalle de proyecto, Perfil, navegación y diálogos inyectables, datos sensibles, límite de uso, notas, 34 pruebas xUnit, docs 01–07. Cierra el *merge* pendiente entre `20b0bb9` y `5844a08`. | 63 archivos (34 nuevos, 2 eliminados, 27 modificados) |
| 1.1 | 03/11/2026 | `fb45b32` | Sebastian Grande Moreno | Proveedor Pollinations y cambio manual; clave por proveedor; `AiErrorMapper` con 402 y 408; selector en Configuración; nombres en «Acerca de». | `AiProvider.cs`, `ChatModels.cs`, `AiErrorMapper.cs`, `AiService.cs`, `ApiKeyService.cs`, `ChatViewModel.cs`, `SettingsViewModel.cs`, `SettingsPage.xaml` |
| 1.2 | 03/11/2026 | `76bd256` | Sebastian Grande Moreno | 32 pruebas nuevas (total 66). | `AiProviderCatalogTests.cs`, `AiErrorMapperTests.cs`, `AsistenteIA.Tests.csproj` |
| 1.3 | 03/11/2026 | (este commit) | Sebastian Grande Moreno | Documentación complementaria (18 entregables), diagramas y comparación con el documento. | `docs/complementaria/**` |

## Responsables por área (según §11 del documento)

| Integrante | Áreas |
|---|---|
| Juan Carlos Díaz Cifuentes | API REST, almacenamiento seguro de la clave, SQLite, pruebas y documentación. |
| Sebastián Grande Moreno | Navegación, pantallas XAML, estilos, integración de la solución y repositorio. |

> Nota: el autor que Git registra en los tres primeros commits es «Juan Rodriguez <juandavidrj037@…>» y el *remote*
> original era `JDRODRIGUEZ037/AsistenteIA`. Si corresponde a la cuenta de un integrante, conviene aclararlo en la
> sustentación; si no, el equipo debe explicar el origen de ese código.
