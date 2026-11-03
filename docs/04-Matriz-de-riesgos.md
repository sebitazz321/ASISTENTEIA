# 4. Matriz de riesgos: privacidad, seguridad y uso responsable

Escala: Probabilidad (P) y Impacto (I) de 1 (bajo) a 3 (alto). Nivel = P × I.

| # | Categoría | Riesgo | P | I | Nivel | Control implementado | Dónde |
|---|---|---|---|---|---|---|---|
| R01 | Seguridad | Exposición de la clave de API en el código o el repositorio | 2 | 3 | 6 | La clave se escribe en la app y se guarda en `SecureStorage`; no existe en el código ni en Git | `ApiKeyService` |
| R02 | Seguridad | La clave aparece en registros o URL | 2 | 3 | 6 | Viaja en el encabezado `x-goog-api-key` sobre HTTPS; los mensajes de error nunca la incluyen | `AiService` |
| R03 | Seguridad | La clave queda visible en pantalla o en el teclado | 2 | 2 | 4 | `Entry IsPassword`, sin corrector ni predicción; el campo nunca se rellena con la clave guardada | `SettingsPage` |
| R04 | Seguridad | Copia de seguridad en la nube de la base de datos | 1 | 2 | 2 | `android:allowBackup="false"` y tráfico en claro deshabilitado | `AndroidManifest.xml` |
| R05 | Seguridad | Consumo excesivo o accidental de la cuota | 2 | 2 | 4 | Límite local de 8 consultas/min, máximo 2000 caracteres de entrada y 4096 tokens de salida, *timeout* de 60 s | `RequestLimiter`, `AiService` |
| R06 | Privacidad | El estudiante envía datos personales al proveedor | 3 | 3 | 9 | Detector (correo, tarjeta, teléfono/cédula, credenciales, claves) con confirmación previa; aviso de privacidad en Inicio y Configuración | `SensitiveDataDetector`, `ChatViewModel` |
| R07 | Privacidad | Datos locales permanecen tras dejar de usar la app | 2 | 2 | 4 | Botones *Borrar historial* y *Borrar todos mis datos locales*; *Eliminar clave* | `ProfileViewModel`, `SettingsViewModel` |
| R08 | Privacidad | El perfil (nombre) se envía a la IA | 1 | 2 | 2 | El perfil nunca forma parte de la solicitud; solo se envía el texto de la consulta y, si aplica, el nombre y descripción del proyecto | `AiService` |
| R09 | Uso responsable | Se acepta una respuesta incorrecta o inventada | 3 | 3 | 9 | Aviso permanente en *Consultar*; respuestas «Sin verificar» hasta que el usuario las marque; nota para anotar la fuente; *system prompt* que pide declarar la incertidumbre | `ChatPage`, `DetailViewModel` |
| R10 | Uso responsable | Plagio o entrega de trabajos generados sin declarar | 2 | 3 | 6 | Sección «Límites del uso de IA» y declaración de uso de IA del proyecto | `SettingsPage`, `07-…` |
| R11 | Uso responsable | Contenido inapropiado | 1 | 2 | 2 | *System prompt* académico; se informa si el proveedor bloquea la consulta | `AiService` |
| R12 | Continuidad | Cambio o caída del proveedor de IA | 2 | 2 | 4 | Endpoint en una constante, modelo configurable, errores 404/5xx con mensajes claros | `AiService`, `SettingsPage` |
| R13 | Integridad | Pérdida del historial por error | 1 | 2 | 2 | Eliminaciones siempre con confirmación; operaciones de base de datos con manejo de excepciones | ViewModels |
| R14 | Autoría | No poder demostrar quién hizo qué | 2 | 2 | 4 | Commits descriptivos por integrante, declaración de uso de IA y sustentación individual | `07-…` |

**Riesgos residuales:** el detector es una heurística (no detecta todo tipo de dato personal) y la verificación
de las respuestas depende del criterio del estudiante; por eso el aviso se repite en las pantallas clave.
