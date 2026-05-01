---
feature_id: create-skill-git-close-cycle
artifact: validacion
status: draft
---

# Validación pre-PR

## Evaluación de impacto SDDIA

**Mutación bajo `./SddIA/`:** sí (skills, norms, actions, process, agents paths, evolution pendiente de registro).

**Acción obligatoria antes de push:** invocar **sddia-evolution-register** con JSON camelCase (Tekton: `.tekton_request.json` + `run-capsule-from-tekton-request.ps1 -Skill sddia-evolution-register`) y consolidar con **git-save-snapshot** adicional, según proceso feature fase 8.

## Checks manuales

- Contrato JSON de **git-close-cycle**: `targetBranch` requerido; salida alineada con skills-contract.
- **finalize-process**: paso 6 coherente con disparador «tarea finalizada» y fusión remota confirmada.
- **cumulo.paths.json**: entrada `git-close-cycle` presente.

## Build

Tras `install.ps1`, comprobar existencia de `scripts/skills/git-close-cycle/bin/git_close_cycle.exe`.
