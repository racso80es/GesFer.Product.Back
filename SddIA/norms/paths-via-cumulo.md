# Norma: Rutas vía Cúmulo

**Fuente:** SddIA/norms. Consulte Cúmulo (contrato de paths) para toda ruta de fichero o carpeta.

## Principio

En SddIA **no se escriben rutas de ficheros literales** (ej. `docs/features/`, `scripts/skills/`, `docs/audits/`). Toda ruta se obtiene del **contrato de paths** referenciado por Cúmulo: `SddIA/agents/cumulo.json` → `pathsContract` → `SddIA/agents/cumulo.paths.json` (clave `paths`). El ejecutor (p. ej. Tekton) o cualquier agente que necesite una ruta debe **consultar Cúmulo** (cargar el contrato indicado en `pathsContract`) para resolver paths.featurePath, paths.auditsPath, paths.skillCapsules[skill-id], etc. Las instrucciones de mapeo están en el **contrato de instructions**: `instructionsContract` → `SddIA/agents/cumulo.instructions.json`.

## Claves de paths (contrato cumulo.paths.json)

- **Persistencia de tareas:** paths.featurePath, paths.fixPath, paths.logPath.
- **Evolution y auditoría:** paths.evolutionPath, paths.evolutionLogFile, paths.auditsPath, paths.accessLogFile.
- **Evolution del protocolo SddIA (distinto de evolución de producto en docs):** paths.sddiaEvolutionPath, paths.sddiaEvolutionLogFile, paths.sddiaEvolutionContractFile. No confundir con paths.evolutionPath / `docs/evolution/` (cierres de feature/producto).
- **Técnico y operativo:** paths.architecturePath, paths.infrastructurePath, paths.debtPath, paths.tasksPath.
- **SddIA:** paths.actionsPath, paths.processPath, paths.patternsPath, paths.principlesPath, paths.tokensPath, paths.normsPath.
- **Skills y tools:** paths.skillsDefinitionPath, paths.skillCapsules[skill-id], paths.skillsRustPath; paths.toolsDefinitionPath, paths.toolCapsules[tool-id], paths.toolsRustPath.
- **Plantillas:** paths.templatesPath (configuraciones predefinidas de procesos).

## Aplicación

- **Actions, process, skills, agents:** Referenciar rutas solo como paths.\<clave\> o paths.\<clave\>[\<id\>]. Estructura de entidades: archivo .md con frontmatter YAML (paths.actionsPath, paths.processPath, paths.skillsDefinitionPath, paths.toolsDefinitionPath, etc.). No usar cadenas literales docs/... ni scripts/... en la documentación de comportamiento.
- **AGENTS.md y constitution:** Indicar que la única fuente de rutas es el contrato de paths (cumulo.paths.json) referenciado por Cúmulo; no ejemplos con rutas literales salvo en ese contrato.
