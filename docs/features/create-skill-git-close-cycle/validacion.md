---
feature_id: create-skill-git-close-cycle
artifact: validacion
status: completed
pr_url: https://github.com/racso80es/GesFer.Product.Back/pull/116
evolution_id: cf1dbcaf-471a-4fd5-8e3c-47cae9884128
---

# Validación pre-PR

## Evaluación de impacto SDDIA

**Mutación bajo `./SddIA/`:** sí (skills, norms, actions, process, agents paths, evolution pendiente de registro).

**Acción obligatoria antes de push:** ejecutado **sddia-evolution-register** → detalle `SddIA/evolution/cf1dbcaf-471a-4fd5-8e3c-47cae9884128.md`; snapshot adicional aplicado.

## Checks manuales

- Contrato JSON de **git-close-cycle**: `targetBranch` requerido; salida alineada con skills-contract.
- **finalize-process**: paso 6 coherente con disparador «tarea finalizada» y fusión remota confirmada.
- **cumulo.paths.json**: entrada `git-close-cycle` presente.

## Build

Tras `install.ps1`, comprobar existencia de `scripts/skills/git-close-cycle/bin/git_close_cycle.exe`.
