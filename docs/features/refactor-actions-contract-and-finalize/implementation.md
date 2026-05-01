---
feature_name: refactor-actions-contract-and-finalize
created: '2026-05-01'
process: feature
artifact: implementation
status: pending
items:
  - id: IMPL-001
    pattern_id: null
    touchpoints:
      - SddIA/actions/actions-contract.md
    summary: Añadir jurisdicción estricta y action_id finalize-process
  - id: IMPL-002
    touchpoints:
      - SddIA/actions/finalize-process/spec.md
    summary: Nueva spec sin scripts; orquestación solo vía skills nombradas
  - id: IMPL-003
    touchpoints:
      - SddIA/actions/finalize/
    summary: Eliminar carpeta finalize tras migración
  - id: IMPL-004
    touchpoints:
      - SddIA/process/**/*.md
      - SddIA/process/**/*.json
      - SddIA/norms/interaction-triggers.md
      - SddIA/norms/interaction-triggers.json
      - SddIA/norms/commands-via-skills-or-tools.md
      - SddIA/norms/git-via-skills-or-process.md
      - SddIA/norms/features-documentation-pattern.md
      - SddIA/skills/git-operations/spec.md
      - SddIA/actions/sddia-difusion/spec.md
      - SddIA/actions/README.md
      - SddIA/templates/**/*.md
      - .cursor/rules/action-suggestions.mdc
      - .cursor/rules/features-documentation-pattern.mdc
      - .cursor/rules/subir-push.mdc
      - .cursor/rules/sddia-ssot.mdc
      - AGENTS.norms.md
    summary: Reemplazar referencias de acción finalize por finalize-process
  - id: IMPL-005
    touchpoints:
      - docs/features/features-contract.md
    summary: Alinear tabla de artefactos con finalize-process si el contrato lo define
  - id: IMPL-006
    touchpoints:
      - SddIA/evolution/
    summary: UUID + fila Evolution_log tras mutación SddIA; skill sddia-evolution-register
---

# Implementation (touchpoints)

Documento de touchpoints para la acción **implementation**: no ejecuta código; lista archivos y el cambio esperado.

| ID | Archivo / área | Cambio esperado |
|----|------------------|-----------------|
| IMPL-001 | `actions-contract.md` | Norma jurisdicción; lista acciones con `finalize-process`. |
| IMPL-002 | `finalize-process/spec.md` | Flujo cierre; skills explícitas; sin `.ps1`. |
| IMPL-003 | `finalize/` | Borrado post-migración. |
| IMPL-004 | Procesos, normas, templates, reglas | `finalize` → `finalize-process` donde sea `action_id` o ruta. |
| IMPL-005 | `features-contract.md` | Artefacto opcional de cierre renombrado si procede. |
| IMPL-006 | `SddIA/evolution/` | Registro obligatorio post-cambio SddIA. |

## Notas

- Scripts legacy `scripts/actions/finalize/`: evaluar en tarea aparte o anotar deuda; no bloquean el contrato si la acción ya no los referencia.
- Búsqueda: usar patrón que distinga `action_id: finalize` / `paths.actionsPath/finalize/` de la palabra común “finalizar”.
