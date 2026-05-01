---
contract_id: features-documentation
norm_ref: SddIA/norms/features-documentation-pattern.md
paths_ref: SddIA/agents/cumulo.paths.json
---

# Contrato: documentación en paths.featurePath / paths.fixPath

Resumen de artefactos `.md` por fase (frontmatter YAML + cuerpo). Detalle normativo: `SddIA/norms/features-documentation-pattern.md`.

| Acción | Archivo | Frontmatter (mín.) |
|--------|---------|---------------------|
| objectives | objectives.md | feature_name, created, process |
| spec | spec.md | feature_name, created |
| clarify | clarify.md | feature_name, created |
| planning | plan.md | feature_name, created |
| implementation | implementation.md | feature_name, created |
| execution | execution.md | feature_name, created |
| validate | validacion.md | feature_name, branch, global |
| finalize-process | finalize-process.md (opc.) | feature_name, pr_url, timestamp |

Sin ficheros `.json` paralelos por acción (patrón unificado).
