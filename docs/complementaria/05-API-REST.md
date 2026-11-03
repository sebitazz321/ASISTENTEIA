# 05. Documentación de la API REST

La aplicación **consume** dos API REST de terceros (no expone una API propia). Ambas se llaman con un único
`HttpClient` registrado como *singleton* (tiempo máximo **60 s**), método **POST**, cuerpo **JSON** y la clave
**en un encabezado**, nunca en la URL.

## 5.1 Google Gemini — `generateContent`

| Elemento | Valor |
|---|---|
| Método y URL | `POST https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent` |
| Parámetro de ruta | `{modelo}`: identificador del modelo (por defecto `gemini-3.5-flash-lite`). Si el usuario escribe `models/x`, se limpia a `x`. |
| Encabezados | `x-goog-api-key: <clave>` · `Content-Type: application/json; charset=utf-8` |

**Solicitud**

```json
{
  "systemInstruction": { "parts": [ { "text": "Eres un asistente académico. Responde de forma clara, indica la incertidumbre cuando exista y no inventes fuentes ni citas." } ] },
  "contents": [ { "role": "user", "parts": [ { "text": "Explica qué es una API REST" } ] } ],
  "generationConfig": { "maxOutputTokens": 4096 }
}
```

| Campo | Obligatorio | Descripción |
|---|---|---|
| `systemInstruction.parts[].text` | No | Instrucción de sistema. Si la consulta tiene proyecto, se agrega «Contexto del proyecto…» (máx. 400 caracteres). |
| `contents[].role` / `parts[].text` | Sí | Turno del usuario con la consulta (5–2000 caracteres). |
| `generationConfig.maxOutputTokens` | No | Límite de longitud de la respuesta (4096). |

**Respuesta 200**

```json
{
  "candidates": [ {
    "content": { "parts": [ { "text": "Una API REST es ..." } ] },
    "finishReason": "STOP"
  } ],
  "promptFeedback": { "blockReason": null }
}
```

| Campo | Uso en la app |
|---|---|
| `candidates[0].content.parts[].text` | Se concatenan y se muestran. |
| `candidates[0].finishReason = "MAX_TOKENS"` | Se agrega el aviso «Respuesta recortada por el límite de longitud». |
| `promptFeedback.blockReason` | Si no hay texto: «El servicio bloqueó la consulta (motivo)». |

**Respuesta de error**

```json
{ "error": { "code": 400, "message": "API key not valid. Please pass a valid API key.", "status": "INVALID_ARGUMENT" } }
```

## 5.2 Pollinations — `chat/completions` (compatible con OpenAI)

| Elemento | Valor |
|---|---|
| Método y URL | `POST https://gen.pollinations.ai/v1/chat/completions` |
| Encabezados | `Authorization: Bearer <clave>` · `Content-Type: application/json; charset=utf-8` |
| Clave | Se crea en `https://enter.pollinations.ai`. La API pública de texto ya no permite uso anónimo sin límites. |

**Solicitud**

```json
{
  "model": "openai",
  "messages": [
    { "role": "system", "content": "Eres un asistente académico. ..." },
    { "role": "user", "content": "Explica qué es una API REST" }
  ],
  "max_tokens": 4096
}
```

**Respuesta 200**

```json
{
  "choices": [ { "message": { "role": "assistant", "content": "Una API REST es ..." }, "finish_reason": "stop" } ]
}
```

| Campo | Uso en la app |
|---|---|
| `choices[0].message.content` | Texto mostrado. |
| `choices[0].finish_reason = "length"` | Aviso de respuesta recortada. |
| Error | Se lee `{"error":{"message":…}}`, `{"error":"…"}` o texto plano (máx. 200 caracteres). |

## 5.3 Códigos de estado

| Código | Significado | Mensaje en la app |
|---|---|---|
| 200 | Correcto | Se muestra la respuesta y se habilita *Guardar*. |
| 400 | Solicitud inválida (en Gemini, también clave inválida) | «Error de autorización (400): la clave de API no es válida» o «Solicitud rechazada (400). Revise el modelo configurado». |
| 401 / 403 | Sin autorización / sin permisos | «Error de autorización (401/403): la clave de {proveedor} es inválida o no tiene permisos». |
| 402 | Sin saldo o límite gratuito (Pollinations) | «{proveedor} rechazó la solicitud por saldo o límite gratuito (402)…» |
| 404 | Modelo no encontrado | «Modelo no encontrado (404). Cambie el modelo en Configuración». |
| 408 | El servidor agotó su tiempo | «El servidor tardó demasiado en responder (408)…» |
| 429 | Cuota o límite de solicitudes | «Se superó la cuota o el límite de solicitudes (429)…» |
| 5xx | Falla del proveedor | «El servicio de IA no está disponible (código)…» |

El detalle completo del manejo de errores está en [08-Gestion-Errores](08-Gestion-Errores.md).
