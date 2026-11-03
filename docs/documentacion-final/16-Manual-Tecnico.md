# 16. Manual técnico para desarrolladores

## 16.1 Organización del código

```
AsistenteIA.slnx
├── AsistenteIA/                     # App .NET MAUI (Windows, Android)
│   ├── MauiProgram.cs               # Inyección de dependencias y HttpClient (60 s)
│   ├── AppShell.xaml(.cs)           # TabBar de 5 pestañas + rutas Detail, ProjectDetail, Settings
│   ├── Models/                      # ChatModels (DTO Gemini y OpenAI), ConversationRecord, Project,
│   │                                # PromptTemplate, UserProfile, AiProvider (catálogo de proveedores)
│   ├── Services/                    # AiService, AiErrorMapper, ApiKeyService, DatabaseService, ProfileService,
│   │                                # PromptValidator, SensitiveDataDetector, RequestLimiter,
│   │                                # INavigationService, IDialogService
│   ├── ViewModels/                  # 8 ViewModels con CommunityToolkit.Mvvm
│   ├── Views/                       # 8 páginas XAML (solo binding)
│   ├── Resources/                   # Colors.xaml, Styles.xaml, fuentes, íconos
│   └── Platforms/                   # Android (manifest), Windows, iOS, MacCatalyst
├── AsistenteIA.Tests/               # xUnit (net9.0) — enlaza los archivos de lógica pura
└── docs/                            # Documentación (01–07 y documentacion-final/)
```

## 16.2 Patrones y reglas

| Regla | Detalle |
|---|---|
| MVVM | Las vistas no tienen lógica; los ViewModels no usan `Shell` directamente (usan `INavigationService` e `IDialogService`). |
| Inyección | Servicios *singleton*; ViewModels y páginas *transient*; las páginas reciben su ViewModel por constructor. |
| Asincronía | Toda E/S es `async`; las consultas aceptan `CancellationToken`. |
| Lógica probable | Lo que no depende de MAUI va en clases puras y se enlaza en `AsistenteIA.Tests.csproj` con `<Compile Include … Link=…>`. |
| Configuración | Proveedor y modelo en `Preferences`; claves en `SecureStorage`; nada en el código. |

## 16.3 Inyección de dependencias (`MauiProgram.cs`)

| Registro | Ciclo de vida | Motivo |
|---|---|---|
| `HttpClient` (BaseAddress de Gemini, Timeout 60 s) | Singleton | Reutiliza conexiones. |
| `RequestLimiter`, `ApiKeyService`, `AiService`, `DatabaseService`, `ProfileService` | Singleton | Estado compartido (límite, conexión SQLite). |
| `INavigationService` → `ShellNavigationService`, `IDialogService` → `ShellDialogService` | Singleton | Abstraen Shell para los ViewModels. |
| 8 ViewModels y 8 páginas | Transient | Cada pantalla empieza limpia. |

## 16.4 Configuración y ejecución

```bash
git clone https://github.com/sebitazz321/ASISTENTEIA.git
cd ASISTENTEIA
dotnet workload install maui            # si no usa Visual Studio
dotnet build AsistenteIA/AsistenteIA.csproj -f net9.0-windows10.0.19041.0
dotnet build AsistenteIA/AsistenteIA.csproj -f net9.0-android
```

En Visual Studio: abrir `AsistenteIA.slnx`, elegir **Windows Machine** (o un emulador Android) y pulsar **Ejecutar**.
El ejecutable de depuración queda en `AsistenteIA/bin/Debug/net9.0-windows10.0.19041.0/win10-x64/AsistenteIA.exe`
(la arquitectura puede variar).

## 16.5 Pruebas

```bash
dotnet test AsistenteIA.Tests        # 66 pruebas: validador, datos sensibles, límite, perfil, proveedores, errores
```

| Clase | Casos | Cubre |
|---|---|---|
| `PromptValidatorTests` | 8 | RF02 |
| `SensitiveDataDetectorTests` | 18 | Privacidad |
| `RequestLimiterTests` | 3 | Límite de uso |
| `UserProfileTests` | 5 | Iniciales del avatar |
| `AiProviderCatalogTests` | 16 | Catálogo, parseo, modelos, endpoints, claves en encabezado |
| `AiErrorMapperTests` | 16 | 400, 401, 402, 403, 404, 408, 429, 5xx, conexión, cancelación, sin clave en mensajes |

Las pruebas manuales P01–P20 están en el capítulo 14.

## 16.6 Mantenimiento

| Tarea | Cómo |
|---|---|
| Cambiar los modelos sugeridos | `AiProviderCatalog` (`SuggestedModels`, `DefaultModel`) y su prueba `DefaultModel_IsAmongSuggested`. |
| Agregar un proveedor | Ver el capítulo 7.4. |
| Agregar un campo a SQLite | Propiedad en la entidad; `CreateTableAsync` agrega la columna a bases existentes (no borra datos). |
| Cambiar el límite de uso | `RequestLimiter.DefaultMaxRequests` / `DefaultWindow`. |
| Cambiar el tiempo máximo | `Timeout` del `HttpClient` en `MauiProgram.cs`. |
| Actualizar paquetes | `dotnet list package --outdated` y `--vulnerable`; probar y registrar en el historial de cambios. |
| Migrar a .NET 10 | TFM `net10.0-android;net10.0-windows10.0.19041.0`, `global.json` 10.0.100, actualizar paquetes y ejecutar las pruebas. |
| Entregar sin binarios | `git archive --format=zip --output=../AsistenteIA-entrega.zip HEAD` |
