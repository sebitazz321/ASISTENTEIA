# 15. Actualización de funcionalidades

Comparación entre lo que describe el documento (§2.2 requisitos funcionales y §4 interfaz) y lo que realmente
implementa el código.

## 15.1 Requisitos del documento

| Código | Requisito | Documento | Código real | Observación |
|---|---|---|---|---|
| RF01 | Ingresar pregunta | Cumple | ✅ | Editor con contador «N / 2000». |
| RF02 | Validar contenido y longitud | Cumple | ✅ | 5–2000 caracteres (`PromptValidator`). |
| RF03 | POST a la API REST | Cumple | ✅ ampliado | Ahora a Gemini **o** Pollinations. |
| RF04 | Estados de carga, respuesta, error, sin conexión | Cumple | ✅ ampliado | Mensajes 402 y 408 agregados. |
| RF05 | Guardar en SQLite | Cumple | ✅ | Botón Guardar. |
| RF06 | Consultar, filtrar, detalle, eliminar | Cumple | ✅ | Búsqueda también en respuesta, categoría y proyecto. |
| RF07 | Plantillas editables | Cumple | ✅ | 6 plantillas. |
| RF08 | Clave sin mostrarla | Cumple | ✅ ampliado | Una clave por proveedor. |
| RF09 | Acerca de | Cumple | ✅ corregido | La «Autoría» tenía un texto de plantilla; ahora tiene los nombres. |
| RF10 | Windows o Android | Cumple | ✅ | Probado en Windows según el documento. |

## 15.2 Funcionalidades implementadas que el documento no describía

| ID | Funcionalidad | Dónde | Estado |
|---|---|---|---|
| RF11 | **Proveedores Gemini y Pollinations con cambio manual** | Configuración, `AiService` | **Nuevo** (3/11/2026) |
| RF12 | Aviso de uso responsable al primer inicio | Inicio (`HomeViewModel`) | Ya existía en el zip |
| RF13 | Aviso de datos sensibles antes de enviar (correo, tarjeta, teléfono/cédula, credenciales, claves) | Consultar (`SensitiveDataDetector`) | Ya existía |
| RF14 | Límite de 8 consultas por minuto | `RequestLimiter` | Ya existía |
| RF15 | Contexto del proyecto enviado a la IA | Consultar | Ya existía |
| RF16 | Aviso de respuesta recortada (límite de 4096 tokens) | `AiService` | Ya existía |
| RF17 | Nota personal por consulta | Detalle de consulta | Ya existía |
| RF18 | Detalle de proyecto: editar, ver sus consultas, consultar en el proyecto, eliminar conservando consultas | Detalle de proyecto | Ya existía |
| RF19 | Estadísticas del perfil (consultas, verificadas, proyectos) | Perfil | Ya existía (el documento lo menciona en §4.5) |
| RF20 | Borrar historial / borrar todos los datos locales | Perfil | Ya existía |
| RF21 | Mensajes de error 402 y 408 | `AiErrorMapper` | **Nuevo** |

## 15.3 Diferencias en pruebas y pantallas

| Tema | Documento | Real |
|---|---|---|
| Casos de prueba manuales | «Se ejecutaron los diez casos» pero la tabla tiene **nueve** (P01–P09); la lista de cotejo dice «Nueve casos con evidencia». | `docs/03-Plan-de-pruebas.md` define **P01–P15**. Falta ejecutar y documentar P10–P15. |
| Pruebas automáticas | 34 | **66** (34 + 32 nuevas de proveedores y errores). Pendiente: ejecutarlas en Visual Studio y actualizar la Figura 18. |
| Pantallas | 7 en la lista de cotejo | **8** (falta «Detalle de proyecto»). |
| Configuración | Clave y modelo | Proveedor + clave + modelo por proveedor. |

## 15.4 Casos de prueba nuevos sugeridos

| Caso | Condición | Resultado esperado |
|---|---|---|
| P16 | Cambiar a Pollinations, guardar clave y probar conexión | «✔ Conexión exitosa con Pollinations». |
| P17 | Consultar con Pollinations | La respuesta aparece y el estado dice «Respuesta recibida de Pollinations». |
| P18 | Volver a Gemini sin escribir la clave otra vez | Funciona: cada proveedor conserva su clave. |
| P19 | Cambiar el selector sin pulsar Guardar y consultar | Se sigue usando el proveedor activo anterior. |
| P20 | Clave inválida en Pollinations | «Error de autorización (401)…» sin mostrar la clave. |
