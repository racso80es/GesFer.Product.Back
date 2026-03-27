---
title: Replicación de cambios SddIA (commits e5bcaad, d38e4e3, 2cdaf21)
doc_type: replication-guide
spec_version: 1.0.0
source_repository: GesFer.Product.Back
source_commits:
  - e5bcaada74639242c823479cca88d97c14152bf4
  - d38e4e3643fb1761bd268c058697989ce52e8e36
  - 2cdaf21a2330de7f5cd05ec61854466eb77b8e94
generated_date: 2026-03-27
---

# Replicar en otro sistema SddIA los cambios de estos commits

Este documento resume el **contenido** y el **orden** de los tres hashes para poder aplicar el mismo resultado en otro repositorio que siga SddIA (rutas vía Cúmulo: `SddIA/agents/cumulo.paths.json`).

## Orden cronológico en `main` (del más antiguo al más reciente)

| Orden | Hash corto | Mensaje |
|------|------------|---------|
| 1 | `e5bcaada` | feat(automatic-task): add fallback to kaizen when no tasks available |
| 2 | `d38e4e36` | fix(ci): add missing sddia_evolution binaries and fix pr-validation workflow |
| 3 | `2cdaf21a` | Merge pull request #12 (integra la rama de feature con `main`) |

Entre (2) y (3) existe en el historial un merge intermedio `4df2a9b` (“Merge branch 'main' into feature/…”) que **no** está en tu lista; al portar cambios suele bastar con aplicar **(1)** y **(2)** en ese orden. El commit **(3)** es un **merge** a `main`; no es necesario “replicarlo” como commit si ya aplicas los cambios equivalentes de la PR.

---

## 1. `e5bcaada74639242c823479cca88d97c14152bf4` — Kaizen en proceso Automatic Task

**Qué hace:** en la fase de triaje, si no hay tareas en `docs/TASKS/`, el agente debe buscar una acción Kaizen, registrarla como `docs/TASKS/Kaizen_YYYY_MM_DD.md` y ejecutarla.

**Archivos:** 1 — `SddIA/process/automatic_task/spec.md` (+1 línea).

**Reproducción manual (sin depender de git):** en `SddIA/process/automatic_task/spec.md`, dentro de la sección “### 1. Identificación y Triaje (Triage)”, **después** del bullet que habla de `docs/TASKS/ACTIVE/`, añade este bullet (misma redacción):

```markdown
- **Acción Alternativa (Kaizen):** Si no hay tareas disponibles en `docs/TASKS/`, analiza el proyecto en busca de posibles acciones de mejora continua (Kaizen). Elige una de estas acciones y regístrala como una nueva tarea en un archivo `.md` dentro del directorio `docs/TASKS/` (por ejemplo, `docs/TASKS/Kaizen_YYYY_MM_DD.md`), procediendo a ejecutarla.
```

**Reproducción vía Git (otro clon con este repo como `upstream`):** tras añadir el remoto si hace falta, aplicar solo ese commit:

```text
git fetch <remote> e5bcaada74639242c823479cca88d97c14152bf4
git cherry-pick e5bcaada74639242c823479cca88d97c14152bf4
```

(La ejecución de comandos git en proyectos GesFer debe hacerse según norma interna, p. ej. skill `invoke-command`.)

---

## 2. `d38e4e3643fb1761bd268c058697989ce52e8e36` — CI, skill sddia-evolution y documentación asociada

**Qué hace:** incorpora la skill **sddia-evolution** (binarios Rust en `scripts/skills-rs`), ajustes de workflow `pr-validation`, normas y documentación de evolución SddIA, regla `.cursor` de sincronía, artefactos bajo `docs/features/sddia-evolution-sync-norma/`, entradas en `SddIA/evolution/`, etc.

**Resumen:** 34 archivos, +1850 / -4 líneas (incluye `Cargo.lock` grande).

**Lista de rutas tocadas** (para auditoría o copia selectiva):

- `.cursor/rules/sddia-evolution-sync.mdc`
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/README.md`
- `.github/workflows/pr-validation.yml`
- `SddIA/CONSTITUTION.md`
- `SddIA/agents/cumulo.paths.json`
- `SddIA/constitution.json`
- `SddIA/evolution/2a5a0e5a-4ecc-40c8-ac2c-8b7299ca637a.md`
- `SddIA/evolution/Evolution_log.md`
- `SddIA/evolution/d50066f8-59e9-48c8-83ef-c24f74efb78e.md`
- `SddIA/evolution/evolution_contract.md`
- `SddIA/norms/paths-via-cumulo.md`
- `SddIA/norms/sddia-evolution-sync.md`
- `SddIA/skills/sddia-evolution-register/spec.md`
- `docs/diagnostics/feat-sddia-evolution-sync-norma/execution_history.json`
- `docs/diagnostics/main/execution_history.json`
- `docs/features/sddia-evolution-sync-norma/bootstrap-register.json`
- `docs/features/sddia-evolution-sync-norma/clarify.md`
- `docs/features/sddia-evolution-sync-norma/execution.md`
- `docs/features/sddia-evolution-sync-norma/implementation.md`
- `docs/features/sddia-evolution-sync-norma/objectives.md`
- `docs/features/sddia-evolution-sync-norma/plan.md`
- `docs/features/sddia-evolution-sync-norma/replicacion-entorno-sddia.md`
- `docs/features/sddia-evolution-sync-norma/spec.md`
- `docs/features/sddia-evolution-sync-norma/validacion.md`
- `scripts/skills-rs/Cargo.lock`
- `scripts/skills-rs/Cargo.toml`
- `scripts/skills-rs/install.ps1`
- `scripts/skills-rs/src/bin/sddia_evolution_register.rs`
- `scripts/skills-rs/src/bin/sddia_evolution_validate.rs`
- `scripts/skills-rs/src/bin/sddia_evolution_watch.rs`
- `scripts/skills/index.json`
- `scripts/skills/sddia-evolution/Register.bat`
- `scripts/skills/sddia-evolution/manifest.json`

**Reproducción vía Git:** con el mismo remoto que tenga ese commit:

```text
git cherry-pick d38e4e3643fb1761bd268c058697989ce52e8e36
```

**Post-condición habitual en destino:** compilar skills Rust y desplegar binarios en la cápsula (según `scripts/skills-rs/install.ps1` y manifest de `scripts/skills/sddia-evolution/`). Revisa `SddIA/norms/sddia-evolution-sync.md` y la regla difundida en `.cursor/rules/sddia-evolution-sync.mdc` para alinear CI y evolución.

**Parche portable:** si no puedes usar `cherry-pick`, genera un parche desde el repositorio origen (`git format-patch -1 d38e4e3643fb1761bd268c058697989ce52e8e36`) e impórtalo en el destino (`git am`), respetando la política de comandos del proyecto.

---

## 3. `2cdaf21a2330de7f5cd05ec61854466eb77b8e94` — Merge a `main` (PR #12)

**Qué es:** merge de la rama `feature/kaizen-automatic-task-…` en `main` (mensaje: “Merge pull request #12…”).

**Diff frente al primer padre** (`git show -m`, vista típica del merge): toca 7 rutas (workflow, evolution log, detalle UUID en evolution, `automatic_task/spec.md`, y tres fuentes Rust `sddia_evolution_*` si el segundo padre aportaba esos archivos en la integración).

**Cómo replicarlo en otro sistema:**

- **Opción recomendada:** aplicar los commits **1** y **2** (o sus equivalentes manuales) y integrar en tu flujo habitual (`main` / PR). **No** hace falta recrear el objeto merge `2cdaf21a` salvo que necesites el mismo grafo de Git.
- **Opción literal:** fusionar la rama que contenga los mismos cambios que la PR #12 contra tu `main` (equivalente funcional al merge).

---

## Secuencia sugerida en el repositorio destino

1. Asegurar rama base alineada con la política del proyecto (sin commits directos a `master`/`main` si la norma lo prohíbe).
2. `cherry-pick` **e5bcaada** → revisar conflictos solo en `SddIA/process/automatic_task/spec.md`.
3. `cherry-pick` **d38e4e36** → revisar conflictos en normas, CI y `Cargo.*`; ejecutar instalación/compilación de skills según documentación del commit.
4. Validar (build, workflows, registro de evolución SddIA si aplica).
5. Abrir PR o merge según proceso del equipo.

---

## Referencia rápida de hashes completos

| Commit | SHA-1 completo |
|--------|----------------|
| Kaizen automatic-task | `e5bcaada74639242c823479cca88d97c14152bf4` |
| CI + sddia_evolution | `d38e4e3643fb1761bd268c058697989ce52e8e36` |
| Merge PR #12 | `2cdaf21a2330de7f5cd05ec61854466eb77b8e94` |
