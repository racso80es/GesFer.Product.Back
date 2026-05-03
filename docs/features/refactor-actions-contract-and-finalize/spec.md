---
feature_name: refactor-actions-contract-and-finalize
created: '2026-05-01'
process: feature
artifact: spec
base_branch: main
target_branch: feat/refactor-actions-contract-and-finalize
---

# Especificación técnica

## Contexto

El contrato actual (`SddIA/actions/actions-contract.md`) lista acciones pero no establece de forma **innegociable** que una acción **no ejecuta** código de SO ni scripts. La acción `finalize` (`SddIA/actions/finalize/spec.md`) referencia `Invoke-Finalize.ps1`, `cargo run`, y flujos que violan la separación “acción = orquestación documental / skills = ejecución”.

## Requisitos funcionales

### R1 — Contrato de acciones (jurisdicción)

- Añadir al contrato una sección **“Jurisdicción de las acciones”**:
  - Una acción **no** invoca procesos del sistema operativo, **no** ejecuta scripts (`.ps1`, `.bat`, `.sh`) y **no** lanza binarios salvo que esos binarios estén publicados como **skill** o **tool** en Cúmulo y la acción solo **nombre** la skill/tool y el orden de invocación.
  - Las acciones **orquestan** pasos; la **ejecución** recae en skills/tools (implementación Rust estándar según `SddIA/constitution.json`).

### R2 — Renombrado `finalize` → `finalize-process`

- **action_id:** `finalize-process` (kebab-case).
- **Carpeta:** `paths.actionsPath/finalize-process/` con `spec.md` (frontmatter YAML + cuerpo).
- **Propósito:** Cierre del ciclo de proceso/tarea: precondiciones, trazabilidad en logs, orquestación de cierre Git mediante skills explícitas (sin scripts de acción).
- **Disparadores naturales (documentales):** alineados con “proceso finalizado”, “tarea finalizada”, “cierre del ciclo”, “publicar rama y abrir PR” en el marco de la acción (detalle en norma `interaction-triggers`).

### R3 — Eliminación de ejecución directa en la spec de cierre

- Eliminar del texto de la acción:
  - Referencias a `implementation_script_ref` hacia `.ps1`.
  - Comandos literales tipo `cargo run`, `.\scripts\...ps1` como pasos obligatorios de la acción.
- Sustituir por una **matriz de orquestación**: p. ej. validación pre-PR vía skill declarada (`verify-pr-protocol` si permanece como skill), luego `git-save-snapshot` / `invoke-commit`, `git-sync-remote`, `git-create-pr`; emergencia `git-tactical-retreat`.

### R4 — Referencias en el ecosistema

Actualizar consumidores conocidos (lista de barrido inicial vía búsqueda `finalize` en `SddIA/`, `.cursor/rules`, `AGENTS.norms.md`):

- Procesos: `feature`, `bug-fix`, `refactorization`, `create-tool`, `create-skill`, `create-template`, `automatic_task`, `correccion-auditorias`, plantillas asociadas, `README` de `paths.processPath`.
- Normas: `interaction-triggers.md`, `commands-via-skills-or-tools.md`, `git-via-skills-or-process.md`, `features-documentation-pattern.md` (artefacto de cierre).
- Acciones: `sddia-difusion`, `README` de `paths.actionsPath`.
- Skills definición: p. ej. `git-operations/spec.md` (mapeo finalize → finalize-process).
- Reglas Cursor: `action-suggestions.mdc`, `features-documentation-pattern.mdc`, `subir-push.mdc`, `sddia-ssot.mdc` si listan acciones.

### R5 — Compatibilidad y migración

- No mantener carpeta `finalize/` duplicada: una sola fuente `finalize-process/`.
- Donde un JSON de proceso liste `related_actions`, reemplazar `finalize` por `finalize-process`.
- Si algún enlace histórico debe conservarse, una línea de “obsoleto: usar finalize-process” en evolución SDdIA, no en contrato activo.

## Criterios de aceptación

- [ ] `actions-contract.md` incluye la norma de no ejecución directa y lista `finalize-process` (no `finalize` como acción activa).
- [ ] Existe `SddIA/actions/finalize-process/spec.md` sin rutas a `.ps1`/`.bat` como implementación de la acción.
- [ ] No quedan referencias operativas a `paths.actionsPath/finalize/` en procesos/normas/reglas (salvo nota de migración en evolution si aplica).
- [ ] Disparadores / tabla `#Action` mencionan `finalize-process` y descripción de cierre acorde.
- [ ] Tras mutación en `SddIA/`: registro en `SddIA/evolution/` vía **sddia-evolution-register** cumpliendo contrato de evolución.

## Riesgos

- **Rotura de documentación o CI** que asuma el string `finalize` en rutas: mitigar con búsqueda global y pruebas de lint frontmatter si existen.
- **Binarios skills ausentes** en entornos nuevos: fase 0 Git documentada en `objectives.md`.
