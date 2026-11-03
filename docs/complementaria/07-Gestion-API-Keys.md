# 07. Gestión de API Keys

## 7.1 Ciclo de vida

| Paso | Dónde | Qué ocurre |
|---|---|---|
| Crear la clave | Google AI Studio / enter.pollinations.ai | El estudiante la crea con su cuenta. |
| Escribirla | Configuración → campo **Clave de API** (`IsPassword=True`, sin corrector ni predicción) | El texto se oculta mientras se escribe. |
| Guardarla | `ApiKeyService.SaveAsync(clave, proveedor)` | Se guarda en `SecureStorage` con `Trim()`. El campo se vacía. |
| Consultar estado | `HasKeyAsync(proveedor)` | La pantalla solo dice «Clave … guardada de forma segura» o «Sin clave configurada». **La clave nunca se vuelve a mostrar.** |
| Usarla | `AiService.AskAsync` | Se lee justo antes del POST y se envía en `x-goog-api-key` (Gemini) o `Authorization: Bearer` (Pollinations). |
| Probarla | Botón **Probar conexión** | Envía una consulta corta y muestra el resultado o el error (sin la clave). |
| Eliminarla | Botón **Eliminar clave** (con confirmación) | `SecureStorage.Remove` del proveedor seleccionado. |

## 7.2 Dónde se guarda en cada plataforma

| Plataforma | Mecanismo de `SecureStorage` (.NET MAUI) |
|---|---|
| Android | `EncryptedSharedPreferences` con clave en **Android Keystore**. Además `android:allowBackup="false"` evita copias de seguridad de los datos de la app. |
| Windows | Cifrado con **DataProtectionProvider** asociado al usuario de Windows (datos de la app no empaquetada). |
| iOS / Mac Catalyst | **Llavero (Keychain)**. *(Plataformas no compiladas en este proyecto.)* |

## 7.3 Protección de credenciales

- Ninguna clave en el código, en `appsettings`, en el repositorio ni en capturas.
- La clave no viaja en la URL (no queda en historiales ni registros de proxy).
- Los mensajes de error se construyen con textos fijos (`AiErrorMapper`), nunca con la clave.
- `SensitiveDataDetector` avisa si el estudiante pega una clave (`AIza…`, `sk-…`) dentro de una consulta.
- Una clave independiente por proveedor (`ai_api_key`, `pollinations_api_key`).
- `RequestLimiter` (8 consultas por minuto) limita el gasto de cuota si alguien usa el equipo.

Análisis de riesgos y hallazgos: [10-Auditoria-Seguridad-API-Keys](10-Auditoria-Seguridad-API-Keys.md).
