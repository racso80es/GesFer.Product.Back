---
feature_name: refactor-actions-contract-and-finalize
created: '2026-05-01'
process: feature
artifact: plan
phases:
  - id: '0'
    name: Preparar entorno Git
    skills: [git-workspace-recon, git-branch-manager]
  - id: '1'
    name: Contrato y norma de jurisdicción
    outputs: [SddIA/actions/actions-contract.md]
  - id: '2'
    name: Nueva acción finalize-process
    outputs: [SddIA/actions/finalize-process/spec.md]
  - id: '3'
    name: Eliminar legacy finalize
    outputs: [remove SddIA/actions/finalize/]
  - id: '4'
    name: Barrido de referencias
    outputs: [SddIA/process, SddIA/norms, SddIA/templates, SddIA/skills, .cursor/rules, AGENTS.norms.md]
  - id: '5'
    name: Difusión y contrato features
    outputs: [docs/features/features-contract.md si aplica, sddia-difusion vía reglas]
  - id: '6'
    name: Validación local
    skills: [invoke-command o validadores existentes según proyecto]
  - id: '7'
    name: Commits atómicos
    skills: [git-save-snapshot]
  - id: '8'
    name: Evolución SDdIA + cierre PR
    skills: [sddia-evolution-register, git-save-snapshot, git-sync-remote, git-create-pr]
---

# Plan de implementación

## Fase 0 — Entorno

1. Compilar skills si faltan `.exe` (`scripts/skills-rs/install.ps1` en entorno autorizado).
2. **git-workspace-recon** (working tree limpio, rama base identificada).
3. **git-branch-manager** `create` `feat/refactor-actions-contract-and-finalize` desde `master` (o default del repo).

## Fase 1 — Contrato

1. Editar `SddIA/actions/actions-contract.md`:
   - Sección **Jurisdicción**: acciones solo orquestan skills/tools registradas; prohibido SO/scripts.
   - Actualizar listas de `action_id`: sustituir `finalize` por `finalize-process`.
2. Revisar `SddIA/actions/README.md` y tabla de acciones.

## Fase 2 — Acción `finalize-process`

1. Crear `SddIA/actions/finalize-process/spec.md` migrando contenido útil de `finalize/spec.md`:
   - Objetivos de cierre (logs, PR, precondiciones).
   - Flujo como lista de skills: `git-save-snapshot` / `invoke-commit`, `git-sync-remote`, `git-create-pr`; validación vía skill nombrada si el repo la mantiene.
   - Sin `implementation_script_ref` a PowerShell.

## Fase 3 — Retirada `finalize`

1. Eliminar carpeta `SddIA/actions/finalize/` tras copiar/merge en `finalize-process`.

## Fase 4 — Barrido

1. `grep`/reemplazo controlado de referencias `finalize` como acción del ciclo (no tocar palabra “finalizar” en prosa si no es `action_id`).
2. Actualizar `SddIA/norms/interaction-triggers.md` (+ JSON si aplica), procesos en `SddIA/process/**`, plantillas, `git-operations`, `.cursor/rules`, `AGENTS.norms.md`.

## Fase 5 — Patrón documentación features

1. En `SddIA/norms/features-documentation-pattern.md` (y difusión `.cursor`): fila **finalize-process** / archivo `finalize-process.md` opcional para cierre.

## Fases 6–7 — Validación y commits

1. Validar coherencia markdown/frontmatter.
2. **git-save-snapshot** por hito (contrato; acción nueva; barrido; normas).

## Fase 8 — Evolución y PR

1. **sddia-evolution-register** (mutación `SddIA/`).
2. **git-save-snapshot** (registro evolución).
3. **git-sync-remote** + **git-create-pr** con resumen de validación y enlaces a esta carpeta (`paths.featurePath/refactor-actions-contract-and-finalize/`).

## Dependencias

- Orden lógico: fases 1–4 antes de 8; evolución SDdIA solo tras archivos `SddIA/` estables en disco.
