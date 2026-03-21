---
contract_ref: paths.processPath/process-contract.md
persist_ref: paths.featurePath/<nombre_feature>
phases:
- description: Rama feat desde master; skill iniciar-rama.
  id: '0'
  name: Preparar entorno
- description: objectives.md en carpeta de la tarea.
  id: '1'
  name: Documentación con objetivos
- description: Acción spec; salida spec.md (frontmatter YAML + Markdown).
  id: '2'
  name: Especificación
- description: Acción clarify; clarify.md (frontmatter YAML + Markdown).
  id: '3'
  name: Clarificación
- description: Acción planning; plan.md (frontmatter YAML + Markdown).
  id: '4'
  name: Planificación
- description: Acción implementation; implementation.md (frontmatter YAML + Markdown).
  id: '5'
  name: Implementación (doc)
- description: Acción execution; execution.md (frontmatter YAML + Markdown).
  id: '6'
  name: Ejecución
- description: Acción validate; validacion.md (frontmatter YAML + Markdown).
  id: '7'
  name: Validar
- description: Acción finalize; commits pendientes (invoke-commit), Evolution Logs, push y PR (finalizar-git pre_pr), merge en remoto, limpieza (finalizar-git post_pr).
  id: '8'
  name: Finalizar
principles_ref: paths.principlesPath
process_id: feature
related_actions:
- spec
- clarify
- planning
- implementation
- execution
- validate
- finalize
related_skills:
- iniciar-rama
- invoke-commit
- finalizar-git
spec_version: 1.0.0
---

# Proceso: Feature

Este documento define el **proceso de tarea** para desarrollar una funcionalidad. Está ubicado en paths.processPath/feature/ (Cúmulo). Las acciones que orquesta están en paths.actionsPath (Cúmulo). La ruta de persistencia se obtiene de **Cúmulo** (paths.featurePath/<nombre_feature>).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): solicita/genera en la carpeta de la tarea (Cúmulo) artefactos **`.md`** con frontmatter YAML + cuerpo Markdown (objectives.md, spec.md, clarify.md, plan.md, implementation.md, execution.md, validacion.md). **No se usan ficheros `.json` separados.** Patrón: SddIA/norms/features-documentation-pattern.md; contrato: paths.featurePath/features-contract.md.

## Propósito

El proceso **feature** define el procedimiento formal de ciclo completo para desarrollar una funcionalidad o tarea: desde la creación de la rama hasta el cierre y la apertura del Pull Request. Orquesta las acciones **spec**, **clarify**, **planning**, **implementation**, **execution**, **validate** y **finalize** en secuencia, fija la ubicación de la documentación de la tarea y garantiza trazabilidad en los logs de evolución.

Proporciona un flujo repetible y auditado, alineado con las Leyes Universales (soberanía documental en AGENTE_CUMULO, no commits en `master`).

## Alcance del procedimiento

Ruta de la tarea: Cúmulo (paths.featurePath/<nombre_feature>).

| Fase | Nombre | Descripción |
| :--- | :--- | :--- |
| **0** | Preparar entorno | Crear rama feat/<nombre_feature> (o `fix/` si aplica) desde `master` actualizado. No trabajar en `master`. **Skill:** iniciar-rama — invocar según contrato (paths.skillsDefinitionPath/iniciar-rama/). Tekton invoca la implementación (paths.skillCapsules[\"iniciar-rama\"]). Parámetros: BranchType feat, BranchName <nombre_feature>. |
| **1** | Documentación con objetivos | Documentar objetivo, alcance y ley aplicada. La documentación de la tarea se ubica en la carpeta de la tarea (Cúmulo)/objectives.md. |
| **2** | Especificación | Ejecutar o generar SPEC (acción **spec**). Entrada: requerimiento o borrador, carpeta de la tarea (Cúmulo)/objectives.md; salida: especificación técnica en carpeta de la tarea (Cúmulo)/spec.md (frontmatter YAML + Markdown). |
| **3** | Clarificación | Ejecutar o generar clarificaciones (acción **clarify**). Entrada: carpeta de la tarea (Cúmulo)/objectives.md, spec.md; salida: carpeta de la tarea (Cúmulo)/clarify.md (frontmatter YAML + Markdown). |
| **4** | Planificación | Ejecutar o generar plan (acción **plan**). Entrada: Especificación, Clarificación. Salida: carpeta de la tarea (Cúmulo)/plan.md (frontmatter YAML + Markdown). |
| **5** | Implementación | Generar documento de implementación. Entrada: carpeta de la tarea (Cúmulo)/objectives.md, spec.md, clarify.md; salida: carpeta de la tarea (Cúmulo)/implementation.md (frontmatter YAML + Markdown). |
| **6** | Ejecución | Aplicar el plan al código (Tekton Developer). Entrada: carpeta de la tarea (Cúmulo)/implementation.md; salida: carpeta de la tarea (Cúmulo)/execution.md (frontmatter YAML + Markdown). |
| **7** | Validar | Ejecutar validación pre-PR. Entrada: carpeta de la tarea (Cúmulo); salida: carpeta de la tarea (Cúmulo)/validacion.md (frontmatter YAML + Markdown). |
| **8** | Finalizar | Acción **finalize** (paths.actionsPath/finalize/spec.md): **(1)** commits pendientes vía skill **invoke-commit**; **(2)** Evolution Logs; **(3)** push y apertura de PR vía **finalizar-git** (pre_pr), script `Invoke-Finalize.ps1`; **(4)** merge del PR en el remoto; **(5)** unificación en master local y eliminación de rama de trabajo vía **finalizar-git** (post_pr). Entrada: carpeta de la tarea (Cúmulo). Salida: rama en origin, PR, repo limpio en master tras cierre. |

## Implementación

Este proceso se implementa como **procedimiento** que combina:

*   **Skills y agentes** para spec, clarify y plan (cuando se requiera trazabilidad con token de auditor), según las acciones definidas en paths.actionsPath (Cúmulo).
*   **Ubicación obligatoria de la documentación de la tarea:** paths.featurePath/<nombre_feature>/ (Cúmulo).

### Contenido mínimo de la carpeta de la tarea (Cúmulo: paths.featurePath/<nombre_feature>/)

| Documento | Contenido |
| :--- | :--- |
| **objectives.md** | Objetivo, alcance, ley aplicada (frontmatter YAML + Markdown). |
| **spec.md** | Especificación técnica (frontmatter YAML + Markdown). |
| **clarify.md** | Clarificaciones y decisiones (frontmatter YAML + Markdown). |
| **plan.md** | Plan de implementación / task roadmap (frontmatter YAML + Markdown). |
| **implementation.md** | Touchpoints y ítems de implementación (frontmatter YAML + Markdown). |
| **execution.md** | Registro de ejecución (frontmatter YAML + Markdown). |
| **validacion.md** | Informe de validación pre-PR (frontmatter YAML + Markdown). |

### Actualización de Evolution Logs

Al cierre de la feature (fase 8):

*   **paths.evolutionPath + paths.evolutionLogFile:** Añadir una línea con formato `[YYYY-MM-DD] [feat/<nombre>] [Descripción breve del resultado.] [Estado].`
*   **paths.evolutionPath + paths.evolutionLogFile:** Añadir una sección con fecha y título de la feature, resumen y referencia a la carpeta de la tarea (Cúmulo)/objectives.md.

## Integración con Agentes

*   **Arquitecto / Spec Architect:** Puede iniciar el procedimiento y asegurar que la fase 1 y la ubicación paths.featurePath/<nombre_feature>/ (Cúmulo) se respeten.
*   **Clarifier:** Responsable de la fase 3 (clarificación) y de persistir decisiones en el SPEC y en la carpeta de la feature.
*   **Tekton Developer:** Ejecuta las fases 4 (plan), 5 (implementación), 6 (ejecución), 7 (validación) y 8 (cierre/PR); aplica la SPEC como marco legal.
*   **Cúmulo:** Valida que la documentación de la tarea esté en paths.featurePath/<nombre_feature>/ como SSOT para esa feature.

## Dependencias con otras acciones

*   El proceso **feature** invoca o utiliza los resultados de las acciones **spec**, **clarify**, **plan**, **implementation**, **execution**, **validate** y **finalize** en paths.actionsPath (Cúmulo).
*   La **documentación de la tarea** (objetivo, spec, clarifications, plan, validacion) debe residir en **paths.featurePath/<nombre_feature>/** (Cúmulo) para aprobación y revisión humana.

## Estándares de Calidad

*   **Grado S+:** Trazabilidad desde el objetivo hasta el PR: rama → paths.featurePath → spec/clarify/plan → implementación → execution → validación → Evolution Logs → PR.
*   **Ley GIT:** Ningún commit en `master`; todo el trabajo en rama `feat/` o `fix/` con documentación en paths.featurePath/<nombre_feature>/ (Cúmulo).
*   **Single Source of Truth:** Para cada feature, la documentación canónica de la tarea es paths.featurePath/<nombre_feature>/ (Cúmulo); la referencia en PR y en Evolution Log es esa ruta.

## Alcance para Fix (bug)

El mismo patrón de persistencia se aplica a correcciones de bugs mediante el proceso **bug-fix**. La ubicación de la documentación se obtiene del agente Cúmulo (paths.fixPath/<nombre_fix>). Ver paths.processPath/bug-fix/.

## Referencia de ejecución

Procedimiento aplicado en la rama **feat/e2e-product-back-mocked** (2026-02-10). Documentación de la tarea: paths.featurePath/<nombre_feature>/. Acciones relacionadas: paths.actionsPath (spec/, clarify/, planning/, execution/, validate/, finalize/).
