# 09. Requisitos técnicos actualizados

Versiones tomadas de `AsistenteIA.csproj`, `AsistenteIA.Tests.csproj` y `global.json`.

## 9.1 SDK y plataformas

| Elemento | Valor en el proyecto | Notas |
|---|---|---|
| .NET SDK | **9.0.100** (`global.json`, `rollForward: latestFeature`) | Acepta cualquier SDK 9.0.1xx o superior de la banda 9.0. |
| Frameworks de destino | `net9.0-android` · `net9.0-windows10.0.19041.0` | iOS y Mac Catalyst tienen carpeta en `Platforms`, pero no se compilan. |
| .NET MAUI | `$(MauiVersion)` (lo fija la carga de trabajo `maui` del SDK 9) | — |
| Windows (mínimo) | 10.0.17763.0 (Windows 10 versión 1809) | `WindowsPackageType = None` (app no empaquetada). |
| Android (mínimo) | API 21 (Android 5.0) | Compilado con el SDK de Android del workload. |
| Pruebas | `net9.0` (sin MAUI) | Se ejecutan en cualquier equipo con el SDK. |

> ⚠️ **Fin de soporte:** Microsoft finaliza el soporte de **.NET 8 y .NET 9 el 10 de noviembre de 2026**. Se recomienda migrar a
> **.NET 10 (LTS, soporte hasta noviembre de 2028)**: cambiar los TFM a `net10.0-…` y `global.json` a `10.0.100`. Ver
> [18-Conclusiones-Trabajo-Futuro](18-Conclusiones-Trabajo-Futuro.md).

## 9.2 Herramientas

| Herramienta | Versión mínima | Carga de trabajo / componente |
|---|---|---|
| Windows | 10 (1809) u 11 | — |
| Visual Studio 2022 | 17.12 o superior (compatible con .NET 9) | «Desarrollo de la interfaz de usuario de aplicaciones multiplataforma de .NET» (.NET MAUI) |
| Componentes adicionales | — | Herramientas de C++ para escritorio (requeridas por `SQLitePCLRaw` en Windows) |
| CLI (opcional) | `dotnet workload install maui` | Alternativa a Visual Studio |
| Android | Emulador API 34 o dispositivo con depuración USB | SDK de Android instalado por el workload |
| Git | 2.30 o superior | — |

## 9.3 Paquetes NuGet

| Paquete | Versión | Proyecto | Uso |
|---|---|---|---|
| `Microsoft.Maui.Controls` | `$(MauiVersion)` | App | Marco de interfaz. |
| `Microsoft.Extensions.Logging.Debug` | 9.0.0 | App | Registro en depuración. |
| `CommunityToolkit.Mvvm` | 8.3.2 | App | `[ObservableProperty]`, `[RelayCommand]`. |
| `sqlite-net-pcl` | 1.9.172 | App | ORM SQLite. |
| `SQLitePCLRaw.bundle_green` | 2.1.10 | App | Motor SQLite nativo. |
| `Microsoft.NET.Test.Sdk` | 17.11.1 | Pruebas | Ejecución de pruebas. |
| `xunit` | 2.9.2 | Pruebas | Marco de pruebas. |
| `xunit.runner.visualstudio` | 2.8.2 | Pruebas | Explorador de pruebas de VS. |

## 9.4 Servicios externos

| Servicio | Requisito |
|---|---|
| Google Gemini | Clave de Google AI Studio y un modelo vigente (por defecto `gemini-3.5-flash-lite`). |
| Pollinations | Clave de enter.pollinations.ai y un modelo de texto vigente (por defecto `openai`). |
| Red | Internet con salida HTTPS (443) a `generativelanguage.googleapis.com` y `gen.pollinations.ai`. |
