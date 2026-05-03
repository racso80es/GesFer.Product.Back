---
feature_name: refactor-actions-contract-and-finalize
branch: feat/refactor-actions-contract-and-finalize
created: '2026-05-01'
global: pass
checks:
  - id: contrato-acciones
    result: pass
    note: actions-contract.md v1.1.0 con jurisdicción; finalize-process creada; finalize retirada
  - id: barrido-referencias
    result: pass
    note: procesos, normas, plantillas, validate spec, .cursor/rules, AGENTS.norms.md, cumulo process_interface
  - id: tooling-tekton
    result: pass
    note: run-capsule-from-tekton-request.ps1 + .gitignore .tekton_request.json; skills-rs compilado localmente
  - id: impacto-sddia
    result: pass
    note: Mutación bajo SddIA/; registro evolution pendiente en misma sesión (sddia-evolution-register)
git_changes: true
---

# Validación pre-PR

## Resumen

- Contrato de acciones actualizado: acciones solo orquestan skills/tools registradas; sin ejecución directa de SO/scripts en la definición.
- Acción **finalize-process** sustituye a **finalize**; referencias actualizadas en procesos, normas, plantillas y difusión Cursor.
- Blindaje JSON Tekton en `scripts/skills/run-capsule-from-tekton-request.ps1`.
- Creado `docs/features/features-contract.md` con fila finalize-process.

## Evaluación de impacto SDdIA

Hubo cambios bajo `./SddIA/`. Se ejecutará **sddia-evolution-register** antes de **git-sync-remote**, más commit de consolidación.
