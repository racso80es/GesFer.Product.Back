---
config:
  default_branch_pattern: feat/correccion-segun-auditorias | feat/correccion-auditorias-<id>
  default_persist: paths.featurePath/<nombre_correccion>
contract_ref: SddIA/templates/templates-contract.md
input_sources:
  audit_file: 'Opcional: fichero o patrón (ej. AUDITORIA_*.md, validacion-*.json)'
  audit_origin: paths.auditsPath (Cúmulo) o ruta parcial/total indicada
interested_agents:
- architect
- tekton-developer
- auditor-back
- auditor-front
- auditor-process
process_ref: correccion-auditorias
related_actions:
- spec
- clarify
- planning
- implementation
- execution
- validate
- finalize-process
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
- invoke-commit
- documentation
- security-audit
template_id: correccion-auditorias-feature
---

# Plantilla: Feature de corrección según auditorías

**template_id:** correccion-auditorias-feature  
**Contrato:** SddIA/templates/templates-contract.md

## Propósito

Esta plantilla procedimenta la **corrección de hallazgos de auditoría** mediante el ciclo de vida completo de una feature: análisis con los agentes adecuados (Auditor Back/Front/Process, Arquitecto, Tekton), definición de objetivos, especificación, clarificación, planificación, implementación, ejecución, validación y cierre. El origen de entrada es el resultado de auditorías (ruta o fichero indicado).

## Proceso asociado

- **process_ref:** correccion-auditorias (paths.processPath/correccion-auditorias/). El ciclo de corrección sigue las fases de ese proceso e integra las acciones del ciclo feature (spec, clarify, planning, implementation, execution, validate, finalize-process).

## Orígenes de entrada (input_sources)

La plantilla espera un **origen de auditorías** que puede indicarse como:

| Clave | Descripción | Ejemplo |
|-------|-------------|---------|
| **audit_origin** | Ruta canónica Cúmulo, parcial o total. | `paths.auditsPath` (por defecto), o `docs/audits/`, o ruta absoluta. |
| **audit_file** | Fichero o patrón opcional. | `AUDITORIA_2026_02_23.md`, `validacion-*.json`, o omitir para usar todo el directorio. |

- **Rutas Cúmulo:** Usar `paths.auditsPath` para la carpeta canónica de informes (recomendado).
- **Rutas parciales:** Relativas al repo, ej. `docs/audits/`, `docs/audits/ultima/`.
- **Rutas totales:** Cuando se indique explícitamente un fichero o informe concreto en disco.

El agente que ejecute la plantilla resolverá estas referencias según la norma paths-via-cumulo cuando corresponda.

## Flujo de uso

1. **Análisis:** Revisar el origen indicado (paths.auditsPath o el fichero/ruta proporcionado) con los agentes Auditor (Back/Front/Process) según el tipo de informe.
2. **Objetivos:** Redactar objectives.md con hallazgos consolidados y prioridades (críticos/medios/bajos).
3. **Ciclo feature:** Ejecutar acciones spec → clarify → planning → implementation → execution → validate → finalize-process según paths.actionsPath y el proceso correccion-auditorias.
4. **Persistencia:** Documentación en paths.featurePath/<nombre_correccion>/ (Cúmulo). Rama: feat/correccion-segun-auditorias o feat/correccion-auditorias-<id>.

## Agentes y skills

- **Agentes:** Auditor (Back/Front/Process) para análisis; Arquitecto y Tekton para orquestación y corrección.
- **Skills:** suite táctica Git (git-workspace-recon, git-branch-manager, git-save-snapshot, git-sync-remote, git-tactical-retreat, git-create-pr) + invoke-commit + documentation + security-audit cuando aplique.

## Referencias

- paths.auditsPath (Cúmulo)
- paths.featurePath (Cúmulo)
- paths.processPath/correccion-auditorias/
- paths.processPath/feature/
- Proceso correccion-auditorias: spec.md en paths.processPath/correccion-auditorias/
