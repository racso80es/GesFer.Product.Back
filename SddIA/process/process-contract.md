---
constraints:
- Cumplimiento obligatorio de Karma2Token para trazabilidad y seguridad.
- process_id en kebab-case (feature, bug-fix, refactorization, create-tool, create-skill).
- Rutas canónicas solo desde Cúmulo (paths.processPath, paths.featurePath, paths.fixPath, paths.actionsPath, paths.principlesPath).
- Un proceso sin spec.md (con frontmatter YAML) en su carpeta no se considera completo.
- 'Fases de diseño/implementación: validar principios según principles-contract.'
consumers:
- paths.actionsPath
- SddIA/agents/*.json
- SddIA/norms/interaction-triggers.md
- .cursor/rules
contract_version: 1.0.0
definition_artefacts:
- ext: .md
  format: frontmatter_yaml
  naming: spec.md
  path: paths.processPath/<process-id>/
  purpose: 'Especificación: frontmatter YAML (metadatos) + cuerpo Markdown. Campos: process_id, name, description, phases, persist_ref, related_actions, contract_ref. es-ES.'
description: 'Contrato que todo proceso de tarea debe cumplir: definición en paths.processPath/<process-id>/ con archivo .md con frontmatter YAML. Cumple process_interface (Cúmulo) para artefactos generados en la carpeta de la tarea.'
principles_validation: 'Los procesos deben validar que las fases de diseño e implementación sean coherentes con paths.principlesPath (principles-contract). En spec.json puede declararse principles_ref: paths.principlesPath.'
process_interface_ref: 'Cúmulo (SddIA/agents/cumulo.json) → process_interface: artefactos .md con frontmatter YAML en la carpeta de la tarea (paths.featurePath, paths.fixPath). Sin .json separados. Patrón: SddIA/norms/features-documentation-pattern.md.'
scope: paths.processPath (SddIA/process/)
security_model:
  description: Todo proceso debe orquestarse bajo el contexto de un Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato: Procesos de tarea (process)

**Alcance:** paths.processPath (SddIA/process/). Todo proceso de tarea debe cumplir este contrato.

## Definición por proceso

Cada proceso tiene una **carpeta** en paths.processPath con identificador `<process-id>` (kebab-case). Dentro de la carpeta:

| Artefacto | Propósito |
|-----------|-----------|
| **spec.md** | Especificación legible: propósito, fases, alcance, ruta de persistencia (persist_ref). Idioma: es-ES. |
| **spec.json** | Metadatos machine-readable: process_id, name, description, phases, persist_ref (paths.featurePath o paths.fixPath), related_actions (paths.actionsPath), contract_ref. |

## Interfaz de proceso (Cúmulo)

La **interfaz de proceso** (qué genera un proceso en la carpeta de la tarea) está definida en Cúmulo (cumulo.json → process_interface): artefactos `.md` con frontmatter YAML + cuerpo Markdown (objectives.md, spec.md, clarify.md, plan.md, implementation.md, execution.md, validacion.md). **No se usan ficheros `.json` separados.** Patrón: SddIA/norms/features-documentation-pattern.md; contrato: paths.featurePath/features-contract.md.

## Restricciones

- process_id en kebab-case.
- Rutas solo vía Cúmulo; no literales en la definición.
- Un proceso sin spec.md y spec.json en paths.processPath/<process-id>/ no se considera completo.

## Consumidores

paths.actionsPath, SddIA/agents, SddIA/norms (interaction-triggers), .cursor/rules.
