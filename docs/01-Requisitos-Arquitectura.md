# 1. Requisitos, arquitectura y decisiones técnicas

## 1.1 Problema delimitado

Los estudiantes usan asistentes de IA de forma dispersa: sin organización por materia, sin registro de lo
consultado, sin verificar las respuestas y, a veces, compartiendo datos personales con el proveedor.

**Alcance:** una app móvil/escritorio que (1) envía consultas académicas a un modelo de IA por REST,
(2) guarda las respuestas localmente y las agrupa en proyectos, (3) obliga a un flujo de verificación y
(4) protege la clave y la privacidad del estudiante.

**Fuera de alcance:** cuentas de usuario en un servidor, sincronización en la nube, conversaciones multi-turno,
carga de archivos, generación de imágenes.

## 1.2 Requisitos funcionales

| Código | Requisito | Implementación |
|---|---|---|
| RF01 | Escribir una consulta académica | `ChatPage` (Editor) |
| RF02 | Validar la consulta: no vacía, 5–2000 caracteres | `PromptValidator`, usado por `ChatViewModel` y `AiService` |
| RF03 | Enviar la consulta por POST con cuerpo JSON | `AiService.AskAsync` |
| RF04 | Mostrar estados: cargando, respuesta, error, sin conexión, cancelado | `ChatViewModel.IsBusy/StatusMessage`, `Connectivity` |
| RF05 | Guardar la respuesta en SQLite | `ChatViewModel.SaveAsync` → `DatabaseService.SaveAsync` |
| RF06 | Historial: listar, buscar, filtrar «verificadas», ver detalle, eliminar con confirmación | `HistoryPage`, `DetailPage` |
| RF07 | Plantillas de prompt editables | `PromptTemplate.Defaults` + `Picker` |
| RF08 | Guardar la clave de API de forma segura y nunca volver a mostrarla | `ApiKeyService` (`SecureStorage`), `Entry IsPassword` |
| RF09 | Pantalla «Acerca de»: finalidad, autoría, fuentes, privacidad y límites | `SettingsPage` |
| RF10 | Ejecutar en Windows y Android | `net9.0-windows10.0.19041.0`, `net9.0-android` |
| RF11 | Crear, editar, listar y eliminar **proyectos** | `ProjectsPage`, `ProjectDetailPage` |
| RF12 | Asociar una consulta a un proyecto; el proyecto da contexto a la IA | `ChatPage` (selector), `AiService.BuildSystemPrompt` |
| RF13 | **Perfil** del estudiante con estadísticas de uso | `ProfilePage`, `ProfileService` |
| RF14 | Editar una **nota personal** y marcar la consulta como verificada | `DetailPage`, `DetailViewModel` |
| RF15 | Avisar antes de enviar datos sensibles (correo, tarjeta, teléfono, credenciales) | `SensitiveDataDetector` |
| RF16 | Limitar la frecuencia de consultas (8 por minuto) y la longitud de la respuesta | `RequestLimiter`, `GenerationConfig.MaxOutputTokens` |
| RF17 | Borrar el historial o todos los datos locales | `ProfileViewModel` |

## 1.3 Requisitos no funcionales

| Código | Categoría | Requisito | Cómo se verifica |
|---|---|---|---|
| RNF01 | Seguridad | La clave se guarda cifrada (`SecureStorage`), viaja en encabezado HTTPS y nunca se registra ni se muestra | Revisión de código; P04 |
| RNF02 | Privacidad | Historial y perfil solo en el dispositivo; aviso previo al enviar datos al proveedor; sin copia de seguridad automática en Android (`allowBackup=false`) | Matriz de riesgos |
| RNF03 | Usabilidad / accesibilidad | Contraste ≥ 4.5:1, objetivos táctiles ≥ 46 px, etiquetas visibles, `SemanticProperties` en controles | Tabla de contraste en 06; P10 |
| RNF04 | Rendimiento | Tiempo de espera de 60 s; la interfaz nunca se bloquea (`async/await`) | P05, P06 |
| RNF05 | Mantenibilidad | MVVM, inyección de dependencias, lógica pura separada y con pruebas unitarias | `AsistenteIA.Tests` |
| RNF06 | Portabilidad | Un solo proyecto para Windows y Android; el diseño se adapta con ancho máximo y `ScrollView` | P10 + capturas |
| RNF07 | Uso responsable | La IA se presenta como apoyo falible: avisos, marca «verificada» manual, límites | Matriz de riesgos |

## 1.4 Historias de usuario y criterios de aceptación

| # | Historia | Criterios de aceptación (verificables) |
|---|---|---|
| HU01 | Como estudiante quiero escribir una pregunta y recibir una respuesta para estudiar. | **Dado** una clave válida y conexión, **cuando** pulso *Consultar* con ≥ 5 caracteres, **entonces** veo un indicador de carga y luego la respuesta. |
| HU02 | Como estudiante quiero que la app me avise si mi consulta es inválida. | Consulta vacía o < 5 caracteres → no se envía y aparece un mensaje. El contador marca máximo 2000. |
| HU03 | Como estudiante quiero cancelar una consulta lenta. | **Cuando** pulso *Cancelar* durante la carga, **entonces** aparece «Consulta cancelada.» y puedo consultar de nuevo. |
| HU04 | Como estudiante quiero guardar las respuestas útiles. | *Guardar* está deshabilitado hasta que hay respuesta; al guardar aparece en *Historial*. |
| HU05 | Como estudiante quiero buscar y filtrar mi historial. | Escribir en el buscador filtra por texto, categoría o proyecto; el interruptor muestra solo las verificadas. |
| HU06 | Como estudiante quiero marcar una respuesta como verificada solo cuando la contrasté. | En *Detalle*, el botón alterna verificada/no verificada y el cambio persiste al reiniciar. |
| HU07 | Como estudiante quiero anotar de dónde verifiqué la respuesta. | La nota (≤ 500 caracteres) se guarda y se ve al volver a abrir la consulta. |
| HU08 | Como estudiante quiero organizar mis consultas en proyectos. | Puedo crear, editar y eliminar proyectos; la lista muestra cuántas consultas tiene cada uno; al eliminar un proyecto sus consultas se conservan. |
| HU09 | Como estudiante quiero consultar dentro de un proyecto para que la IA tenga contexto. | Desde el proyecto, *Consultar en este proyecto* abre *Consultar* con ese proyecto seleccionado; la consulta guardada queda asociada. |
| HU10 | Como estudiante quiero ver mi perfil y mis estadísticas. | El perfil muestra iniciales, número de consultas, verificadas y proyectos; mis datos se guardan al pulsar *Guardar perfil*. |
| HU11 | Como estudiante quiero proteger mi clave y mi privacidad. | La clave nunca se muestra; si la consulta contiene datos sensibles la app pide confirmación; puedo borrar mis datos. |
| HU12 | Como estudiante quiero que la app no me deje consumir mi cuota sin querer. | A la consulta 9 en un minuto aparece «Límite de uso alcanzado: espere N s». |

## 1.5 Arquitectura

```
┌────────────────────────── Views (XAML, sin lógica) ──────────────────────────┐
│ HomePage  ChatPage  ProjectsPage  HistoryPage  ProfilePage                   │  ← pestañas
│ DetailPage  ProjectDetailPage  SettingsPage                                  │  ← rutas secundarias
└───────────────▲──────────────────────────────────────────────────────────────┘
                │ Binding / Command (CommunityToolkit.Mvvm)
┌───────────────┴─────────────── ViewModels ───────────────────────────────────┐
│ Home  Chat  Projects  ProjectDetail  History  Detail  Profile  Settings      │
└───────────────▲──────────────────────────────────────────────────────────────┘
                │ constructor injection (Microsoft.Extensions.DependencyInjection)
┌───────────────┴──────────────── Services ────────────────────────────────────┐
│ AiService ── HttpClient ──▶ API REST de IA (Gemini)                          │
│ DatabaseService ──▶ SQLite     ApiKeyService ──▶ SecureStorage               │
│ ProfileService ──▶ Preferences                                               │
│ INavigationService / IDialogService (Shell)                                  │
│ PromptValidator · SensitiveDataDetector · RequestLimiter  (lógica pura)      │
└───────────────▲──────────────────────────────────────────────────────────────┘
                │
┌───────────────┴──────────────── Models ──────────────────────────────────────┐
│ ConversationRecord (tabla)  Project (tabla)  UserProfile  PromptTemplate     │
│ ChatModels (JSON de la API)                                                  │
└──────────────────────────────────────────────────────────────────────────────┘
```

**Flujo principal:** `ChatPage` → `ChatViewModel.AskCommand` → `PromptValidator` → `SensitiveDataDetector`
(confirmación) → `AiService.AskAsync` (límite de uso → POST JSON) → respuesta → *Guardar* →
`DatabaseService.SaveAsync` → `HistoryViewModel` / `ProjectDetailViewModel`.

### Mapa de pantallas

| # | Pantalla | Tipo | Función |
|---|---|---|---|
| 1 | Inicio | Pestaña | Saludo, accesos rápidos, recomendaciones; aviso de uso responsable la primera vez |
| 2 | Consultar | Pestaña | Plantilla, proyecto, consulta, respuesta, guardar/cancelar |
| 3 | Proyectos | Pestaña | Lista de proyectos y botón para crear |
| 4 | Historial | Pestaña | Búsqueda, filtro de verificadas, eliminar |
| 5 | Perfil | Pestaña | Datos personales, estadísticas, privacidad |
| 6 | Detalle de consulta | Secundaria | Respuesta completa, nota, verificar, eliminar |
| 7 | Detalle de proyecto | Secundaria | Crear/editar/eliminar proyecto, sus consultas |
| 8 | Configuración y Acerca de | Secundaria | Clave, modelo, probar conexión, autoría, privacidad, límites |

## 1.6 Decisiones técnicas

| Decisión | Alternativas | Razón |
|---|---|---|
| **.NET MAUI + Shell** | Xamarin, WPF | Requisito de la asignatura; un proyecto para Windows y Android; Shell da rutas y pestañas |
| **CommunityToolkit.Mvvm** | `INotifyPropertyChanged` manual, Prism | Los generadores `[ObservableProperty]`/`[RelayCommand]` eliminan código repetitivo y facilitan la sustentación |
| **Inyección con `Microsoft.Extensions.DependencyInjection`** | `new` en cada ViewModel | Desacopla y permite sustituir servicios. *Singleton*: servicios con estado compartido (`HttpClient`, límite de uso). *Transient*: ViewModels y páginas, para empezar limpios |
| **`INavigationService` / `IDialogService`** | Llamar a `Shell.Current` desde el ViewModel | Los ViewModels no dependen de la interfaz y son más fáciles de probar |
| **Un único `HttpClient` reutilizable** | Uno por solicitud | Evita agotar sockets; el `BaseAddress` y el *timeout* están en un solo lugar |
| **Gemini `generateContent`** | Pollinations (propuesto en la guía) | Requiere clave (permite demostrar autenticación) y devuelve errores HTTP bien definidos. El endpoint es una constante y el modelo es configurable |
| **Clave en encabezado `x-goog-api-key`** | Parámetro `?key=` en la URL | Las URL se registran en historiales y proxies; los encabezados no |
| **`SecureStorage` para la clave; `Preferences` para el perfil** | Todo en SQLite | La clave es un secreto (almacén cifrado del sistema); el perfil no lo es |
| **sqlite-net-pcl** | Entity Framework Core | Ligero, asíncrono y suficiente para dos tablas; `CreateTableAsync` migra columnas nuevas |
| **Límite de uso en el cliente** | Confiar solo en el proveedor | Protege la cuota de la clave del estudiante y evita ráfagas accidentales |
| **Detección de datos sensibles (heurística)** | Bloquear siempre | Avisa y deja decidir al usuario; no pretende ser infalible |
| **Tema único gris oscuro (“Grafito”)** | Tema claro/oscuro del sistema | Reduce el alcance, asegura el contraste en todas las pantallas |
| **Lógica pura en clases estáticas sin MAUI** | Dentro de los ViewModels | Se prueba con xUnit sin emulador |
