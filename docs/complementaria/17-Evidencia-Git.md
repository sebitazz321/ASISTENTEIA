# 17. Evidencia del repositorio Git

## 17.1 Estado encontrado (antes de esta actualización)

| Elemento | Hallazgo |
|---|---|
| Repositorio en GitHub `sebitazz321/ASISTENTEIA` | **Vacío** (sin commits). |
| Carpeta `.git` dentro del zip | 3 commits del 22/10/2026, autor «Juan Rodriguez», *remote* `https://github.com/JDRODRIGUEZ037/AsistenteIA.git`. |
| Ramas | `main` local (`20b0bb9`) y `origin/main` (`5844a08`) habían divergido y había un ***merge* sin terminar** (`MERGE_HEAD`). |
| Cambios sin versionar | 60 archivos modificados, nuevos o eliminados (todo el trabajo de mejora). |
| Coincidencia con el documento (§11) | **No coincide**: el documento afirma «commits descriptivos por funcionalidad realizados por los dos integrantes». |

## 17.2 Estado actual

- **Remoto:** `https://github.com/sebitazz321/ASISTENTEIA` · **rama:** `main`.
- Se conservó el historial original (no se reescribió) y se agregaron commits descriptivos:

```
*   (docs)    Documentación complementaria: 18 entregables y comparación   ← este commit
*   76bd256   Agrega 32 pruebas de proveedores y manejo de errores
*   fb45b32   Agrega proveedor Pollinations y cambio manual de proveedor de IA
*   17ab990   Integra la versión mejorada del proyecto (AsistenteIA_mejorado)   (merge de 20b0bb9 y 5844a08)
|\
| * 5844a08   Agregar archivos de proyecto.
* | 20b0bb9   Trabajo completo Agregar archivos de proyecto.
|/
*   4fc6329   Agregar .gitattributes y .gitignore.
```

## 17.3 Organización del repositorio

| Ruta | Contenido |
|---|---|
| `AsistenteIA/` | Aplicación .NET MAUI |
| `AsistenteIA.Tests/` | Pruebas xUnit |
| `docs/01…07` | Documentación original del equipo (Markdown) |
| `docs/complementaria/` | Los 18 entregables, diagramas (PNG + Mermaid), comparación, matriz Excel y documento Word/PDF consolidado |
| `.gitignore` | Excluye `bin/`, `obj/`, `.vs/`, `*.user` |

## 17.4 Flujo recomendado para el resto del proyecto

1. Cada integrante configura su identidad: `git config user.name "Nombre Apellido"` y `git config user.email "correo"`.
2. Trabajo en ramas cortas: `feature/…`, `fix/…`, `docs/…`, con *Pull Request* hacia `main`.
3. Un commit por cambio lógico, en imperativo y explicando el porqué.
4. Comandos útiles para la evidencia:

```bash
git log --graph --oneline --all          # gráfico de ramas y commits
git shortlog -sne                        # commits por autor
git log --stat --since="2026-10-01"      # archivos afectados por commit
```
