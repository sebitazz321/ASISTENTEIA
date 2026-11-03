# 4. Requisitos y análisis

## 4.1 Historias de usuario y criterios de aceptación

| ID | Historia de usuario | Criterios de aceptación |
|---|---|---|
| HU01 | Como estudiante quiero escribir una pregunta y recibir una respuesta de la IA para resolver una duda. | Dado una clave válida y conexión, cuando pulso *Consultar* con 5 a 2000 caracteres, entonces veo el indicador de carga y luego la respuesta; la interfaz no se bloquea. |
| HU02 | Como estudiante quiero elegir una plantilla de prompt para formular mejor mi consulta. | Al elegir una plantilla su texto aparece en el editor y se puede modificar antes de enviar. |
| HU03 | Como estudiante quiero que la app me avise si mi consulta es inválida. | Consulta vacía o de menos de 5 caracteres: no se envía y aparece un mensaje. El contador marca máximo 2000. |
| HU04 | Como estudiante quiero cancelar una consulta lenta. | Al pulsar *Cancelar* aparece «Consulta cancelada.» y puedo consultar de nuevo. |
| HU05 | Como estudiante quiero guardar la consulta y su respuesta para volver a verlas. | *Guardar* se habilita cuando hay respuesta; el registro aparece en el historial y sigue disponible tras reiniciar. |
| HU06 | Como estudiante quiero buscar y filtrar mi historial. | La búsqueda filtra por texto, categoría o proyecto y el interruptor «Solo verificadas» muestra únicamente las verificadas. |
| HU07 | Como estudiante quiero eliminar consultas que ya no necesito. | Se pide confirmación y la lista se actualiza sin recargar la aplicación. |
| HU08 | Como estudiante quiero marcar una respuesta como verificada y anotar de dónde la verifiqué. | El detalle alterna el estado y guarda una nota de hasta 500 caracteres; los cambios persisten. |
| HU09 | Como estudiante quiero agrupar mis consultas por materia o trabajo. | Puedo crear, editar y eliminar proyectos; al eliminar un proyecto sus consultas se conservan. |
| HU10 | Como estudiante quiero consultar dentro de un proyecto para que la IA tenga contexto. | *Consultar en este proyecto* abre Consultar con el proyecto seleccionado y la consulta queda asociada. |
| HU11 | Como estudiante quiero ver mi perfil y mis estadísticas. | El perfil muestra iniciales, consultas, verificadas y proyectos; los datos se guardan solo en el dispositivo. |
| HU12 | Como estudiante quiero registrar mi clave de API sin que se muestre en pantalla y proteger mi privacidad. | La clave se escribe en un campo oculto, se guarda cifrada y solo se informa que existe; si la consulta contiene datos sensibles la app pide confirmación; puedo borrar mis datos. |
| HU13 | Como estudiante quiero elegir el proveedor de IA (Gemini o Pollinations). | En Configuración elijo el proveedor, su clave y su modelo; al guardar, las consultas usan ese proveedor y cada uno conserva su clave. |
| HU14 | Como estudiante quiero que la app no consuma mi cuota sin querer. | A la novena consulta en un minuto aparece «Límite de uso alcanzado: espere N s». |
| HU15 | Como estudiante quiero conocer la finalidad, la privacidad y los límites de la IA. | Aviso de uso responsable al primer inicio y sección «Acerca de» en Configuración. |

## 4.2 Requisitos funcionales

| Código | Requisito | Implementación | Estado |
|---|---|---|---|
| RF01 | Ingresar una pregunta o instrucción | `ChatPage` (editor con contador) | Implementado |
| RF02 | Validar contenido y longitud (5–2000) | `PromptValidator`, `ChatViewModel`, `AiService` | Implementado |
| RF03 | Enviar la consulta por POST con cuerpo JSON | `AiService.AskAsync` | Implementado |
| RF04 | Mostrar estados: carga, respuesta, error, sin conexión, cancelado | `ChatViewModel.IsBusy/StatusMessage`, `Connectivity`, `AiErrorMapper` | Implementado |
| RF05 | Guardar consulta y respuesta en SQLite | `DatabaseService.SaveAsync` | Implementado |
| RF06 | Consultar, buscar, filtrar, ver detalle y eliminar del historial | `HistoryPage`, `DetailPage` | Implementado |
| RF07 | Plantillas de prompt editables (6) | `PromptTemplate.Defaults` | Implementado |
| RF08 | Registrar, actualizar y eliminar la clave sin mostrarla, una por proveedor | `ApiKeyService` (`SecureStorage`), `Entry IsPassword` | Implementado |
| RF09 | Sección «Acerca de»: finalidad, autoría, fuentes, privacidad y límites | `SettingsPage` | Implementado |
| RF10 | Ejecutar en Windows y Android | `net9.0-windows10.0.19041.0`, `net9.0-android` | Implementado |
| RF11 | Crear, editar, listar y eliminar proyectos | `ProjectsPage`, `ProjectDetailPage` | Implementado |
| RF12 | Asociar una consulta a un proyecto y enviar su contexto a la IA | `ChatPage`, `AiService.BuildSystemPrompt` | Implementado |
| RF13 | Perfil del estudiante con estadísticas de uso | `ProfilePage`, `ProfileService` | Implementado |
| RF14 | Nota personal y marca de verificada por consulta | `DetailViewModel` | Implementado |
| RF15 | Avisar antes de enviar datos sensibles | `SensitiveDataDetector` | Implementado |
| RF16 | Limitar la frecuencia (8/min) y la longitud de la respuesta (4096 tokens) | `RequestLimiter`, `MaxOutputTokens` | Implementado |
| RF17 | Borrar el historial o todos los datos locales | `ProfileViewModel` | Implementado |
| RF18 | Elegir manualmente el proveedor de IA: Google Gemini o Pollinations | `AiProviderCatalog`, `AiService`, `SettingsViewModel` | Implementado |
| RF19 | Probar la conexión con el proveedor elegido | `SettingsViewModel.TestConnectionAsync` | Implementado |
| RF20 | Aviso de uso responsable al primer inicio | `HomeViewModel` | Implementado |

## 4.3 Requisitos no funcionales

| Código | Atributo | Requisito y cómo se atiende |
|---|---|---|
| RNF01 | Seguridad | Ninguna clave en el código ni en el repositorio; la clave viaja en un encabezado HTTPS (no en la URL) y se guarda en `SecureStorage`. |
| RNF02 | Privacidad | Aviso visible de no ingresar datos personales; historial y perfil solo en el dispositivo; sin copia de seguridad automática en Android (`allowBackup=false`). |
| RNF03 | Accesibilidad | `SemanticProperties` y niveles de encabezado; tema oscuro con contraste ≥ 4.5:1; objetivos táctiles ≥ 46 px. |
| RNF04 | Rendimiento y respuesta | Operaciones asíncronas con tiempo máximo de 60 s y cancelación; la interfaz no se bloquea. |
| RNF05 | Usabilidad | Navegación por pestañas, mensajes claros y confirmación antes de eliminar. |
| RNF06 | Mantenibilidad | MVVM con inyección de dependencias; proveedores de IA aislados en `AiProviderCatalog` y `AiService`; lógica pura con pruebas. |
| RNF07 | Portabilidad | Un solo proyecto para Windows y Android; diseño adaptable con ancho máximo y `ScrollView`. |
| RNF08 | Uso responsable | La IA se presenta como apoyo falible: avisos, verificación manual, límites de uso. |

### Contraste de colores (WCAG 2.1, paleta «Grafito»)

| Par (texto / fondo) | Contraste | Mínimo AA |
|---|---|---|
| Texto principal / fondo | 17.0:1 | 4.5:1 |
| Texto principal / tarjeta | 15.5:1 | 4.5:1 |
| Texto atenuado / tarjeta | 6.6:1 | 4.5:1 |
| Texto de botón principal / botón | 14.5:1 | 4.5:1 |
| Blanco / botón de éxito | 5.1:1 | 4.5:1 |
| Blanco / botón de peligro | 5.8:1 | 4.5:1 |
| Aviso / fondo de aviso | 9.9:1 | 4.5:1 |

## 4.4 Funcionalidades implementadas

| Módulo | Funcionalidades | Pantalla |
|---|---|---|
| Consulta | Plantillas (Pregunta libre, Explicar un concepto, Resumir, Comparar, Preguntas de práctica, Revisar código), proyecto opcional, contador 0/2000, Consultar, Cancelar, Guardar, aviso de respuesta recortada | Consultar |
| Proveedores | Google Gemini y Pollinations, modelo por proveedor, clave por proveedor, prueba de conexión | Configuración |
| Historial | Lista con fecha, categoría, proyecto y verificación; búsqueda; «Solo verificadas»; eliminar | Historial, Detalle |
| Detalle | Respuesta completa, verificar/quitar verificación, nota personal, eliminar | Detalle de consulta |
| Proyectos | Crear, editar, eliminar (conserva consultas), contador de consultas, consultar en el proyecto | Proyectos, Detalle de proyecto |
| Perfil | Nombre, programa, institución, iniciales, estadísticas, borrar historial, borrar todos los datos | Perfil |
| Seguridad y privacidad | SecureStorage, campo oculto, detector de datos sensibles, límite de 8 consultas/min, aviso de uso responsable | Varias |
| Errores | Mensajes para validación, sin red, sin clave, límite de uso, 400, 401, 402, 403, 404, 408, 429, 5xx, tiempo agotado y cancelación | Consultar, Configuración |
