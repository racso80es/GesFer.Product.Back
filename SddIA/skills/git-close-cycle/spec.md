---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-close-cycle
owner: tekton-developer
parameters:
  mainBranch:
    required: false
    type: string
  targetBranch:
    required: true
    type: string
  workingDirectory:
    required: false
    type: string
skill_id: git-close-cycle
spec_version: 1.0.0
status: Active
---

# Skill: git-close-cycle

Cierra el **ciclo de desarrollo local** cuando la rama de trabajo ya está integrada en remoto: sitúa el repositorio en la rama troncal (`main` o `master`, detectada vía `origin/<troncal>` salvo `mainBranch` explícita), ejecuta `git pull origin HEAD`, `git fetch --prune` y elimina la rama local indicada con `git branch -d`, con **fallback** a `-D` si `-d` no aplica.

## Entrada JSON (camelCase)

- **targetBranch** (obligatorio): nombre de la rama local a dar de baja.
- **mainBranch** (opcional): troncal a usar; si se omite, se elige entre `main` y `master` según exista `origin/main` o `origin/master`.
- **workingDirectory** (opcional): raíz del repo; por defecto `GESFER_REPO_ROOT` o directorio actual.

## Errores

- `targetBranch` igual a la troncal: validación rechazada.
- Fallo en checkout, pull, fetch o borrado: salida con `success: false` y detalle en `feedback`.
