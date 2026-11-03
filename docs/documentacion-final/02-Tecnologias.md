# 2. Tecnologías utilizadas

Versiones tomadas del código del proyecto y verificadas en las fuentes oficiales el 3 de noviembre de 2026.

| Componente | Versión / detalle | Uso |
|---|---|---|
| .NET SDK | **9.0.100** (`global.json`, `latestFeature`) · ⚠️ fin de soporte 10/11/2026 → migrar a .NET 10 LTS | Compilación. |
| Visual Studio y .NET MAUI | Workload `maui` de .NET 9 (`$(MauiVersion)`) | Interfaz multiplataforma. |
| Plataformas | `net9.0-windows10.0.19041.0` (mín. 10.0.17763) y `net9.0-android` (mín. API 21) | Destinos. |
| C# y XAML | C# 13 (por defecto en .NET 9): *collection expressions* y expresiones `switch` con patrones | Lógica e interfaz. |
| MVVM — CommunityToolkit.Mvvm | **8.3.2** | `[ObservableProperty]`, `[RelayCommand]`. |
| Shell | TabBar de 5 pestañas + 3 rutas secundarias | Navegación. |
| HTTP/JSON — HttpClient y System.Text.Json | `HttpClient` singleton (60 s) · `System.Net.Http.Json` · `JsonDocument` para errores | Consumo REST. |
| SQLite — sqlite-net-pcl | **1.9.172** + `SQLitePCLRaw.bundle_green` **2.1.10** | Historial y proyectos. |
| Almacenamiento seguro | SecureStorage de MAUI (Keystore / DataProtection) + `Preferences` | Claves y preferencias. |
| Registro | `Microsoft.Extensions.Logging.Debug` **9.0.0** (solo DEBUG) | Depuración. |
| Pruebas — AsistenteIA.Tests | **xUnit 2.9.2**, `xunit.runner.visualstudio` 2.8.2, `Microsoft.NET.Test.Sdk` 17.11.1 | 66 pruebas. |
| IA – Google Gemini | API `v1beta` `generateContent`. Modelos estables: `gemini-3.5-flash-lite`, `gemini-3.1-flash-lite`, `gemini-3.6-flash`, `gemini-3.8-flash` (todos vigentes; `gemini-2.0-*` y `gemini-3.1-flash-lite-preview` ya fueron retirados) | Proveedor 1. |
| IA – Pollinations | `gen.pollinations.ai/v1/chat/completions` (formato OpenAI), clave en `Authorization: Bearer`, modelo por defecto `openai` | Proveedor 2. |
| Control de versiones | Git + GitHub `sebitazz321/ASISTENTEIA` | Repositorio. |
| Asistencia de IA para desarrollo | Claude (Anthropic) (ver declaración de uso de IA, capítulo 13) | Apoyo en código y documentación. |

## 2.1 Fuentes

- Modelos Gemini: <https://ai.google.dev/gemini-api/docs/models>
- Pollinations API: <https://gen.pollinations.ai/docs> y <https://github.com/pollinations/pollinations/blob/master/APIDOCS.md>
- Fin de soporte de .NET 8 y 9: <https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/>
- Política de soporte de .NET MAUI: <https://dotnet.microsoft.com/platform/support/policy/maui>
