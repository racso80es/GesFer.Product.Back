# Herramientas (paths.toolsPath)

Este directorio es **paths.toolsPath** (Cúmulo, `SddIA/agents/cumulo.json`). Contiene el índice de herramientas y los launchers wrapper que delegan a cada cápsula.

## Listado de herramientas

El listado canónico de herramientas existentes se obtiene de:

- **Índice (Cúmulo):** **paths.toolsIndexPath** — fichero `index.json` en la raíz de tools; array `tools` con `toolId`, `path`, `manifest`, `wrapper_bat`, `description`.
- **Fuente de verdad de rutas:** Cúmulo **paths.toolCapsules** (`SddIA/agents/cumulo.json`).

| toolId | Descripción breve | Launcher (wrapper) |
|--------|-------------------|-------------------|
| **invoke-mysql-seeds** | Migraciones EF y seeds de Admin sobre MySQL. | `Invoke-MySqlSeeds.bat` |
| **prepare-full-env** | Docker (DB, cache, Adminer) y opcionalmente API y clientes. | `Prepare-FullEnv.bat` |

Cada herramienta reside en una **cápsula** **paths.toolCapsules[&lt;tool-id&gt;]** con `manifest.json`, script `.ps1`, config, documentación y el ejecutable Rust (`.exe`) en la misma carpeta de la cápsula.

## Uso del índice

Para listar las herramientas de forma programática (scripts, agentes, CI):

- Leer `scripts/tools/index.json` y usar el array `tools`.
- O consultar Cúmulo `paths.toolCapsules` para las rutas canónicas de cada cápsula.

Contrato de herramientas: `SddIA/tools/tools-contract.json`.
