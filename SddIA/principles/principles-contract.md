---
agents_implementation: 'Los agentes que aplican principios declaran principlesContract (agents.principles). Norma: SddIA/norms/agents-principles-contract.md.'
constraints:
- Cumplimiento obligatorio de Karma2Token para trazabilidad y seguridad.
- principle_id en kebab-case.
- Rutas canónicas solo desde Cúmulo (paths.principlesPath).
- Un principio sin spec.md (con frontmatter YAML) en su carpeta no se considera completo.
- 'Principios con blocking_for_pr: true son obligatorios en validate; si el check falla, global fail y blocking true (PR no debe aprobarse).'
- 'Los principios son entidades de dominio (ecosistema SddIA): integran el contrato Token y deben respetar estructura frontmatter YAML. Norma: SddIA/norms/entidades-dominio-ecosistema-sddia.md.'
consumers:
- SddIA/agents/architect.json
- SddIA/agents/tekton-developer.json
- SddIA/agents/cumulo.json
- paths.actionsPath
- paths.processPath
contract_version: 1.0.0
definition_artefacts:
- ext: .md
  format: frontmatter_yaml
  naming: spec.md
  path: paths.principlesPath/<principle-id>/
  purpose: 'Especificación: frontmatter YAML (metadatos: id, principle_id, title, category, tags, metadata, contract_ref) + cuerpo Markdown. es-ES.'
description: 'Contrato que todo principio técnico debe cumplir: definición en paths.principlesPath/<principle-id>/ con archivo .md con frontmatter YAML. Principios consumidos por Arquitecto y Tekton en tareas de diseño e implementación.'
scope: paths.principlesPath (SddIA/principles/)
security_model:
  description: La aplicación o modificación de un principio requiere un contexto de Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato: Principios técnicos (principles)

**Alcance:** paths.principlesPath (SddIA/principles/). Toda entidad que actúe como principio técnico debe cumplir este contrato.

**Objetivo:** Los principios son guías técnicas (Clean Code, SOLID, DDD, Testing, etc.) tenidas en cuenta por **Arquitecto** y **Tekton** al realizar tareas de diseño e implementación. Solo definición; no tienen implementación ejecutable.

---

## 1. Definición por principio (paths.principlesPath/<principle-id>/)

Cada principio tiene una **carpeta** en paths.principlesPath con identificador `<principle-id>` (kebab-case). Dentro de la carpeta:

| Artefacto | Propósito |
|-----------|-----------|
| **spec.md** | Especificación legible: título, categoría, contenido del principio, aplicación para arquitectos/tekton, referencias. es-ES. |
| **spec.json** | Metadatos machine-readable: id (UUID), principle_id, title, category, tags, metadata (difficulty, status), contract_ref. |

## 2. Restricciones

- principle_id en kebab-case.
- Rutas canónicas solo desde Cúmulo (paths.principlesPath).
- Un principio sin spec.md y spec.json en su carpeta no se considera completo.

## 3. Consumidores

SddIA/agents/architect.json, SddIA/agents/tekton-developer.json, acciones y procesos que requieran criterios de diseño.

**Referencia machine-readable:** paths.principlesPath/principles-contract.json.
