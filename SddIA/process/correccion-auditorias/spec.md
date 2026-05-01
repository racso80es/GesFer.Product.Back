---
contract_ref: paths.processPath/process-contract.md
input_ref: paths.auditsPath
persist_ref: paths.featurePath/<nombre_correccion>
phases:
- description: Ejecutar git-workspace-recon para validar entorno limpio. Tras confirmar, crear rama feat/correccion-segun-auditorias o feat/correccion-auditorias-<id> con git-branch-manager.
  id: '0'
  name: Preparar entorno
- description: Revisar últimos informes en paths.auditsPath; consolidar hallazgos (críticos/medios/bajos).
  id: '1'
  name: Análisis de auditorías
- description: objectives.md con hallazgos priorizados y criterios de cierre.
  id: '2'
  name: Documentación de objetivos
- description: Acción spec; spec.md (frontmatter YAML + Markdown).
  id: '3'
  name: Especificación
- description: Acción clarify si aplica; clarify.md (frontmatter YAML + Markdown).
  id: '4'
  name: Clarificación
- description: Acción planning; plan.
  id: '5'
  name: Planificación
- description: Acción implementation; implementation.md (frontmatter YAML + Markdown).
  id: '6'
  name: Implementación (doc)
- description: Acción execution; execution.md (frontmatter YAML + Markdown). Consolidar hitos con git-save-snapshot; ante fallo estructural, git-tactical-retreat.
  id: '7'
  name: Ejecución
- description: Acción validate; validacion.md (frontmatter YAML + Markdown).
  id: '8'
  name: Validar
- description: Acción finalize. git-sync-remote; git-create-pr con objectives.md y validacion.md en el cuerpo del Pull Request. Evolution Logs.
  id: '9'
  name: Finalizar
principles_ref: paths.principlesPath
process_id: correccion-auditorias
related_actions:
- spec
- clarify
- planning
- implementation
- execution
- validate
- finalize
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
- documentation
- security-audit
spec_version: 2.0.0
---

# Proceso: Corrección según Auditorías

Este documento define el **proceso de tarea** para la corrección de hallazgos derivados de auditorías (paths.auditsPath). Está ubicado en paths.processPath/correccion-auditorias/ (Cúmulo). La ruta de persistencia se obtiene de **Cúmulo** (paths.featurePath/<nombre_correccion>).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): solicita/genera en la carpeta de la tarea (Cúmulo) artefactos **`.md`** con frontmatter YAML (objectives.md, spec.md, clarify.md, validacion.md). Sin .json separados. Patrón: SddIA/norms/features-documentation-pattern.md.

## Propósito

El proceso **correccion-auditorias** orquesta el ciclo de corrección de hallazgos reportados en los informes de auditoría (paths.auditsPath): análisis de auditorías recientes, priorización de hallazgos, documentación de objetivos, y ejecución de correcciones mediante feature o bug-fix según el tipo de hallazgo.

## Entrada

- **Fuentes:** Informes en paths.auditsPath (p. ej. AUDITORIA_YYYY_MM_DD.md, validacion-*.json).
- **Artefacto de análisis:** Documento de objetivos (objectives.md) que consolida hallazgos, prioridades y alcance.

## Alcance

- **Rama:** feat/correccion-segun-auditorias o feat/correccion-auditorias-<identificador> (nunca master).
- **Documentación:** Carpeta paths.featurePath/<nombre_correccion>/ con objectives.md, spec.md, clarify.md si aplica, implementation.md, validacion.md (todos con frontmatter YAML + Markdown). Sin .json separados.
- **Skills Git (suite táctica):** git-workspace-recon, git-branch-manager, git-save-snapshot, git-sync-remote, git-tactical-retreat, git-create-pr; más documentation, security-audit cuando aplique.
- **Restricciones:** Priorizar hallazgos críticos (compilación, seguridad, violación de capas); alcance acotado por lo reportado en auditorías.

## Fases

0. **Preparar entorno:** git-workspace-recon; git-branch-manager para la rama de corrección.
1. **Análisis de auditorías:** Revisar últimos informes en paths.auditsPath y consolidar hallazgos (críticos / medios / bajos).
2. **Documentación de objetivos:** Redactar objectives.md con hallazgos priorizados y criterios de cierre.
3. **Especificación y plan:** Acciones spec, clarify, planning según ciclo feature.
4. **Implementación y ejecución:** Aplicar correcciones; git-save-snapshot para hitos; git-tactical-retreat si aplica.
5. **Validación y cierre:** validacion.md; git-sync-remote y git-create-pr enlazando artefactos; registrar en paths.auditsPath o Evolution Logs que los hallazgos fueron abordados.

## Integración

- Los agentes **Auditor (Back/Front/Process)** generan los informes en paths.auditsPath.
- El ciclo de corrección lo orquestan **Arquitecto** y **Tekton** usando este proceso.
- Referencia canónica de la tarea: paths.featurePath/<nombre_correccion>/ (Cúmulo). SSOT para esa ronda de corrección.

## Referencias

- paths.auditsPath (Cúmulo)
- paths.featurePath (Cúmulo)
- Proceso feature: paths.processPath/feature/
- AGENTS.md — Leyes Universales (COMPILACIÓN, GIT, Soberanía documental)
