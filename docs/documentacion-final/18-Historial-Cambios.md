# 18. Historial de cambios del proyecto

Construido a partir del historial Git (`git log --stat`).

| Versión | Fecha | Commit | Autor registrado en Git | Cambios | Archivos afectados |
|---|---|---|---|---|---|
| 0.1 | 22/10/2026 | `4fc6329` | Juan Rodriguez | Agrega `.gitattributes` y `.gitignore`. | 2 archivos |
| 0.2 | 22/10/2026 | `5844a08` | Juan Rodriguez | Agrega los archivos del proyecto (plantilla .NET MAUI). | 38 archivos |
| 0.3 | 22/10/2026 | `20b0bb9` | Juan Rodriguez | Base inicial: modelos, servicios, ViewModels y vistas. | 56 archivos |
| 1.0 | 03/11/2026 | `17ab990` | Sebastian Grande Moreno | Versión mejorada: Proyectos, Detalle de proyecto, Perfil, navegación y diálogos inyectables, detector de datos sensibles, límite de uso, notas, 34 pruebas xUnit y documentación `docs/01–07`. Une las ramas `20b0bb9` y `5844a08`. | 63 archivos (34 nuevos, 2 eliminados, 27 modificados) |
| 1.1 | 03/11/2026 | `fb45b32` | Sebastian Grande Moreno | Proveedor Pollinations y cambio manual de proveedor; clave y modelo por proveedor; `AiErrorMapper` con 402 y 408; selector en Configuración; autoría en «Acerca de». | `AiProvider.cs`, `ChatModels.cs`, `AiErrorMapper.cs`, `AiService.cs`, `ApiKeyService.cs`, `ChatViewModel.cs`, `SettingsViewModel.cs`, `SettingsPage.xaml` |
| 1.2 | 03/11/2026 | `76bd256` | Sebastian Grande Moreno | 32 pruebas de proveedores y errores (total 66). | `AiProviderCatalogTests.cs`, `AiErrorMapperTests.cs`, `AsistenteIA.Tests.csproj` |
| 1.3 | 03/11/2026 | `ad5cbe7` y siguientes | Sebastian Grande Moreno | Documentación final: diagramas, diccionario de datos, API REST, proveedores, API Keys, errores, auditoría, manuales, historial, Git y conclusiones; documento consolidado en Word y PDF. | `docs/documentacion-final/**`, `docs/README.md` |

## Responsables por área

| Integrante | Áreas |
|---|---|
| Juan Carlos Díaz Cifuentes | API REST, almacenamiento seguro de la clave, SQLite, pruebas y documentación. |
| Sebastián Grande Moreno | Navegación, pantallas XAML, estilos, integración de la solución, proveedores de IA y repositorio. |
