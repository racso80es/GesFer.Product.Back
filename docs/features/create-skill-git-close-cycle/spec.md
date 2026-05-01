---
feature_id: create-skill-git-close-cycle
artifact: spec
status: draft
---

# Especificación

## Hito 1 — Skill `git-close-cycle`

- **skill_id:** `git-close-cycle`
- **Parámetros:** `targetBranch` (string, requerido); `mainBranch` (opcional); `workingDirectory` (opcional).
- **Secuencia Git (en repo válido):**
  1. Resolver troncal: `main` o `master` según exista `origin/<nombre>`, o usar `mainBranch`.
  2. `git checkout <troncal>`
  3. `git pull origin HEAD`
  4. `git fetch --prune`
  5. `git branch -d <targetBranch>`; si falla, `git branch -D <targetBranch>`
- **Salida:** JSON skills-contract (`success`, `feedback`, `data` con `targetBranch`, `mainBranch`).

## Hito 2 — Acción `finalize-process`

- En disparadores «tarea finalizada» / cierre de ciclo, el **último paso orquestado** (tras publicación y PR cuando aplique, y **tras** confirmación de fusión remota del trabajo) es invocar **git-close-cycle** con `targetBranch` igual al nombre de la **rama de trabajo** que se documentó en la tarea (rama actual de trabajo antes de abandonar la feature en local).

## Rutas (Cúmulo)

- Definición: `paths.skillsDefinitionPath/git-close-cycle/`
- Implementación: `paths.skillCapsules.git-close-cycle`
- Fuente Rust: `paths.skillsRustPath` → `src/bin/git_close_cycle.rs`
