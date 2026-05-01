---
feature_name: refactor-actions-contract-and-finalize
created: '2026-05-01'
process: feature
process_version: 2.0.0
branch: feat/refactor-actions-contract-and-finalize
contract_ref: SddIA/norms/features-documentation-pattern.md
scope: SddIA/actions, SddIA/process, SddIA/norms, .cursor/rules, agentes y consumidores del contrato de acciones
---

# Objetivos

## Objetivo

Refactorizar el **contrato de acciones** y el ecosistema SDdIA que lo consume para que:

1. Las **acciones** sean exclusivamente **orquestadoras documentales**: su jurisdicción es definir flujo y enlazar **skills** y **tools** registradas en Cúmulo; **prohibido** invocar comandos del sistema operativo, scripts `.ps1`, `.bat`, `.sh` o binarios no declarados como skill/tool.
2. La acción hoy denominada **`finalize`** pase a **`finalize-process`**, con disparadores y texto alineados a **cierre de proceso/tarea**, y sin referencias a ejecución directa de scripts; el cierre Git (commits, push, PR) queda descrito como secuencia de **macro-operaciones** ya existentes: `git-save-snapshot` / `invoke-commit`, `git-sync-remote`, `git-create-pr`, `git-tactical-retreat` (emergencia).
3. **Saneamiento de referencias:** todos los procesos en `paths.processPath`, normas, plantillas, skills de definición y reglas Cursor que citen `finalize` como `action_id` deben actualizarse a **`finalize-process`** y, donde aplique, a rutas `paths.actionsPath/finalize-process/`.

## Alcance

- `SddIA/actions/actions-contract.md` (nueva sección normativa + lista de `action_id`).
- Carpeta de acción: renombrar/migrar `SddIA/actions/finalize/` → `SddIA/actions/finalize-process/` (solo `spec.md` y metadatos coherentes; sin `implementation_script_ref` hacia `.ps1`).
- Referencias en: `SddIA/process/**`, `SddIA/norms/**`, `SddIA/templates/**`, `SddIA/skills/**`, `SddIA/actions/README.md`, `.cursor/rules/**`, `AGENTS.norms.md` si lista acciones, `SddIA/norms/interaction-triggers.md` (+ `.json` si existe listado), `docs/features/features-contract.md` si menciona `finalize` como acción.
- Patrón de documentación de features: actualizar tabla **finalize** → **finalize-process** / artefacto `finalize-process.md` (o convención acordada en spec) vía acción **sddia-difusion** si toca `.cursor/rules`.

## Fuera de alcance (esta iteración)

- Eliminar físicamente `scripts/actions/finalize/` u otra deuda de scripts legacy (puede quedar como ítem opcional en `implementation.md` si hay dependencias CI).
- Cambiar el binario `verify-pr-protocol` o el flujo de validación pre-PR salvo que la nueva spec de `finalize-process` lo referencie solo como skill/tool.

## Ley aplicada

- **Ley COMANDOS** (`SddIA/norms/commands-via-skills-or-tools.md`): ejecución solo vía skills/tools; las acciones no son canal de ejecución directa.
- **Ley GIT** (`SddIA/norms/git-via-skills-or-process.md`): sin `git` ni `gh` en texto de “ejecutar a mano” dentro de la definición de acción; orquestación explícita vía skills registradas.
- **Impacto SDdIA** (`SddIA/norms/sddia-evolution-sync.md`): cambios bajo `SddIA/` obligan a **sddia-evolution-register** + entrada en `SddIA/evolution/` antes del cierre publicado.

## Precondición operativa (fase 0)

- Ejecutar **git-workspace-recon** y **git-branch-manager** (`create` → `feat/refactor-actions-contract-and-finalize` desde base canónica del repo) cuando existan los `.exe` en `paths.skillCapsules` (compilación `scripts/skills-rs/install.ps1` si faltan binarios).
