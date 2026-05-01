---
feature_id: create-skill-git-close-cycle
process: feature
process_spec_version: 2.1.0
task_version: 2.0.0
branch: feat/create-skill-git-close-cycle
status: in_progress
---

# Objetivos

## Objetivo

Introducir la skill **git-close-cycle** (Rust) y enlazarla formalmente a la acción **finalize-process** como paso de orquestación al cerrar una tarea cuya rama ya está fusionada en remoto.

## Alcance

1. **Forja:** definición SddIA, cápsula, binario `git_close_cycle`, registro en Cúmulo (`paths.skillCapsules`).
2. **Enlace:** actualizar `SddIA/actions/finalize-process/spec.md` para que el cierre orquestado incluya **git-close-cycle** con `targetBranch` = rama de trabajo documentada.

## Ley aplicada

Orquestación solo vía skills registradas; Git sin comandos sueltos en specs; mutación bajo `SddIA/` con registro de evolución según norma.
