# Procesos de tarea (SddIA/process)

Al iniciar una tarea se debe elegir un **proceso**. Cada proceso define el ciclo completo (rama, documentación, especificación, implementación, validación y cierre) y la ubicación de persistencia según el agente **Cúmulo** (paths; ver paths.processPath).

Todo proceso cumple el **contrato** en [process-contract.json](./process-contract.json) y [process-contract.md](./process-contract.md): definición en carpeta `<process-id>/` con spec.md y spec.json.

## Procesos disponibles

| Proceso      | Descripción                                                                 | Definición                                      | Agente principal        |
| :---         | :---                                                                        | :---                                             | :---                    |
| **feature**  | Desarrollo de una funcionalidad: rama `feat/<nombre_feature>`, documentación en paths.featurePath/<nombre_feature>/ (Cúmulo). | [feature/](./feature/) (spec.md, spec.json)     | Arquitecto, Tekton      |
| **bug-fix**  | Corrección de un bug: rama `fix/<nombre_fix>`, documentación en paths.fixPath/<nombre_fix>/ (Cúmulo). Alcance mínimo. | [bug-fix/](./bug-fix/)                           | Bug Fix Specialist      |
| **refactorization** | Refactorización: rama `feat/refactorization-<nombre_refactor>`, documentación en paths.featurePath/refactorization-&lt;nombre_refactor&gt;/ (Cúmulo). | [refactorization/](./refactorization/)           | Arquitecto, Tekton      |
| **create-tool** | Creación de herramienta: rama `feat/create-tool-<tool-id>`, cápsula en paths.toolCapsules. | [create-tool/](./create-tool/)                   | Tekton, Arquitecto      |
| **correccion-auditorias** | Corrección de hallazgos de auditoría: rama `feat/correccion-segun-auditorias` o `feat/correccion-auditorias-<id>`, documentación en paths.featurePath. Entrada: paths.auditsPath. | [correccion-auditorias/](./correccion-auditorias/) | Arquitecto, Tekton      |
| **create-pattern** | Creación de patrón de diseño: carpeta en paths.patternsPath con spec.md y spec.json. | [create-pattern.json](./create-pattern.json)     | Arquitecto              |
| **create-principle** | Creación de principio técnico: carpeta en paths.principlesPath con spec.md y spec.json. Principios con blocking_for_pr bloquean PR si falla validate. | [create-principle.json](./create-principle.json) | Cúmulo, Arquitecto      |
| **create-template** | Creación de plantilla: rama feat/create-template-&lt;template-id&gt;, carpeta en paths.templatesPath con spec.md y spec.json. Configuración predefinida de un proceso con fin concreto. | [create-template/](./create-template/)           | Cúmulo, Arquitecto      |
| **audit-tool** | Auditoría de herramienta: verificación empírica del funcionamiento de una tool. Resultado: informe en paths.auditsPath/tools/&lt;tool-id&gt;/. | [audit-tool/](./audit-tool/)                     | Auditor, Arquitecto     |
| **create-skill** | Creación de skill: rama feat/create-skill-&lt;skill-id&gt;, cápsula en paths.skillCapsules. Implementación obligatoria en Rust (.exe). | [create-skill/](./create-skill/)                   | Tekton, Arquitecto      |
| **validate-pull-requests** | Validación integral de PR (S+ Grade): revisión multi-agente (architect, qa-judge, security-engineer) sobre la rama origen; informe en paths.featurePath; semillas Kaizen en paths.tasksPath. | [validate-pull-requests/](./validate-pull-requests/) | Arquitecto, QA-Judge, Security-Engineer |

## Uso

1. **Feature:** paths.processPath/feature/. Ruta: Cúmulo (paths.featurePath/<nombre_feature>).
2. **Bug-fix:** paths.processPath/bug-fix/. Ruta: Cúmulo (paths.fixPath/<nombre_fix>).
3. **Refactorization:** paths.processPath/refactorization/. Ruta: Cúmulo (paths.featurePath/refactorization-&lt;nombre_refactor&gt;).
4. **Create-tool:** paths.processPath/create-tool/. Ruta: Cúmulo (paths.featurePath/create-tool-&lt;tool-id&gt;). Entregable: cápsula en paths.toolsPath/&lt;tool-id&gt;/.
5. **Corrección según auditorías:** paths.processPath/correccion-auditorias/. Ruta: Cúmulo (paths.featurePath/&lt;nombre_correccion&gt;). Entrada: paths.auditsPath (informes de auditoría).
6. **Create-template:** paths.processPath/create-template/. Ruta: Cúmulo (paths.featurePath/create-template-&lt;template-id&gt;). Entregable: paths.templatesPath/&lt;template-id&gt;/ (spec.md, spec.json).
7. **Audit-tool:** paths.processPath/audit-tool/. Ruta: Cúmulo (paths.featurePath/audit-tool-&lt;tool-id&gt;). Resultado: paths.auditsPath/tools/&lt;tool-id&gt;/ (audit-report.md, audit-result.json).
8. **Create-skill:** paths.processPath/create-skill/. Ruta: Cúmulo (paths.featurePath/create-skill-&lt;skill-id&gt;). Entregable: cápsula **paths.skillCapsules[&lt;skill-id&gt;]** (resuelta bajo paths.skillsPath/&lt;skill-id&gt;/ según Cúmulo).
9. **Validate-pull-requests:** paths.processPath/validate-pull-requests/. Ruta: Cúmulo (paths.featurePath/validate-pull-requests-&lt;pr-slug&gt;). Semillas Kaizen: paths.tasksPath.

Las **acciones** (spec, clarify, plan, implementation, execution, validate, finalize-process) siguen en paths.actionsPath (Cúmulo) y son invocadas por los procesos.

## Interfaz de procesos (norma para agentes)

Todo proceso debe cumplir la **interfaz** definida en Cúmulo (Cúmulo → process_interface): solicitar o generar en la carpeta de la tarea (Cúmulo) artefactos con las extensiones:

| Formato | Uso |
| :--- | :--- |
| **`.md` con frontmatter YAML** | Un fichero por acción: objectives.md, spec.md, clarify.md, plan.md, implementation.md, execution.md, validacion.md. Sin ficheros `.json` separados. Patrón: SddIA/norms/features-documentation-pattern.md. |

Los agentes de proceso y los procedimientos (paths.processPath/<process-id>/) documentan qué artefactos requieren o producen en la carpeta de la tarea.
