# 00. Comparación: documento entregado vs. lista de requisitos

Se comparó el documento **«AsistenteIA – Documentación del proyecto final» (PDF, 21 páginas, octubre de 2026)** con
los 18 entregables solicitados y con el **código real** del proyecto (`AsistenteIA_mejorado.zip`).

**Convención:** ✅ completo · ⚠️ parcial o desactualizado · ❌ no existe en el documento.

| # | Entregable solicitado | Estado en el PDF | Qué tenía el PDF | Qué faltaba o estaba mal | Dónde quedó completo |
|---|---|---|---|---|---|
| 1 | Diagrama de arquitectura del sistema | ❌ | Tabla de carpetas (§3.1) y recorrido de una consulta en texto (§3.2). | No hay representación visual de componentes ni de la comunicación app ↔ API de IA ↔ SQLite. | [01-Diagrama-Arquitectura](01-Diagrama-Arquitectura.md) |
| 2 | Diagrama de clases | ❌ | Lista de clases por carpeta. | No hay atributos, métodos, responsabilidades ni relaciones. | [02-Diagrama-Clases](02-Diagrama-Clases.md) |
| 3 | Modelo entidad-relación (ERD) | ⚠️ | Una entidad (§6.1). | Falta la tabla **Project** y su relación; el campo se llama «Proyecto» en vez de `ProjectId`; falta `Note`; no hay PK/FK ni cardinalidad. | [03-Modelo-Entidad-Relacion](03-Modelo-Entidad-Relacion.md) |
| 4 | Diccionario de datos | ⚠️ | Campo / tipo / descripción de una tabla. | Sin restricciones (longitudes, nulos, valores por defecto), sin tabla Project, sin claves de Preferences y SecureStorage. | [04-Diccionario-Datos](04-Diccionario-Datos.md) |
| 5 | Documentación de la API REST | ⚠️ | Solicitud, respuesta y códigos de Gemini (§5). | Faltan parámetros (`generationConfig.maxOutputTokens`), cuerpo de error, `finishReason`/`promptFeedback`, códigos 402 y 408, y la API de Pollinations. | [05-API-REST](05-API-REST.md) |
| 6 | Arquitectura de proveedores de IA (Gemini y Pollinations, cambio manual) | ❌ | Pollinations solo aparece como alternativa descartada (§3.4). | El código **no** tenía Pollinations ni selección de proveedor. **Se implementó** en el código y se documentó. | [06-Arquitectura-Proveedores-IA](06-Arquitectura-Proveedores-IA.md) |
| 7 | Gestión de API Keys | ⚠️ | Menciones a SecureStorage, campo oculto y encabezado (§1, §3.4, §5.1). | No explica ciclo de vida (guardar, probar, eliminar), cifrado por plataforma ni una clave por proveedor. | [07-Gestion-API-Keys](07-Gestion-API-Keys.md) |
| 8 | Gestión de errores y excepciones | ⚠️ | Tabla de códigos (§5.3). | Faltaban **408** (tampoco estaba en el código) y **402**; no documenta las excepciones internas ni el flujo. | [08-Gestion-Errores](08-Gestion-Errores.md) |
| 9 | Requisitos técnicos actualizados | ⚠️ | «.NET 9 o superior», carga de trabajo de VS, paquetes sin versión (§10.1). | Sin versiones exactas (SDK 9.0.100, MAUI, paquetes), sin versiones mínimas de Windows/Android, sin aviso de fin de soporte de .NET 9. | [09-Requisitos-Tecnicos](09-Requisitos-Tecnicos.md) |
| 10 | Auditoría de seguridad de API Keys | ❌ | Una fila en la matriz de riesgos (§8.1). | No hay análisis de amenazas, verificación de controles ni hallazgos. | [10-Auditoria-Seguridad-API-Keys](10-Auditoria-Seguridad-API-Keys.md) |
| 11 | Manual de usuario actualizado | ⚠️ | Primeros pasos, consulta, historial, proyectos (§9). | No cubre: selección de proveedor, aviso de uso responsable, aviso de datos sensibles, límite de 8 consultas/min, notas, detalle de proyecto, borrar historial / todos los datos. | [11-Manual-Usuario](11-Manual-Usuario.md) |
| 12 | Manual técnico para desarrolladores | ⚠️ | «Manual técnico breve» (§10). | Falta organización del código, cómo agregar un proveedor, ejecución de pruebas por consola, mantenimiento. | [12-Manual-Tecnico](12-Manual-Tecnico.md) |
| 13 | Manual de solución de problemas | ❌ | — | No existe. | [13-Solucion-Problemas](13-Solucion-Problemas.md) |
| 14 | Actualización de tecnologías utilizadas | ⚠️ | Tabla de tecnologías sin versiones (§1). | Sin versiones; no menciona xUnit, Pollinations ni la vigencia de los modelos Gemini. | [14-Actualizacion-Tecnologias](14-Actualizacion-Tecnologias.md) |
| 15 | Actualización de funcionalidades | ⚠️ | RF01–RF10 «Cumple» (§2.2). | El código tiene funciones no descritas (8 pantallas, notas, límite de uso, datos sensibles, borrado de datos) y el PDF dice «diez casos» pero solo lista nueve (P01–P09). | [15-Actualizacion-Funcionalidades](15-Actualizacion-Funcionalidades.md) |
| 16 | Historial de cambios del proyecto | ❌ | — | No existe. | [16-Historial-Cambios](16-Historial-Cambios.md) |
| 17 | Evidencia del repositorio Git | ⚠️ | Párrafo «rama main con commits por funcionalidad de los dos integrantes» (§11). | **No coincide con el repositorio real**: el `.git` del zip tenía 3 commits de otro autor y un *merge* sin terminar; el repositorio de GitHub estaba vacío. | [17-Evidencia-Git](17-Evidencia-Git.md) |
| 18 | Conclusiones y trabajo futuro | ❌ | — | No existe. | [18-Conclusiones-Trabajo-Futuro](18-Conclusiones-Trabajo-Futuro.md) |

**Resumen:** 0 de 18 completos en el PDF · 11 parciales · 7 inexistentes. Todos quedan completos en esta carpeta.

## Otras diferencias encontradas entre el PDF y el código

| Tema | PDF | Código real | Acción |
|---|---|---|---|
| Pruebas automáticas | 34 pruebas | 34 en el zip | Se agregaron 32 → **66** (pendiente ejecutarlas en Visual Studio). |
| Casos de prueba manuales | «Diez casos» | Tabla con nueve (P01–P09); `docs/03-Plan-de-pruebas.md` define P01–P15 | Documentado en 15-Actualizacion-Funcionalidades. |
| Autoría en «Acerca de» | Nombres de los integrantes | Decía «[escriba aquí los nombres del equipo]» | Corregido en `SettingsPage.xaml`. |
| Pantallas | 7 (lista de cotejo) | 8 (incluye Detalle de proyecto) | Documentado. |
| Error 408 | No se menciona | No se manejaba | Agregado en `AiErrorMapper`. |
| Proveedor Pollinations | Alternativa descartada | No existía | Implementado con cambio manual en Configuración. |
| `ApplicationId` | — | `com.companyname.asistenteia` (valor de plantilla) | Recomendado cambiarlo antes de publicar (trabajo futuro). |
