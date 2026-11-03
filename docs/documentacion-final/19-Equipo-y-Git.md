# 19. Trabajo en equipo y repositorio Git

## 19.1 Roles

| Integrante | Rol principal | Aportes |
|---|---|---|
| Juan Carlos Díaz Cifuentes | Servicios y datos; calidad y documentación | API REST, almacenamiento seguro de la clave, SQLite, pruebas y documentación. |
| Sebastián Grande Moreno | Líder e integración; interfaz y accesibilidad | Navegación, pantallas XAML, estilos, integración de la solución, proveedores de IA y repositorio. |

Los roles siguen la organización propuesta por la guía: líder e integración; interfaz y accesibilidad; servicios y
datos; calidad y documentación. Ambos integrantes revisaron el código y comprenden el recorrido completo de la
aplicación.

## 19.2 Repositorio

| Elemento | Valor |
|---|---|
| URL | <https://github.com/sebitazz321/ASISTENTEIA> |
| Rama principal | `main` |
| Exclusiones (`.gitignore`) | `bin/`, `obj/`, `.vs/`, `*.user` |

| Ruta | Contenido |
|---|---|
| `AsistenteIA/` | Aplicación .NET MAUI |
| `AsistenteIA.Tests/` | Pruebas xUnit |
| `docs/01…07` | Documentación de trabajo del equipo (Markdown) |
| `docs/documentacion-final/` | Este documento: capítulos en Markdown, diagramas (PNG y Mermaid), capturas y versión consolidada en Word y PDF |

## 19.3 Historial de commits

```
*   (documentación final)
*   ad5cbe7   Documentación complementaria: 18 entregables
*   76bd256   Agrega 32 pruebas de proveedores y manejo de errores
*   fb45b32   Agrega proveedor Pollinations y cambio manual de proveedor de IA
*   17ab990   Integra la versión mejorada del proyecto (merge de 20b0bb9 y 5844a08)
|\
| * 5844a08   Agregar archivos de proyecto.
* | 20b0bb9   Trabajo completo Agregar archivos de proyecto.
|/
*   4fc6329   Agregar .gitattributes y .gitignore.
```

Los tres primeros commits (22/10/2026) corresponden a la base inicial del proyecto y conservan el autor que
registró Git en ese momento. Los commits siguientes (03/11/2026) contienen la versión mejorada, la integración de
Pollinations, las pruebas y la documentación.

## 19.4 Flujo de trabajo

1. Cada integrante configura su identidad antes de trabajar:
   `git config user.name "Nombre Apellido"` y `git config user.email "correo"`.
2. Trabajo en ramas cortas (`feature/…`, `fix/…`, `docs/…`) con *Pull Request* hacia `main`.
3. Un commit por cambio lógico, en imperativo y explicando el porqué.
4. Comandos para revisar la evidencia:

```bash
git log --graph --oneline --all          # gráfico de ramas y commits
git shortlog -sne                        # commits por autor
git log --stat --since="2026-10-01"      # archivos afectados por commit
```
