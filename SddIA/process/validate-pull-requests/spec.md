---
contract_ref: paths.processPath/process-contract.md
inputs:
- description: Identificador o ruta parcial del Pull Request en el remoto (origin).
  name: pr_path_or_id
  required: true
  type: string
- description: Nombre de la rama origen del PR (feature branch) cuyo estado debe analizarse.
  name: pr_branch_name
  required: true
  type: string
- description: Diff o lista de archivos modificados, añadidos o eliminados en el PR.
  name: code_diff
  required: true
  type: string
- description: Documentación SddIA, tickets o READMEs relevantes para el alcance del cambio.
  name: document_context
  required: false
  type: string
- description: Directorio destino para tareas Kaizen generadas (Cúmulo paths.tasksPath).
  name: tasks_path_cumulo
  required: true
  type: path_ref
outputs:
- description: Objetivos y alcance de la revisión (frontmatter YAML + Markdown).
  name: objectives.md
  type: file
- description: Informe de consenso multi-agente y veredicto (frontmatter YAML + Markdown); cumple rol de validación del ciclo.
  name: validacion.md
  type: file
- description: Semillas Kaizen opcionales; un fichero por tema en paths.tasksPath.
  name: '[YYYYMMDD]-Refactor-[Tema].md'
  type: file_pattern
persist_ref: paths.featurePath/validate-pull-requests-<pr-slug>
phases:
- description: Verificar contexto Karma2Token según contrato (paths.tokensPath). Sincronizar el working tree con la rama origen del PR solo mediante skill, herramienta, acción o proceso (norma SddIA/norms/git-via-skills-or-process.md); p. ej. paths.skillCapsules.invoke-command u orquestación equivalente documentada. Confirmar que el análisis se aplicará al código propuesto en el PR, no a la rama de integración destino.
  id: '0'
  name: Preparar contexto y rama del PR
- description: Escrutinio de alineación y estructura (Clean Architecture, separación de responsabilidades, modularidad, ecosistema de nombres). Agente SddIA/agents/architect.json (System Architect).
  id: '1'
  name: Escrutinio architect
- description: Escrutinio de necesidad, ausencia de alucinación (APIs/símbolos existentes en la rama), estados frontera y rendimiento. Agente SddIA/agents/qa-judge.json.
  id: '2'
  name: Escrutinio qa-judge
- description: Escrutinio de inmunidad (exposición de datos, sanitización, vulnerabilidades). Hallazgos bloqueantes. Agente SddIA/agents/security-engineer.json.
  id: '3'
  name: Escrutinio security-engineer
- description: Integrar dictámenes, aplicar criterios de veredicto y redactar el informe en el formato de consenso definido en este spec.
  id: '4'
  name: Consenso e informe final
- description: Persistir validacion.md bajo persist_ref. Escribir semillas Kaizen en paths.tasksPath con nombre [YYYYMMDD]-Refactor-[Tema].md y plantilla indicada en este documento.
  id: '5'
  name: Persistencia y Cúmulo Kaizen
principles_ref: paths.principlesPath
process_id: validate-pull-requests
related_actions:
- spec
- validate
related_agents:
- ref: SddIA/agents/architect.json
  role: architect
- ref: SddIA/agents/qa-judge.json
  role: qa-judge
- ref: SddIA/agents/security-engineer.json
  role: security-engineer
related_skills:
- invoke-command
norms_ref:
- SddIA/norms/git-via-skills-or-process.md
- SddIA/norms/paths-via-cumulo.md
spec_version: 1.0.0
name: Validación integral de Pull Requests (S+ Grade)
description: >-
  Nodo de control que orquesta architect, qa-judge y security-engineer sobre la rama origen del PR;
  produce informe de consenso y semillas Kaizen en paths.tasksPath.
---

# Proceso: Validación integral de Pull Requests (validate-pull-requests)

Este documento define el **proceso de tarea** para revisar un Pull Request con criterio **S+ Grade**. Está ubicado en paths.processPath/validate-pull-requests/ (Cúmulo). Las rutas de persistencia y tareas se obtienen de **Cúmulo** (paths.featurePath, paths.tasksPath, paths.tokensPath).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): genera en la carpeta de la tarea (Cúmulo) artefactos **`.md`** con frontmatter YAML + cuerpo Markdown (objectives.md, validacion.md). **No se usan ficheros `.json` separados** para la documentación de la revisión. Patrón: SddIA/norms/features-documentation-pattern.md.

## Identidad y propósito

Actúas como **nodo de control** que orquesta tres especialistas:

| Perspectiva | Agente (definición) |
|-------------|---------------------|
| Alineación y estructura | SddIA/agents/architect.json |
| Lógica y resiliencia | SddIA/agents/qa-judge.json |
| Inmunología (seguridad) | SddIA/agents/security-engineer.json |

Propósito: asegurar el desarrollo Kaizen del código mediante revisión disciplinada antes de integrar.

### Regla de oro (rama del PR)

Debes operar **exclusivamente sobre la rama origen del Pull Request** (la rama que propone los cambios). Antes de analizar:

1. **Sincroniza** el contexto de trabajo con esa rama (vía skill/herramienta/acción/proceso autorizado; ver SddIA/norms/git-via-skills-or-process.md).
2. **Verifica** que el árbol de trabajo y el diff analizado corresponden al **estado propuesto en el PR**, no a la rama de integración final ni a un merge local no publicado.

Toda alteración o sugerencia debe estar contenida en este entorno aislado (la revisión no mezcla cambios ajenos al PR).

## Alcance del procedimiento

- **Documentación de la tarea de revisión:** Cúmulo (paths.featurePath/validate-pull-requests-&lt;pr-slug&gt;/).
- **Semillas Kaizen (refactors diferidos):** `paths.tasksPath` (nombre de fichero: `[YYYYMMDD]-Refactor-[Tema].md`).

## Fases del proceso

| Fase | Nombre | Descripción |
|:-----|:-------|:------------|
| 0 | Preparar contexto y rama del PR | Token/trazabilidad; checkout o equivalente autorizado a la rama del PR; confirmación de alcance. |
| 1 | Escrutinio architect | Arquitectura: Clean Architecture y separación de responsabilidades. Modularidad y estándar: reutilización y ecosistema de nombres. Si la mejora es válida pero fuera de alcance del PR, marcar como **Semilla Kaizen** para Cúmulo. |
| 2 | Escrutinio qa-judge | Necesidad: el cambio corresponde a lo solicitado. Ausencia de alucinación: llamadas y APIs existen en la rama actual. Estados frontera y rendimiento. |
| 3 | Escrutinio security-engineer | Exposición de datos, sanitización de inputs, vulnerabilidades. **Hallazgos de seguridad bloqueantes (🔴).** |
| 4 | Consenso e informe final | Unificar dictámenes y generar el formato de salida obligatorio (ver sección siguiente). |
| 5 | Persistencia y Cúmulo Kaizen | objectives.md y validacion.md en persist_ref; ficheros Kaizen en paths.tasksPath cuando aplique. |

## Criterios de veredicto

| Veredicto | Condición |
|-----------|-----------|
| 🟢 **APROBADO** | Sin hallazgos bloqueantes; mejoras opcionales solo como Semillas Kaizen. |
| 🟡 **REQUIERE CAMBIOS** | Hallazgos no bloqueantes que deben corregirse en la rama del PR antes de merge. |
| 🔴 **RECHAZADO** | Cualquier hallazgo de seguridad bloqueante **o** error crítico de arquitectura/QA que impida integrar el PR en su estado actual. |

**Regla:** Un rechazo de seguridad o un fallo crítico architect/QA implica **🔴** inmediato.

## Salida esperada (formato de consenso y acción)

Tras finalizar el análisis sobre la rama indicada, el contenido de **validacion.md** (y la respuesta al usuario) debe seguir esta estructura:

### Veredicto Final: [ 🟢 APROBADO | 🟡 REQUIERE CAMBIOS | 🔴 RECHAZADO ]

*(Un rechazo de Seguridad o un error crítico de Arquitectura/QA supone un 🔴 inmediato).*

### 1. Resumen de Asimilación

* Explicación de 2 líneas sobre lo que aporta este PR al sistema.

### 2. Dictámenes Especializados

* **Reporte Architect:** [Aprobado/Rechazado] en la rama `[nombre_rama]`.
* **Reporte QA-Judge:** [Aprobado/Rechazado].
* **Reporte Security-Engineer:** [Aprobado/Rechazado].

### 3. Hallazgos Bloqueantes (Frenan el PR)

* Tabla con: **Agente**, **Archivo**, **Severidad** y **Justificación** de por qué requiere cambios inmediatos en la rama del PR.

### 4. Semillas Kaizen (Refactors Diferidos a Cúmulo)

Si se han detectado mejoras no bloqueantes, generar el contenido en formato Markdown para `paths.tasksPath`.

**Ruta sugerida:** `[paths.tasksPath]/[YYYYMMDD]-Refactor-[Tema].md`

**Contenido del archivo:**

```markdown
# Tarea Automatizada: Refactorización de [Tema]
**Origen:** PR [ID/Path] (Rama: [nombre_rama])
**Agente Detector:** [architect / qa-judge]

## Contexto
Se identificó entropía estructural en `[Archivo]` que puede ser optimizada.

## Objetivo del Refactor (S+ Grade)
[Explicación de 3 líneas].

## Instrucciones para Tekton/Jules
1. [Paso 1]
2. [Paso 2]
```

## Contenido mínimo de la carpeta de la tarea (Cúmulo: paths.featurePath/validate-pull-requests-&lt;pr-slug&gt;/)

| Documento | Contenido |
|-----------|-----------|
| **objectives.md** | Objetivo de la revisión, identificador del PR, rama, y confirmación de sincronización con la rama del PR (frontmatter YAML + Markdown). |
| **validacion.md** | Informe de consenso con el formato de la sección «Salida esperada» (frontmatter YAML + Markdown). |

## Integración con agentes y normas

* **Arquitecto, QA-Judge, Security-Engineer:** Cargar definiciones desde SddIA/agents/ según la tabla de identidad (misma convención que AGENTS.md).
* **Cúmulo:** Valida rutas vía paths-via-cumulo; paths.tasksPath para semillas Kaizen.
* **Principios:** Coherencia paths.principlesPath (principles-contract) en diseño cuando aplique.

## Referencias

* Contrato de procesos: paths.processPath/process-contract.md
* Git solo vía skills/proceso: SddIA/norms/git-via-skills-or-process.md
* Cola de tareas y Kaizen: paths.processPath/automatic_task/spec.md (paths.tasksPath)
