# Acciones del ciclo (SddIA/actions)

**Alcance:** paths.actionsPath. Cada acción tiene una carpeta con spec.md y spec.json (contrato: actions-contract.json). Rutas vía Cúmulo.

| action_id | Descripción | Definición |
|-----------|-------------|------------|
| spec | Especificación: transformar requerimientos en SPEC técnico formal. | paths.actionsPath/spec/ |
| clarify | Clarificación: resolver ambigüedades y gaps en SPECs. | paths.actionsPath/clarify/ |
| planning | Plan: convertir spec y clarificaciones en hoja de ruta ejecutable. | paths.actionsPath/planning/ |
| implementation | Implementación (doc): touchpoints en código; no modifica código. | paths.actionsPath/implementation/ |
| execution | Ejecución: aplicar al código los cambios del documento de implementación. | paths.actionsPath/execution/ |
| validate | Validación: calidad pre-PR (build, tests, docs); validacion.md. | paths.actionsPath/validate/ |
| finalize-process | Finalizar proceso/tarea: cierre (commits, Evolution Logs, push, PR). Suite táctica: git-save-snapshot / invoke-commit, git-sync-remote, git-create-pr; emergencia git-tactical-retreat. | paths.actionsPath/finalize-process/ |
| sddia-difusion | Difusión de SddIA: mantener .cursor/rules, .github alineados con SddIA. | paths.actionsPath/sddia-difusion/ |

Orden típico en proceso feature: spec → clarify → planning → implementation → execution → validate → finalize-process.

Contrato: actions-contract.json, actions-contract.md (raíz de actions).
