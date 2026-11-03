# 10. Auditoría de seguridad de API Keys

**Alcance:** almacenamiento, transmisión, visualización y uso de las claves de Gemini y Pollinations en el código del
3 de noviembre de 2026. **Método:** revisión del código fuente, del repositorio y de la configuración del proyecto.

## 10.1 Verificación de controles

| # | Control | Evidencia en el código | Resultado |
|---|---|---|---|
| C1 | No hay claves en el código | Búsqueda de `AIza…`, `sk-…`, `pk_…`, `sk_…` en todo el historial: solo aparecen dos claves **ficticias** dentro de las pruebas | ✅ Cumple |
| C2 | No hay claves en el repositorio ni en el historial Git | Revisión de los 6 commits; sin archivos de configuración con secretos | ✅ Cumple |
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

## 10.2 Matriz de riesgos de credenciales

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

## 10.3 Hallazgos y recomendaciones

| ID | Hallazgo | Severidad | Recomendación |
|---|---|---|---|
| H1 | La clave se lee en memoria como `string` y permanece hasta que el recolector la libera. | Baja | Aceptable en una app cliente; no guardarla en campos estáticos (ya se cumple). |
| H2 | En Windows no empaquetado, el cifrado depende de la cuenta de usuario de Windows. | Baja | Recomendar cuentas con contraseña y no compartir el usuario. |
| H3 | No hay rotación ni caducidad automática de claves. | Baja | Indicar en el manual cómo revocar la clave en Google AI Studio / Pollinations si se filtra. |
| H4 | Las claves son personales y se usan desde el cliente (sin servidor intermedio). | Media (si se publica la app) | Para uso masivo, usar un backend que guarde la clave y aplique cuotas por usuario. |
| H5 | Las dependencias pueden tener vulnerabilidades nuevas. | Baja | Ejecutar `dotnet list package --vulnerable` antes de cada entrega. |

**Conclusión:** no se encontraron claves expuestas y los 14 controles se cumplen. Los riesgos residuales son bajos
para un proyecto académico de uso personal.
