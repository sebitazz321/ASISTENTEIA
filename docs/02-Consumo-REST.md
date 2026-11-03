# 2. Consumo de la API REST de IA

Código: `AsistenteIA/Services/AiService.cs` · Modelos JSON: `AsistenteIA/Models/ChatModels.cs`

## 2.1 Solicitud

| Elemento | Valor |
|---|---|
| Método | `POST` |
| URL base | `https://generativelanguage.googleapis.com/v1beta/` (solo HTTPS) |
| Ruta | `models/{modelo}:generateContent` (modelo por defecto: `gemini-3.5-flash-lite`, configurable en *Configuración*) |
| Autenticación | Encabezado `x-goog-api-key: <clave del usuario>` (la clave sale de `SecureStorage`; nunca va en la URL) |
| Content-Type | `application/json` |
| Tiempo de espera | 60 s (`HttpClient.Timeout`) |
| Cancelación | `CancellationToken` ligado al botón *Cancelar* |

Cuerpo (ejemplo; `projectContext` solo aparece si la consulta pertenece a un proyecto):

```json
{
  "systemInstruction": {
    "parts": [{ "text": "Eres un asistente académico. Responde de forma clara, indica la incertidumbre cuando exista y no inventes fuentes ni citas. Contexto del proyecto del estudiante (úsalo solo como contexto): Cálculo II. Taller de derivadas." }]
  },
  "contents": [
    { "role": "user", "parts": [{ "text": "Explica con un ejemplo qué es la regla de la cadena." }] }
  ],
  "generationConfig": { "maxOutputTokens": 4096 }
}
```

## 2.2 Respuesta exitosa (HTTP 200)

Estructura (ejemplo ilustrativo; el texto cambia en cada ejecución):

```json
{
  "candidates": [
    {
      "content": { "role": "model", "parts": [{ "text": "La regla de la cadena permite derivar funciones compuestas..." }] },
      "finishReason": "STOP"
    }
  ]
}
```

`AiService` concatena los `parts[].text` del primer candidato. Casos especiales:

| Situación | Tratamiento |
|---|---|
| `finishReason = MAX_TOKENS` | Se añade el aviso «Respuesta recortada por el límite de longitud» |
| Sin `candidates` y `promptFeedback.blockReason` | «El servicio bloqueó la consulta (motivo)…» |
| Sin contenido | «El servicio no devolvió contenido.» |

## 2.3 Códigos HTTP y mensajes al usuario

| Código / condición | Causa típica | Mensaje mostrado (`AiService.GetFriendlyMessage`) |
|---|---|---|
| 200 | Éxito | La respuesta |
| 400 con «API key» | Clave mal escrita | Error de autorización (400): la clave de API no es válida. |
| 400 | Modelo o cuerpo inválido | Solicitud rechazada (400). Revise el modelo configurado. |
| 401 / 403 | Clave inválida o sin permisos | Error de autorización (401/403): la clave es inválida o no tiene permisos. |
| 404 | Modelo inexistente | Modelo no encontrado (404). Cambie el modelo en Configuración. |
| 429 | Cuota o límite del proveedor | Se superó la cuota o el límite de solicitudes (429). Espere un momento. |
| 5xx | Falla del proveedor | El servicio de IA no está disponible (5xx). Intente más tarde. |
| Sin red (`StatusCode = null`) | DNS/conexión | No se pudo conectar con el servicio. Verifique su conexión. |
| `NetworkAccess != Internet` | Modo avión | Sin conexión a internet. Verifique su red e intente de nuevo. |
| Tiempo agotado (60 s) | Servicio lento | Tiempo de espera agotado. Intente de nuevo. |
| Cancelación del usuario | Botón *Cancelar* | Consulta cancelada. |
| Límite local (8/min) | Ráfaga de consultas | Límite de uso alcanzado: espere N s antes de consultar de nuevo. |
| Validación local | Consulta vacía/corta/larga | Mensaje de `PromptValidator` (no se envía nada) |

Ningún mensaje incluye la clave ni el cuerpo de la solicitud.

## 2.4 Asincronía

Toda la comunicación usa `async/await` (`HttpClient.SendAsync`, `ReadFromJsonAsync`). La interfaz sigue
respondiendo mientras se espera (`IsBusy` muestra el `ActivityIndicator` y el botón *Cancelar* sigue activo).

## 2.5 Evidencia para entregar

Adjunte (en una carpeta `evidencias/`) capturas de: consulta exitosa, error por clave inválida (P04) y consulta
sin conexión (P03). Opcional: la pestaña *Red* de las herramientas de desarrollo o un registro de `dotnet`
**sin** la clave visible.
