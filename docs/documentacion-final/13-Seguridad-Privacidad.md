# 13. Seguridad, privacidad y uso responsable de la IA

## 13.1 Matriz de riesgos: privacidad, seguridad y uso responsable

Escala: Probabilidad (P) y Impacto (I) de 1 (bajo) a 3 (alto). Nivel = P × I.

| # | Categoría | Riesgo | P | I | Nivel | Control implementado | Dónde |
|---|---|---|---|---|---|---|---|
| R01 | Seguridad | Exposición de la clave de API en el código o el repositorio | 2 | 3 | 6 | La clave se escribe en la app y se guarda en `SecureStorage`; no existe en el código ni en Git | `ApiKeyService` |
| R02 | Seguridad | La clave aparece en registros o URL | 2 | 3 | 6 | Viaja en el encabezado `x-goog-api-key` (Gemini) o `Authorization: Bearer` (Pollinations) sobre HTTPS; los mensajes de error nunca la incluyen | `AiService` |
| R03 | Seguridad | La clave queda visible en pantalla o en el teclado | 2 | 2 | 4 | `Entry IsPassword`, sin corrector ni predicción; el campo nunca se rellena con la clave guardada | `SettingsPage` |
| R04 | Seguridad | Copia de seguridad en la nube de la base de datos | 1 | 2 | 2 | `android:allowBackup="false"` y tráfico en claro deshabilitado | `AndroidManifest.xml` |
| R05 | Seguridad | Consumo excesivo o accidental de la cuota | 2 | 2 | 4 | Límite local de 8 consultas/min, máximo 2000 caracteres de entrada y 4096 tokens de salida, *timeout* de 60 s | `RequestLimiter`, `AiService` |
| R06 | Privacidad | El estudiante envía datos personales al proveedor | 3 | 3 | 9 | Detector (correo, tarjeta, teléfono/cédula, credenciales, claves) con confirmación previa; aviso de privacidad en Inicio y Configuración | `SensitiveDataDetector`, `ChatViewModel` |
| R07 | Privacidad | Datos locales permanecen tras dejar de usar la app | 2 | 2 | 4 | Botones *Borrar historial* y *Borrar todos mis datos locales*; *Eliminar clave* | `ProfileViewModel`, `SettingsViewModel` |
| R08 | Privacidad | El perfil (nombre) se envía a la IA | 1 | 2 | 2 | El perfil nunca forma parte de la solicitud; solo se envía el texto de la consulta y, si aplica, el nombre y descripción del proyecto | `AiService` |
| R09 | Uso responsable | Se acepta una respuesta incorrecta o inventada | 3 | 3 | 9 | Aviso permanente en *Consultar*; respuestas «Sin verificar» hasta que el usuario las marque; nota para anotar la fuente; *system prompt* que pide declarar la incertidumbre | `ChatPage`, `DetailViewModel` |
| R10 | Uso responsable | Plagio o entrega de trabajos generados sin declarar | 2 | 3 | 6 | Sección «Límites del uso de IA» y declaración de uso de IA del proyecto | `SettingsPage`, capítulo 13.3 |
| R11 | Uso responsable | Contenido inapropiado | 1 | 2 | 2 | *System prompt* académico; se informa si el proveedor bloquea la consulta | `AiService` |
| R12 | Continuidad | Cambio o caída del proveedor de IA | 2 | 2 | 4 | Dos proveedores con cambio manual, modelo configurable, errores 402/404/5xx con mensajes claros | `AiProviderCatalog`, `AiService`, `SettingsPage` |
| R13 | Integridad | Pérdida del historial por error | 1 | 2 | 2 | Eliminaciones siempre con confirmación; operaciones de base de datos con manejo de excepciones | ViewModels |
| R14 | Autoría | No poder demostrar quién hizo qué | 2 | 2 | 4 | Commits descriptivos por integrante, declaración de uso de IA y sustentación individual | Capítulo 13.3 |

**Riesgos residuales:** el detector es una heurística (no detecta todo tipo de dato personal) y la verificación
de las respuestas depende del criterio del estudiante; por eso el aviso se repite en las pantallas clave.

## 13.2 Auditoría de seguridad de API Keys

**Alcance:** almacenamiento, transmisión, visualización y uso de las claves de Gemini y Pollinations en el código del
3 de noviembre de 2026. **Método:** revisión del código fuente, del repositorio y de la configuración del proyecto.

### 13.2.1 Verificación de controles

| # | Control | Evidencia en el código | Resultado |
|---|---|---|---|
| C1 | No hay claves en el código | Búsqueda de `AIza…`, `sk-…`, `pk_…`, `sk_…` en todo el historial: solo aparecen dos claves **ficticias** dentro de las pruebas | ✅ Cumple |
| C2 | No hay claves en el repositorio ni en el historial Git | Revisión de todos los commits; sin archivos de configuración con secretos | ✅ Cumple |
| C3 | Almacenamiento cifrado | `ApiKeyService` → `SecureStorage.Default` | ✅ Cumple |
| C4 | Campo oculto | `Entry IsPassword="True"`, sin corrector ni predicción | ✅ Cumple |
| C5 | La clave no se vuelve a mostrar | El ViewModel vacía `ApiKey` tras guardar; solo se informa si existe | ✅ Cumple |
| C6 | La clave no viaja en la URL | `x-goog-api-key` / `Authorization: Bearer` | ✅ Cumple |
| C7 | Solo HTTPS | Endpoints `https://`; Android `usesCleartextTraffic="false"` | ✅ Cumple |
| C8 | Errores sin la clave | `AiErrorMapper` usa textos fijos; prueba `Messages_NeverContainTheKey` | ✅ Cumple |
| C9 | Sin copias de seguridad en Android | `android:allowBackup="false"` | ✅ Cumple |
| C10 | Eliminación por el usuario | Botón *Eliminar clave* con confirmación | ✅ Cumple |
| C11 | Límite de uso | `RequestLimiter`: 8 solicitudes/min | ✅ Cumple |
| C12 | Aviso al pegar una clave en una consulta | `SensitiveDataDetector` detecta `AIza…` y `sk-…` | ✅ Cumple |
| C13 | Clave separada por proveedor | `ai_api_key` / `pollinations_api_key` | ✅ Cumple |
| C14 | Registros (logs) sin la clave | No se registran encabezados; `AddDebug` solo en DEBUG | ✅ Cumple |

### 13.2.2 Matriz de riesgos de credenciales

| Riesgo | Prob. | Impacto | Nivel | Mitigación implementada | Riesgo residual |
|---|---|---|---|---|---|
| Clave publicada en GitHub | Baja | Alto | Medio | C1, C2, `.gitignore` | Bajo |
| Clave visible en capturas o video | Media | Alto | Alto | C4, C5 | Bajo |
| Robo del archivo de la app / respaldo | Baja | Alto | Medio | C3, C9 | Bajo |
| Interceptación en red | Baja | Alto | Medio | C6, C7 | Bajo |
| Clave en registros o mensajes | Baja | Medio | Bajo | C8, C14 | Bajo |
| Gasto de cuota por uso indebido del equipo | Media | Medio | Medio | C11 | Medio (el equipo desbloqueado sigue pudiendo consultar) |
| Clave pegada en una consulta y enviada al proveedor | Media | Alto | Alto | C12 (aviso con confirmación) | Medio (es una heurística) |
| Ingeniería inversa del APK para extraer claves | — | — | — | No aplica: la app no incluye claves; cada usuario usa la suya | Nulo |

### 13.2.3 Hallazgos y recomendaciones

| ID | Hallazgo | Severidad | Recomendación |
|---|---|---|---|
| H1 | La clave se lee en memoria como `string` y permanece hasta que el recolector la libera. | Baja | Aceptable en una app cliente; no guardarla en campos estáticos (ya se cumple). |
| H2 | En Windows no empaquetado, el cifrado depende de la cuenta de usuario de Windows. | Baja | Recomendar cuentas con contraseña y no compartir el usuario. |
| H3 | No hay rotación ni caducidad automática de claves. | Baja | Indicar en el manual cómo revocar la clave en Google AI Studio / Pollinations si se filtra. |
| H4 | Las claves son personales y se usan desde el cliente (sin servidor intermedio). | Media (si se publica la app) | Para uso masivo, usar un backend que guarde la clave y aplique cuotas por usuario. |
| H5 | Las dependencias pueden tener vulnerabilidades nuevas. | Baja | Ejecutar `dotnet list package --vulnerable` antes de cada entrega. |

**Conclusión:** no se encontraron claves expuestas y los 14 controles se cumplen. Los riesgos residuales son bajos
para un proyecto académico de uso personal.

## 13.3 Declaración de uso de herramientas de IA

El equipo declara que usó las siguientes herramientas de IA durante el proyecto:

- **Claude (Anthropic):** apoyo para depurar errores de compilación y de entorno, generar y revisar código C# y XAML
  (incluida la integración del proveedor Pollinations, el manejo de errores 402/408 y sus pruebas), proponer el diseño
  visual y redactar borradores de esta documentación.
- **Google Gemini y Pollinations:** servicios de IA que consume la aplicación en su funcionamiento; no se usaron para
  escribir el código.

El equipo revisó el código y la documentación producidos con ayuda de IA, y cada integrante puede explicar el
recorrido completo de una consulta (capítulo 5.5) y justificar las decisiones de seguridad.

**Fuentes técnicas consultadas:** Microsoft Learn (.NET MAUI, `HttpClient`, inyección de dependencias,
`SecureStorage`), documentación de CommunityToolkit.Mvvm, sqlite-net, Google Gemini API y Pollinations API.
