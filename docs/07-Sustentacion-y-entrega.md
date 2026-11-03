# 7. Entrega, evidencias y sustentación

## 7.1 Lista de evidencias finales

| Evidencia | Estado | Dónde / cómo |
|---|---|---|
| Repositorio Git con ramas o commits descriptivos de **todos** los integrantes | ☐ | Ver 7.2 |
| Proyecto ejecutable y código fuente **sin** `bin` y `obj` | ☐ | Ver 7.3 |
| Documento de requisitos, arquitectura y decisiones técnicas | ✔ | `01-Requisitos-Arquitectura.md` |
| Cinco pantallas y evidencia de adaptación en el dispositivo seleccionado | ☐ capturas | Ver 7.4 (la app tiene 8) |
| Consumo REST documentado con solicitud, respuesta y códigos HTTP | ✔ (anexar capturas) | `02-Consumo-REST.md` |
| Historial SQLite con crear, consultar, actualizar y eliminar | ✔ código | `06-Manual-tecnico.md §6.6`, casos P07–P09, P13, P14 |
| Tabla de diez casos de prueba diligenciada | ☐ ejecutar | `03-Plan-de-pruebas.md` |
| Matriz de riesgos de privacidad, seguridad y uso responsable | ✔ | `04-Matriz-de-riesgos.md` |
| Manual de usuario y manual técnico breve | ✔ | `05-…`, `06-…` |
| Video de funcionamiento y sustentación individual del código | ☐ | Ver 7.5 y 7.6 |

## 7.2 Flujo de Git sugerido (para que se vea la autoría de cada integrante)

- `main` solo recibe cambios por *merge* de ramas cortas: `feature/proyectos-ui`, `feature/perfil`, `feature/seguridad`,
  `feature/pruebas`, `docs/requisitos`, `fix/…`.
- Cada integrante configura su identidad antes de trabajar: `git config user.name "Nombre Apellido"` y
  `git config user.email "correo"`.
- Mensajes de commit descriptivos, en imperativo y con el *qué* y el *por qué*, por ejemplo:
  - `Agregar detector de datos sensibles antes de enviar la consulta`
  - `Corregir contador de caracteres que no se actualizaba al elegir plantilla`
- Un commit por cambio lógico (no un solo commit con todo). Ejemplo de reparto de este trabajo:
  1. `Aplicar tema gris Grafito a colores y estilos`
  2. `Agregar modelo y CRUD de proyectos en SQLite`
  3. `Agregar pantallas Proyectos, Detalle de proyecto y Perfil`
  4. `Abstraer navegación y diálogos con interfaces inyectables`
  5. `Agregar límite de uso y detección de datos sensibles`
  6. `Agregar pruebas unitarias xUnit`
  7. `Agregar documentación y matriz de riesgos`
- La propia carpeta `.git` se entrega dentro del `.zip`.

## 7.3 Entregar sin `bin` ni `obj`

El `.gitignore` ya los excluye del repositorio. Para el `.zip`, desde la carpeta que contiene `AsistenteIA.slnx`:

```bash
git archive --format=zip --output=../AsistenteIA-entrega.zip HEAD
```

(Incluye solo lo versionado; si necesita incluir `.git`, comprima la carpeta y borre antes las carpetas `bin` y `obj` de ambos proyectos.)

## 7.4 Capturas de pantalla (adaptación en el dispositivo)

Tome las capturas en el **dispositivo elegido** (anótelo en el plan de pruebas) y guárdelas en `evidencias/`:

| Archivo sugerido | Pantalla |
|---|---|
| `01-inicio.png`, `02-consultar-respuesta.png`, `03-proyectos.png`, `04-historial.png`, `05-perfil.png` | Las cinco pestañas |
| `06-detalle-proyecto.png`, `07-detalle-consulta.png`, `08-configuracion.png` | Pantallas secundarias |
| `09-aviso-datos-sensibles.png` | Diálogo de privacidad |
| `10-adaptacion-*.png` | Misma pantalla en **dos tamaños** (teléfono vertical y horizontal, o ventana de Windows ancha y estrecha) |

## 7.5 Guion sugerido para el video (5–6 min)

1. (0:30) Presentación del equipo y el problema.
2. (0:30) Primer uso: aviso de uso responsable y guardado de la clave (sin mostrarla).
3. (1:30) Consulta: plantilla, proyecto, respuesta, cancelación y un error (clave inválida o sin red).
4. (0:30) Aviso de datos sensibles al escribir un correo.
5. (1:00) Guardar → Historial → Detalle: nota, verificar, eliminar con confirmación; reiniciar la app y mostrar que persiste.
6. (0:45) Proyectos y Perfil: crear proyecto, consultar dentro de él, estadísticas.
7. (0:45) Código: `AiService`, un ViewModel, `MauiProgram`, `AsistenteIA.Tests` con `dotnet test`.

## 7.6 Preguntas probables de sustentación (cada integrante debe poder responderlas)

| Pregunta | Respuesta corta (ubique el código) |
|---|---|
| ¿Qué es MVVM y cómo se ve aquí? | La vista (XAML) solo enlaza propiedades/comandos; el ViewModel tiene el estado y la lógica de presentación; los servicios hablan con la API o la base. Ej.: `ChatPage.xaml` ↔ `ChatViewModel` ↔ `AiService`. |
| ¿Qué hace `[ObservableProperty]`? | Genera la propiedad pública y la notificación `PropertyChanged` desde un campo privado (CommunityToolkit.Mvvm). |
| ¿Y `[RelayCommand]`? | Genera un `ICommand` (`AskCommand`) a partir del método `AskAsync`; soporta `async` y `CanExecute`. |
| ¿Por qué inyección de dependencias? | Para no crear servicios con `new`, reutilizar instancias y poder sustituirlas. Se registra en `MauiProgram.cs`. |
| ¿Singleton vs Transient? | Singleton: una instancia para toda la app (HttpClient, base de datos). Transient: una nueva cada vez (ViewModels, páginas). |
| ¿Por qué `HttpClient` único? | Crear uno por solicitud agota sockets y desperdicia conexiones. |
| ¿Cómo se cancela una consulta? | `CancellationTokenSource` en `ChatViewModel`; `Cancel()` lo cancela y el token viaja a `SendAsync`. |
| ¿Por qué la clave va en un encabezado y en `SecureStorage`? | Las URL quedan en registros; `SecureStorage` usa el almacén cifrado del sistema (Keystore/DPAPI). |
| ¿Cómo se actualiza un registro en SQLite? | `SaveAsync`: si `Id != 0` llama a `UpdateAsync`, si no `InsertAsync`. |
| ¿Cómo se agregó `ProjectId` sin perder datos? | `CreateTableAsync` detecta columnas nuevas y las agrega. |
| ¿Cómo funciona el detector de datos sensibles? | Expresiones regulares + algoritmo de Luhn para tarjetas; avisa y deja decidir. |
| ¿Cómo se limita el uso? | `RequestLimiter`: cola de marcas de tiempo; ventana de 60 s y máximo 8. |
| ¿Cómo se adapta a pantallas pequeñas o grandes? | `ScrollView`, `Grid` con columnas `*` y `MaximumWidthRequest` de 760 centrado. |
| ¿Qué es `Shell` y cómo se navega? | Contenedor de navegación; `GoToAsync("//ChatPage")` a una pestaña, `GoToAsync(nameof(DetailPage), parámetros)` a una ruta secundaria. |
| ¿Por qué `INavigationService`? | El ViewModel no conoce `Shell`; facilita pruebas y respeta MVVM. |

## 7.7 Declaración de uso de IA

Se utilizó un asistente de IA (Claude) como apoyo para depurar la compilación, completar código, rediseñar la
interfaz y redactar borradores de la documentación. El equipo revisó el código, lo ejecutó y puede explicarlo.

**Prompts y decisiones propias del equipo:** _(completar: qué pidieron, qué cambiaron o rechazaron, qué corrigieron a mano)_

**Fuentes técnicas consultadas:** Microsoft Learn (.NET MAUI, `HttpClient`, inyección de dependencias,
`SecureStorage`), documentación de CommunityToolkit.Mvvm, documentación de sqlite-net y de la API de Google Gemini.
