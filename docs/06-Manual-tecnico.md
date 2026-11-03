# 6. Manual técnico breve

## 6.1 Requisitos de desarrollo

- Visual Studio 2022 17.12+ (o posterior) con la carga de trabajo **Desarrollo de la interfaz de usuario de aplicaciones multiplataforma de .NET**.
- SDK .NET 9 (`global.json` fija 9.0.100 con *rollForward*).
- Para Android: emulador o dispositivo con API 21+. Para Windows: Windows 10 (1809) o superior.

## 6.2 Ejecutar

1. Abrir `AsistenteIA.slnx`.
2. Elegir **Windows Machine** o un dispositivo Android y ejecutar el proyecto `AsistenteIA`.
3. En *Perfil › Configuración* guardar la clave propia (no está en el repositorio) y pulsar *Probar conexión*.
4. Pruebas: `dotnet test AsistenteIA.Tests`.

## 6.3 Estructura

```
AsistenteIA/
├─ AsistenteIA/                 proyecto MAUI
│  ├─ Models/                   ConversationRecord, Project, UserProfile, PromptTemplate, ChatModels (JSON)
│  ├─ Services/                 AiService, DatabaseService, ApiKeyService, ProfileService,
│  │                            INavigationService, IDialogService,
│  │                            PromptValidator, SensitiveDataDetector, RequestLimiter
│  ├─ ViewModels/               Home, Chat, Projects, ProjectDetail, History, Detail, Profile, Settings
│  ├─ Views/                    8 páginas XAML (+ code-behind mínimo)
│  ├─ Resources/Styles/         Colors.xaml (paleta), Styles.xaml (estilos)
│  ├─ AppShell.xaml(.cs)        pestañas y rutas
│  └─ MauiProgram.cs            registro de dependencias
├─ AsistenteIA.Tests/           xUnit (enlaza la lógica pura)
└─ docs/
```

## 6.4 Inyección de dependencias (`MauiProgram.cs`)

| Registro | Vida | Motivo |
|---|---|---|
| `HttpClient` (con `BaseAddress` y *timeout*) | Singleton | Reutilizar conexiones |
| `RequestLimiter`, `ApiKeyService`, `AiService`, `DatabaseService`, `ProfileService` | Singleton | Estado compartido (conexión SQLite, ventana del límite) |
| `INavigationService → ShellNavigationService`, `IDialogService → ShellDialogService` | Singleton | Abstracción de la UI |
| ViewModels y Páginas | Transient | Estado limpio; las páginas reciben su ViewModel por constructor |

Las páginas de pestañas se crean una vez (`ShellContent`), por eso `ChatPage`, `HistoryPage`, `ProjectsPage`,
`ProfilePage` y `HomePage` recargan datos en `OnAppearing`.

## 6.5 Navegación

- Pestañas: `//HomePage`, `//ChatPage`, `//ProjectsPage`, `//HistoryPage`, `//ProfilePage`.
- Rutas secundarias registradas en `AppShell.xaml.cs`: `DetailPage`, `ProjectDetailPage`, `SettingsPage`.
- Parámetros: `Record` y `Project` (objetos, vía diccionario) y `projectId` (texto en la ruta `//ChatPage?projectId=3`),
  recibidos con `[QueryProperty]`.

## 6.6 Base de datos (SQLite, archivo `AsistenteIA.db3` en `FileSystem.AppDataDirectory`)

| Tabla | Columnas |
|---|---|
| `ConversationRecord` | `Id` (PK, autoincremental), `Prompt`, `Answer`, `CreatedAt` (UTC), `Category`, `IsVerified`, `ProjectId` (0 = sin proyecto), `Note` |
| `Project` | `Id` (PK, autoincremental), `Name`, `Description`, `CreatedAt` |

| Operación | Método |
|---|---|
| **C**rear | `SaveAsync` / `SaveProjectAsync` con `Id == 0` (INSERT) |
| **R**ead | `GetItemsAsync`, `GetItemsByProjectAsync`, `GetProjectsAsync`, `GetStatsAsync` |
| **U**pdate | `SaveAsync` / `SaveProjectAsync` con `Id != 0` (verificar, nota, editar proyecto) |
| **D**elete | `DeleteAsync`, `DeleteProjectAsync` (desasocia consultas), `DeleteAllItemsAsync`, `DeleteAllProjectsAsync` |

`CreateTableAsync` agrega las columnas nuevas (`ProjectId`, `Note`) a una base creada con la versión anterior.

## 6.7 Cambiar de proveedor o modelo

- Modelo: *Configuración › Modelo de IA* (se guarda en `Preferences`).
- Proveedor: cambiar `AiService.BaseUrl`, la ruta en `AskAsync` y los modelos de `Models/ChatModels.cs`
  (el resto de la app solo conoce el `string` de la respuesta).

## 6.8 Diseño visual y accesibilidad

Paleta «Grafito» (`Resources/Styles/Colors.xaml`). Contraste calculado con la fórmula WCAG 2.1:

| Par (texto / fondo) | Contraste | Mínimo AA |
|---|---|---|
| Texto principal / fondo | 17.0:1 | 4.5:1 |
| Texto principal / tarjeta | 15.5:1 | 4.5:1 |
| Texto atenuado / tarjeta | 6.6:1 | 4.5:1 |
| Texto de botón principal / botón | 14.5:1 | 4.5:1 |
| Blanco / botón de éxito | 5.1:1 | 4.5:1 |
| Blanco / botón de peligro | 5.8:1 | 4.5:1 |
| Aviso / fondo de aviso | 9.9:1 | 4.5:1 |

Otras medidas: el estado nunca se comunica solo con color (siempre hay texto: «✔ Verificada», mensajes);
botones y campos con 46 de alto mínimo; `SemanticProperties.Description/Hint/HeadingLevel` en los controles
importantes; formularios dentro de `ScrollView` (teclado en pantallas pequeñas); el contenido se centra con un
ancho máximo de 760 (`ContentMaxWidth`) para ventanas anchas de Windows y ocupa todo el ancho en el teléfono.

## 6.9 Problemas conocidos / límites

- Los identificadores de modelos cambian con el tiempo; si aparece 404, elija otro en Configuración.
- `SecureStorage` en Windows sin empaquetar puede requerir permisos; si falla, la app muestra «No se pudo leer el almacén seguro».
- El detector de datos sensibles es heurístico.
