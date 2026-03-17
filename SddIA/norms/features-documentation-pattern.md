---
contract_ref: docs/features/features-contract.md
norm_id: features-documentation-pattern
related:
- SddIA/norms/entidades-dominio-ecosistema-sddia.md
- SddIA/process/feature/spec.md
- paths.featurePath
---

# Norma: Patrón de documentación de features (paths.featurePath)

**Fuente:** SddIA/norms. Aplicable a toda documentación de tareas en paths.featurePath (Cúmulo) y paths.fixPath (Cúmulo).

## Principio

La documentación de features y fixes sigue el **mismo patrón que skills, tools y entidades de dominio SddIA**: **un solo fichero `.md` por acción**, con **frontmatter YAML** (metadatos estructurados) + **cuerpo Markdown** (contenido legible). **No se usan ficheros `.json` separados.**

## Obligaciones

1. **Un artefacto por acción:** Cada fase del proceso (spec, clarify, planning, implementation, execution, validate, finalize) produce **exclusivamente** un archivo `.md` con frontmatter YAML + cuerpo Markdown.
2. **Sin duplicación JSON:** No se generan ni mantienen archivos `spec.json`, `clarify.json`, `plan.json`, `implementation.json`, `execution.json`, `validacion.json` ni `finalize.json`. Los datos estructurados residen en el frontmatter YAML del `.md` correspondiente.
3. **Estructura canónica:** Cada `.md` debe tener:
   - **Frontmatter YAML** (entre `---`): metadatos machine-readable (feature_name, created, scope, checks, etc. según el tipo de artefacto).
   - **Cuerpo Markdown:** contenido legible para humanos (secciones, tablas, listas).
4. **Validación:** El check opcional `sddia_frontmatter_valid` (acción validate) aplica a los `.md` de paths.featurePath cuando el diff los toque.

## Artefactos por acción

| Acción | Archivo | Frontmatter (campos mínimos) | Cuerpo |
|-------|---------|-----------------------------|--------|
| objectives | objectives.md | feature_name, created, process | Objetivo, alcance, ley aplicada |
| spec | spec.md | feature_name, created, base, scope (opc.) | Especificación técnica |
| clarify | clarify.md | feature_name, created, purpose | Clarificaciones y decisiones |
| planning | plan.md | feature_name, created, phases | Plan de implementación |
| implementation | implementation.md | feature_name, created, items | Touchpoints y propuestas |
| execution | execution.md | feature_name, created, items_applied | Registro de ejecución |
| validate | validacion.md | feature_name, branch, global, checks, git_changes | Informe de validación |
| finalize | finalize.md (opc.) | feature_name, pr_url, timestamp | Resumen de cierre |

Detalle completo en **docs/features/features-contract.md** (Cúmulo: paths.featurePath/features-contract.md).

## Compatibilidad con entidades de dominio

Este patrón es coherente con **SddIA/norms/entidades-dominio-ecosistema-sddia.md**: las entidades de dominio (skills, tools, actions, process) usan `.md` con frontmatter YAML. La documentación de tareas (features, fixes) aplica el mismo estándar para mantener SSOT y evitar duplicación.

## Migración

Las features existentes con `.json` pueden migrarse gradualmente: consolidar el contenido del `.json` en el frontmatter YAML del `.md` correspondiente y eliminar el `.json`. Las **nuevas** documentaciones deben cumplir este patrón desde el inicio.

---
*Norma canónica. Ref: refactorization-sincronidad-md-json; alineada con entidades-dominio-ecosistema-sddia.*
