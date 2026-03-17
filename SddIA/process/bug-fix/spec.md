---
constraints:
- Rama fix/<nombre_fix>, nunca master.
- Documentación obligatoria en paths.fixPath/<nombre_fix>.
- 'Alcance mínimo: causa raíz; no refactor en la misma rama.'
contract_ref: paths.processPath/process-contract.md
paths:
  bugPath_ref: paths.bugPath (Cúmulo)
  fixPath_ref: paths.fixPath (Cúmulo)
persist_ref: paths.fixPath/<nombre_fix>
principles_ref: paths.principlesPath
process_id: bug-fix
process_interface_compliance: Solicita/genera en carpeta de la tarea artefactos .md con frontmatter YAML (objectives.md, spec.md, clarify.md, validacion.md). Sin .json separados. Patrón: SddIA/norms/features-documentation-pattern.md.
related_actions:
- spec
- clarify
- implementation
- execution
- validate
- finalize
related_skills:
- iniciar-rama
skills:
- documentation
- filesystem-ops
- dotnet-development
- iniciar-rama
spec_version: 1.0.0
triggers:
- Reporte de bug
- Fallo en CI o tests
- Solicitud de fix con bug-id o título
---

# Proceso: Bug Fix

Este documento define el **proceso de tarea** para la corrección de un bug. Está ubicado en paths.processPath/bug-fix/ (Cúmulo). La ruta de persistencia se obtiene de **Cúmulo** (paths.fixPath/<nombre_fix>).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): solicita/genera en la carpeta de la tarea (Cúmulo) artefactos **`.md`** con frontmatter YAML (objectives.md, spec.md, clarify.md, validacion.md). Sin .json separados. Patrón: SddIA/norms/features-documentation-pattern.md.

## Propósito

El proceso **bug-fix** orquesta el ciclo de vida del bug: triaje, documentación, reproducción, alcance mínimo del fix y trazabilidad en la carpeta de persistencia del fix (paths.fixPath/<nombre_fix>).

## Alcance

- **Rama:** fix/<nombre_fix> (nunca master).
- **Documentación:** Carpeta paths.fixPath/<nombre_fix>/ con objectives.md, spec.md, clarify.md si aplica, implementation.md, validacion.md (todos con frontmatter YAML + Markdown). Sin .json separados.
- **Skills:** iniciar-rama, documentation, filesystem-ops, dotnet-development.
- **Restricciones:** Alcance mínimo (solo causa raíz); no refactorizar ni ampliar funcionalidad en la misma rama.

## Integración

El agente Bug Fix Specialist (definición en paths.processPath/bug-fix/) orquesta el ciclo. En la descripción del PR y en Evolution Logs, la referencia canónica es **paths.fixPath/<nombre_fix>/** (Cúmulo). SSOT para ese fix.
