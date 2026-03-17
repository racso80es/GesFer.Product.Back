# Skills (paths.skillsPath)

Este directorio es **paths.skillsPath** (Cúmulo, `SddIA/agents/cumulo.json`). Contiene el índice de skills con implementación ejecutable y las cápsulas por skill.

## Listado de skills

El listado canónico se obtiene de:

- **Índice (Cúmulo):** **paths.skillsIndexPath** — fichero `index.json` en la raíz de skills; array `skills` con `skillId`, `path`, `manifest`, `launcher_bat`, `description`.
- **Fuente de verdad de rutas:** Cúmulo **paths.skillCapsules** (`SddIA/agents/cumulo.json`).

| skillId | Descripción breve | Launcher en cápsula |
|---------|-------------------|---------------------|
| **iniciar-rama** | Crea rama feat/ o fix/ actualizada con master. | `iniciar-rama/Iniciar-Rama.bat` |
| **finalizar-git** | Post-merge: master, sync, eliminar rama. Pre-pr: Unificar-Rama.ps1. | `finalizar-git/Merge-To-Master-Cleanup.bat` |
| **invoke-command** | Interceptor de comandos de sistema (git, dotnet, npm, pwsh). | `invoke-command/Invoke-Command.bat` |

Cada skill con ejecutable reside en una **cápsula** **paths.skillCapsules[&lt;skill-id&gt;]** con `manifest.json`, script `.ps1`, launcher `.bat` y opcionalmente `bin/` con el ejecutable Rust (generado por `scripts/skills-rs/install.ps1`).

## Implementación por defecto: Rust

Las implementaciones por defecto de los scripts de skills han de ser en Rust (igual que en tools). Los binarios se construyen en `scripts/skills-rs` y se copian a cada cápsula `bin/`. El launcher `.bat` invoca el `.exe` en `bin/` si existe; si no, fallback al `.ps1`.

## Definición vs implementación

- **Definición:** SddIA/skills/&lt;skill-id&gt;/ (spec.md, spec.json) — paths.skillsDefinitionPath.
- **Implementación:** scripts/skills/&lt;skill-id&gt;/ — paths.skillCapsules[skill-id].

Contrato: `SddIA/skills/skills-contract.json`.
