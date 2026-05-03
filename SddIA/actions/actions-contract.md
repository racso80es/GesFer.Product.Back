---
constraints:
- action_id en kebab-case.
- Rutas canónicas solo desde Cúmulo (paths.actionsPath, paths.processPath, paths.featurePath, paths.fixPath, paths.principlesPath, paths.patternsPath).
- 'Acciones de diseño/implementación: validar principios según principles-contract.'
- 'Planning, implementation, execution: aplicar patrones según patterns-in-planning-implementation-execution.md.'
- Cumplimiento obligatorio de Karma2Token para trazabilidad y seguridad.
consumers:
- paths.processPath
- SddIA/agents/*.json
- SddIA/norms/interaction-triggers.md
- .cursor/rules
contract_version: 1.1.0
definition_artefacts:
- ext: .md
  format: frontmatter_yaml
  naming: spec.md
  path: paths.actionsPath/<action-id>/
  purpose: 'Especificación: frontmatter YAML (metadatos) + cuerpo Markdown. Campos: action_id, name, purpose, inputs, outputs, flow_steps, contract_ref. es-ES.'
description: 'Contrato que toda acción del ciclo debe cumplir: definición en paths.actionsPath/<action-id>/ con archivo .md con frontmatter YAML. Entrada/salida y flujo documentados.'
patterns_validation: 'Las acciones planning, implementation y execution deben consultar paths.patternsPath para referenciar patrones en el PLAN, en ítems IMPL (pattern_id) y en ejecución. Norma: SddIA/norms/patterns-in-planning-implementation-execution.md.'
principles_validation: 'Las acciones que afecten diseño o implementación (spec, planning, implementation, execution) deben validar coherencia con paths.principlesPath (principles-contract.json). En spec.json puede declararse principles_applicable (array de principle_id) o principles_ref: paths.principlesPath.'
scope: paths.actionsPath (SddIA/actions/)
security_model:
  description: Toda acción debe ser invocada bajo el contexto de un Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato: Acciones del ciclo (actions)

**Alcance:** paths.actionsPath (SddIA/actions/). Toda acción del ciclo debe cumplir este contrato.

## Jurisdicción de las acciones (innegociable)

Una **acción** es un artefacto **documental y de orquestación**: define propósito, entradas, salidas y el **orden lógico** de pasos del ciclo. **No** tiene jurisdicción para:

- Ejecutar comandos del sistema operativo directamente.
- Invocar scripts (`.ps1`, `.bat`, `.sh`) ni encadenar shell como mecanismo de implementación de la acción.
- Lanzar binarios que **no** estén publicados y referenciados como **skill** o **tool** en Cúmulo (`paths.skillCapsules`, `paths.toolCapsules`).

La **única** forma permitida de materializar el trabajo operativo es: la acción **nombra y ordena** qué **skills** o **tools** debe invocar el ejecutor (agente/humano), conforme a `SddIA/norms/commands-via-skills-or-tools.md` y a la implementación estándar Rust del proyecto.

## Definición por acción

Cada acción tiene una **carpeta** en paths.actionsPath con identificador `<action-id>` (kebab-case). Dentro de la carpeta:

| Artefacto | Propósito |
|-----------|-----------|
| **spec.md** | Especificación legible: frontmatter YAML (action_id, inputs, outputs, flow_steps, etc.) + cuerpo Markdown. es-ES. Sin spec.json separado. |

## Restricciones

- action_id en kebab-case (spec, clarify, planning, implementation, execution, validate, finalize-process, sddia-difusion).
- Salida de acciones en carpeta de tarea (paths.featurePath, paths.fixPath): un .md por acción con frontmatter YAML + Markdown. Sin .json separados. Patrón: SddIA/norms/features-documentation-pattern.md.
- Rutas solo vía Cúmulo.

## Consumidores

paths.processPath, SddIA/agents, SddIA/norms (interaction-triggers), .cursor/rules.
