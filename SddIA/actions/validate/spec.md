---
action_id: validate
blocking_principles:
- nomenclatura
contract_ref: actions-contract.md
flow_steps:
- Validación git (siempre)
- Contexto
- Checks obligatorios (incl. nomenclatura)
- Opcionales
- Informe
- Auditoría
inputs:
- Carpeta tarea (Cúmulo)
- Rama actual
optional_checks:
  sddia_frontmatter_valid: 'Si el diff toca SddIA/skills, SddIA/process, paths.patternsPath, paths.principlesPath, paths.featurePath, paths.fixPath, etc., comprobar que el .md tenga frontmatter YAML válido. Ref: entidades-dominio-ecosistema-sddia.md, features-documentation-pattern.md.'
outputs:
- validacion.md con frontmatter YAML + cuerpo Markdown en carpeta tarea (Cúmulo) o paths.auditsPath. Sin validacion.json. Patrón: SddIA/norms/features-documentation-pattern.md.
principles_ref: paths.principlesPath
---

# Action: Validate

## Propósito

La acción **validate** (validación) comprueba que la feature cumple los criterios de calidad antes del cierre y de la apertura del Pull Request. **Siempre** incluye una **validación de cambios git** (diff frente a la rama base), además de compilación, tests, reglas de documentación y, si aplica, comprobaciones de seguridad o rendimiento. El resultado se persiste en un artefacto estructurado para que la fase de finalización y los revisores sepan el estado de la rama y el alcance de los cambios.

## Principio

- **Asumir que está roto:** No se aprueba hasta que las comprobaciones definidas pasen.
- **Evidencia auditable:** Todo resultado (éxito/fallo por categoría) se registra en un archivo de validación.
- **Bloqueo explícito:** Si la validación falla, la acción debe dejar constancia y el proceso de finalización (finalize-process) puede bloquear el PR hasta corrección.

## Entradas

- **Carpeta de la feature:** Ruta obtenida de Cúmulo (ej. paths.featurePath/<nombre_feature>/ o paths.fixPath/<nombre_fix>/). Opcional: si no existe documentación de tarea, se aplica el **modo sin documentación** (véase más abajo).
  - Se usan como contexto: `objectives.md`, `spec.md`, `clarify.md`, `implementation.md`, `execution.md` (si existen) para saber alcance y qué se ha tocado.
- **Rama actual:** La validación se ejecuta sobre la rama de la feature/fix (feat/ o fix/), nunca sobre `master`. En modo sin documentación, la rama actual y el diff frente a la base definen el alcance.

## Salidas

- **Informe de validación:** validacion.md en la carpeta de la tarea (Cúmulo) si existe; si no, paths.auditsPath + validacion-<rama>-<timestamp>.md.
  - Estructura mínima (siempre):
    - **timestamp**, **branch**, **base_branch** (rama de referencia para el diff).
    - **git_changes:** resultado de la validación de cambios git (obligatorio en toda ejecución). Ver más abajo.
    - **checks:** lista de comprobaciones (git_changes, build, test, documentation, ley_git, …) con nombre, resultado (pass | fail | warn), mensaje opcional, detalle.
    - **global:** pass | fail (pass solo si las comprobaciones obligatorias pasan).
    - **blocking:** si hay fallos que impiden el PR.
  - Formato: frontmatter YAML + cuerpo Markdown (validacion.md). Sin validacion.json.
  - Opcional: copia o resumen en paths.auditsPath para historial.

### Validación de cambios git (siempre)

En **toda** ejecución de validate se debe:

1. Obtener la **rama base** de referencia (por defecto `main` o `master`, o por parámetro).
2. Calcular el **diff** entre la rama actual y la rama base: `git diff --name-status <base_branch>` (y, si hay cambios no commiteados, incluirlos con `git status` o diff contra HEAD).
3. Clasificar archivos en **files_added**, **files_modified**, **files_deleted** y, opcionalmente, un **summary_by_category** (p. ej. por prefijo: `src/`, `SddIA/`, `docs/`, configuración).
4. Incluir en el informe la sección **git_changes** con esa estructura; opcionalmente, estadísticas de líneas (ej. `git diff --shortstat`) si se dispone.
5. Registrar un check **git_changes** con resultado pass (si el diff se obtuvo correctamente) y usar ese análisis como contexto de alcance para el resto de checks.

Estructura sugerida de **git_changes** en el informe:

```json
"git_changes": {
  "base_branch": "main",
  "files_added": [],
  "files_modified": [],
  "files_deleted": [],
  "summary_by_category": {},
  "shortstat": "+n -m"
}
```

## Modo sin documentación (cuando no hay carpeta de tarea)

Cuando **no existe** carpeta de tarea (ruta no proporcionada o vacía):

1. La **validación de cambios git** se ejecuta igual (es obligatoria siempre).
2. El informe se persiste en **paths.auditsPath + validacion-<rama>-<timestamp>.json**.
3. El check **documentación** puede figurar como **warn** en lugar de fail, con mensaje explícito de que no hay documentación de tarea (Cúmulo). La compilación y los tests siguen siendo obligatorios (pass/fail).
4. En el informe puede añadirse **mode:** `"no_persist"` para indicar que no había carpeta de feature/fix.

## Flujo de ejecución (propuesto)

1. **Validación de cambios git (siempre):** Obtener rama actual y rama base; ejecutar `git diff --name-status <base_branch>` (y considerar cambios no commiteados). Construir `git_changes` (files_added, files_modified, files_deleted, summary_by_category, opcionalmente shortstat). Registrar check `git_changes` = pass si el análisis se completó.
2. **Contexto:** Resolver la carpeta de la tarea (por parámetro o por convención desde el nombre de la rama; Cúmulo). Si no hay carpeta de tarea, activar **modo sin documentación** (persistir en paths.auditsPath, documentación = warn).
3. **Comprobaciones obligatorias (mínimo):**
   - **Compilación:** `dotnet build` (backend) y, si aplica, build del frontend.
   - **Tests:** `dotnet test` (y tests E2E/linting si están definidos en el proyecto).
   - **Documentación:** Existencia y completitud mínima de `objectives.md` en la carpeta de la tarea (Cúmulo) (y opcionalmente spec, clarify, plan). En modo sin documentación: resultado warn.
   - **Ley GIT:** La rama actual no es `master`/`main` (trabajo en feat/ o fix/).
   - **Nomenclatura (principio bloqueante):** Check según paths.principlesPath/nomenclatura/ (principle_id: nomenclatura, definido por Cúmulo). Comprobar nombre de rama (prefijo feat/ o fix/, resto en kebab-case) y, cuando aplique, formato de commits. Si falla: **blocking: true**; el PR no debe aprobarse. **Implementación:** ejecutar `scripts/validate-nomenclatura.ps1` (rama; opcional `-CheckCommits`). Exit 0 = pass; exit 1 = fail. Incluir salida del script en el check `nomenclatura` del frontmatter de validacion.md. Ver spec: `blocking_principles: ["nomenclatura"]`.
4. **Comprobaciones opcionales (según proyecto):**
   - Script de validación de PR: según skill o herramienta (Cúmulo).
   - Reglas de seguridad (Security Engineer) o rendimiento (Performance Engineer).
   - **Frontmatter válido (SddIA):** Si el diff toca entidades de dominio (SddIA/skills, SddIA/process, SddIA/patterns, SddIA/principles, etc.), comprobar que el .md tenga frontmatter YAML válido. Registrarlo como check `sddia_frontmatter_valid` (pass | warn). Ref: SddIA/norms/entidades-dominio-ecosistema-sddia.md.
5. **Generación de informe:** Construir el resultado incluyendo **siempre** `git_changes` y todos los checks; persistir en validacion.md (frontmatter YAML + Markdown) en la carpeta de la tarea (Cúmulo) o en paths.auditsPath + validacion-<rama>-<timestamp>.md si no hay carpeta de tarea.
6. **Auditoría:** Registrar la ejecución de validate en paths.auditsPath + paths.accessLogFile o en log de evolución (paths.evolutionPath).

## Implementación técnica (opcional)

La validación puede ejecutarse mediante scripts existentes (ej. `validate-pr.ps1`) y un agente que interprete la salida y genere `validacion.md`. Parámetros típicos:

- `--persist` (o equivalente): ruta de la carpeta de la feature/fix (ej. paths.featurePath/<nombre_feature>/). Si no se indica, se usa modo sin documentación.
- `--base-branch`: rama de referencia para el diff (por defecto `main` o `master`). La validación de cambios git siempre se ejecuta contra esta rama.
- `--token`: (opcional) token de auditoría.

## Integración con agentes

- **QA Judge (validador principal):** Responsable de ejecutar esta acción. Aplica la política "asumir que está roto"; exige tests y documentación; bloquea el proceso si faltan evidencias o la compilación falla. Genera `validacion.md` y, en caso de fallo repetido, puede generar `AUDIT_FAIL.md` y solicitar intervención humana.
- **Tekton Developer:** Puede invocar validate tras execution para comprobar que sus cambios no rompen nada antes de pedir el PR.
- **Security Engineer / Performance Engineer:** Pueden aportar comprobaciones adicionales cuyos resultados se integren en el mismo `validacion.md` o en informes vinculados.

## Agente responsable (referencia para definición de agente)

| Concepto | Descripción |
| :--- | :--- |
| **Id sugerido** | `qa-judge` (ya existente). Es el validador final antes del PR. |
| **Rol** | Validador: ejecutar checks, generar `validacion.md`, bloquear si no hay pruebas o la compilación falla. |
| **Skills necesarios** | `dotnet-development`, `git-operations`, `documentation`, y acceso a scripts de validación (invoke-command si ejecuta scripts). |
| **Restricciones** | Bloquear si falta documentación de rama; bloquear si faltan tests para lógica nueva; bloquear si la compilación falla. Circuit breaker tras N fallos. |

No se requiere un agente nuevo: **QA Judge** asume la fase de validación. Si se desea un agente dedicado solo a "ejecutar checks y rellenar validacion.md", podría definirse un **Validation Runner** que delegue en QA Judge la decisión de bloqueo.

## Estándares de calidad

- **Grado S+:** Trazabilidad desde execution hasta validacion.md; el PR solo se considera listo si `validacion.md` indica global pass y no blocking.
- **Validación git obligatoria:** Todo informe incluye `git_changes`; no hay ejecución de validate sin análisis de diff frente a la rama base.
- **Reproducibilidad:** Misma rama y mismo contexto deben producir el mismo resultado de validación (salvo flakiness de tests).
- **Single Source of Truth:** Para el estado de la feature antes del PR, el artefacto canónico es validacion.md en la carpeta de la tarea (Cúmulo) (o el generado en paths.auditsPath si no hay carpeta de tarea).
- **Frontmatter SddIA:** Las entidades de dominio usan .md con frontmatter YAML; validate puede incluir el check opcional `sddia_frontmatter_valid` cuando el diff toque paths.skillsDefinitionPath, paths.processPath u otras rutas de entidades.

## Dependencias con otras acciones

- **execution:** Proporciona `execution.md`; validate puede usarlo para saber qué archivos/cambios revisar o priorizar tests.
- **finalize-process:** Consume `validacion.md`; si global es fail o blocking es true, finalize-process no debe hacer push/PR sin advertencia o bloqueo.

## Resumen

- **Validación de cambios git:** Se ejecuta **siempre**; el informe incluye en toda ejecución la sección `git_changes` en el frontmatter (diff frente a rama base: archivos añadidos, modificados, eliminados y resumen por categoría).
- Con carpeta de tarea (Cúmulo): además de git_changes, checks de build, test y documentación; salida en validacion.md en esa carpeta.
- Sin carpeta de tarea: mismo flujo; salida en paths.auditsPath + validacion-<rama>-<timestamp>.md; el check de documentación es warn en lugar de fail.

---
*Documento de definición de la acción Validate. Corresponde a la fase 7 del procedimiento feature (validar antes de cierre y PR).*
