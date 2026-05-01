# Skills SddIA — Definición vs implementación

Este directorio es **paths.skillsDefinitionPath** (Cúmulo, `SddIA/agents/cumulo.json`). Separa la **capa de definición (SddIA)** de la **capa de implementación (scripts)** para desacoplar la arquitectura IA de los artefactos técnicos.

## Estructura

| Ubicación | Contenido | Propósito |
|-----------|-----------|-----------|
| **SddIA/skills/** (este directorio) | Contrato global: `skills-contract.json`, `skills-contract.md`. Por skill: subcarpeta **&lt;skill-id&gt;/** con `spec.md` y `spec.json`. | **Definición:** qué hace el skill, contrato, entradas/salidas, flujo. Consumido por agentes y procesos. |
| **scripts/skills/** | Índice `index.json` y, por skill con ejecutable, cápsula **&lt;skill-id&gt;/** (manifest, .ps1, .bat, doc, bin/). | **Implementación:** código, launchers y ejecutables. Ruta canónica: **paths.skillCapsules[&lt;skill-id&gt;]** (Cúmulo). |

La **raíz del path de implementación** la indica Cúmulo (**paths.skillsPath**, **paths.skillCapsules**). No se usan rutas literales en la definición.

## Definición por skill (SddIA/skills/&lt;skill-id&gt;/)

Cada skill tiene en este directorio una carpeta con:

- **spec.md** — Especificación legible: objetivo, entradas, salidas, flujo, reglas. Idioma: es-ES.
- **spec.json** — Metadatos machine-readable. Si el skill tiene implementación ejecutable, debe incluir **implementation_path_ref**: referencia a la ruta de implementación en Cúmulo (ej. `paths.skillCapsules.<skill-id>`).

## Implementación por defecto: Rust

**Las implementaciones por defecto de los scripts de skills (igual que en tools) han de ser en Rust.** Los ejecutables se construyen en paths.skillsRustPath (Cúmulo) y se copian a cada cápsula `bin/`. Launcher `.bat` en la cápsula invoca el `.exe` en `bin/` si existe; si no, fallback al script `.ps1`.

## Listado de skills

| skill_id | Descripción breve | Cápsula (paths.skillCapsules) |
| :--- | :--- | :--- |
| git-workspace-recon | Inspección del workspace Git (rama, porcelana, ahead/behind). | git-workspace-recon |
| git-branch-manager | Crear, checkout o eliminar ramas. | git-branch-manager |
| git-save-snapshot | Stage + commit (snapshot) con mensaje. | git-save-snapshot |
| git-sync-remote | fetch, pull o push contra remoto. | git-sync-remote |
| git-tactical-retreat | Stash / reset / clean con confirmación destructiva. | git-tactical-retreat |
| git-create-pr | Crear PR con GitHub CLI (`gh`). | git-create-pr |
| invoke-command | Interceptor de comandos de sistema (git, dotnet, npm, pwsh). | invoke-command |
| git-workspace-recon | Inspección del workspace Git (rama, porcelana, ahead/behind). | git-workspace-recon |
| git-branch-manager | Crear, checkout o eliminar ramas. | git-branch-manager |
| git-save-snapshot | Stage y commit con mensaje. | git-save-snapshot |
| git-sync-remote | fetch / pull / push contra remoto. | git-sync-remote |
| git-tactical-retreat | Stash, reset duro, clean (confirmación destructiva). | git-tactical-retreat |
| git-create-pr | Crear PR con `gh`. | git-create-pr |
| git-operations | Uso seguro de Git (ramas feat/fix, commits convencionales). | — |
| documentation | Estándares SSOT y gestión de documentación. | — |
| filesystem-ops | Operaciones de archivo seguras (PowerShell). | — |
| dotnet-development | Estándares .NET (build, test, logging). | — |
| frontend-build | Build Next.js (Product, Admin) fallback. | — |
| security-audit | Auditoría y hooks pre-commit/pre-push. | — |

## Guías de portabilidad (otros repos SddIA)

| Documento | Propósito |
| :--- | :--- |
| `reproducir-create-skill-en-otros-entornos-sddia.md` | Portar el **proceso de tarea** `create-skill` y su visibilidad (`#Process`, README, difusión). |
| `reproducir-skills-en-otros-entornos-sddia.md` | Portar **skills concretas** (definición, cápsula, índice, Rust). |

## Referencias

- Contrato global: `skills-contract.json`, `skills-contract.md` (en este directorio).
- Cúmulo: `SddIA/agents/cumulo.json` → **paths.skillsDefinitionPath**, **paths.skillsPath**, **paths.skillCapsules**, **paths.skillsIndexPath**.
- Índice de implementaciones: **paths.skillsIndexPath** (paths.skillsIndexPath, Cúmulo).
