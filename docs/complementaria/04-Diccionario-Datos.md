# 04. Diccionario de datos

> Las longitudes las valida la aplicación (ViewModels y `PromptValidator`); en SQLite las columnas `VARCHAR` no tienen límite.
> `DateTime` se guarda como *ticks* (`BIGINT`), que es el comportamiento por defecto de sqlite-net.

## Tabla `ConversationRecord` (historial)

| Campo | Tipo C# | Tipo SQLite | Restricciones | Valor por defecto | Función |
|---|---|---|---|---|---|
| `Id` | `int` | INTEGER | PK, autoincremental, no nulo | asignado por SQLite | Identificador. `0` = registro nuevo (insertar). |
| `Prompt` | `string` | VARCHAR | 5–2000 caracteres (`PromptValidator`) | `""` | Consulta enviada a la IA. |
| `Answer` | `string` | VARCHAR | — | `""` | Respuesta recibida. |
| `CreatedAt` | `DateTime` | BIGINT | UTC | `DateTime.UtcNow` | Fecha; se muestra en hora local (`dd/MM/yyyy HH:mm`). |
| `Category` | `string` | VARCHAR | Categoría de la plantilla | `"General"` | Explicación, Resumen, Comparación, Estudio, Programación… |
| `IsVerified` | `bool` | INTEGER (0/1) | — | `false` | El estudiante contrastó la respuesta. |
| `ProjectId` | `int` | INTEGER | FK lógica a `Project.Id`; `0` = sin proyecto | `0` | Agrupa la consulta. |
| `Note` | `string` | VARCHAR | máx. 500 caracteres | `""` | Nota personal editable en el detalle. |
| `ProjectName`, `CreatedAtLocal`, `VerificationText`, `MetaText` | `string` | — (`[Ignore]`) | No se guardan | — | Solo presentación. |

## Tabla `Project`

| Campo | Tipo C# | Tipo SQLite | Restricciones | Valor por defecto | Función |
|---|---|---|---|---|---|
| `Id` | `int` | INTEGER | PK, autoincremental | asignado por SQLite | Identificador. |
| `Name` | `string` | VARCHAR | 3–60 caracteres, obligatorio | `""` | Nombre del proyecto. |
| `Description` | `string` | VARCHAR | máx. 300 caracteres | `""` | Contexto que se envía a la IA (máx. 400 caracteres incluido el nombre). |
| `CreatedAt` | `DateTime` | BIGINT | UTC | `DateTime.UtcNow` | Fecha de creación. |
| `ConversationCount`, `CountText`, `DescriptionText` | — | — (`[Ignore]`) | No se guardan | — | Presentación. |

## `Preferences` (preferencias del dispositivo, sin cifrar)

| Clave | Tipo | Ejemplo | Función |
|---|---|---|---|
| `SelectedProvider` | string | `Gemini` / `Pollinations` | Proveedor activo (cambio manual). |
| `SelectedModel` | string | `gemini-3.5-flash-lite` | Modelo de Gemini. |
| `SelectedModel_Pollinations` | string | `openai` | Modelo de Pollinations. |
| `profile_name`, `profile_program`, `profile_institution` | string (máx. 60) | — | Perfil del estudiante (no se envía a la IA). |
| `responsible_use_notice_accepted` | bool | `true` | El aviso de uso responsable ya se mostró. |

## `SecureStorage` (cifrado por el sistema operativo)

| Clave | Contenido |
|---|---|
| `ai_api_key` | Clave de Google Gemini (nombre original, conserva claves guardadas antes). |
| `pollinations_api_key` | Clave de Pollinations. |
