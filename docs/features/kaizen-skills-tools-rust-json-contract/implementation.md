---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
plan_ref: docs/features/kaizen-skills-tools-rust-json-contract/plan.md
spec_ref: docs/features/kaizen-skills-tools-rust-json-contract/spec.md
items:
  - id: pre.1
    phase: Preparar entorno
  - id: 1.1
    phase: Crear proceso create-skill
  - id: 1.2
    phase: Crear proceso create-skill
  - id: 1.3
    phase: Crear proceso create-skill
  - id: 2.1
    phase: Contrato JSON skills
  - id: 2.2
    phase: Contrato JSON skills
  - id: 3.1
    phase: Skills Rust
  - id: 3.2
    phase: Skills Rust
  - id: 3.3
    phase: Skills Rust
  - id: 3.4
    phase: Skills Rust
  - id: 4.1
    phase: Tools Rust
  - id: 4.2
    phase: Tools Rust
  - id: 4.3
    phase: Tools Rust
  - id: 5.1
    phase: Eliminar .ps1
  - id: 5.2
    phase: Actualizar .bat
  - id: 5.3
    phase: Actualizar manifest
  - id: 6.1
    phase: Validación
  - id: 6.2
    phase: Validación
  - id: 7.1
    phase: Finalizar
  - id: 7.2
    phase: Finalizar
  - id: 7.3
    phase: Finalizar
  - id: 7.4
    phase: Finalizar
---

# Implementation: Kaizen skills/tools Rust + JSON

## Ítems de implementación

### Preparar entorno: Generar rama

#### pre.1 – Crear rama feat/kaizen-skills-tools-rust-json-contract
- **Id:** pre.1
- **Acción:** Ejecutar
- **Skill:** iniciar-rama (paths.skillCapsules.iniciar-rama)
- **Propuesta:** Invocar Iniciar-Rama.bat feat kaizen-skills-tools-rust-json-contract. Rama creada desde master actualizado; working tree en feat/kaizen-skills-tools-rust-json-contract.
- **Dependencias:** Ninguna

---

### Fase 1: Crear proceso create-skill (Paso 1)

#### 1.1 – Crear spec.md del proceso create-skill
- **Id:** 1.1
- **Acción:** Crear
- **Ruta:** `SddIA/process/create-skill/spec.md`
- **Propuesta:** Adecuar SddIA/process/create-tool/spec.md: sustituir tool_id por skill_id, paths.tools* por paths.skills*, tools-contract por skills-contract. Incluir frontmatter YAML (process_id, contract_ref, persist_ref, phases) y cuerpo con propósito, alcance, fases, restricciones, estructura de cápsula, prohibiciones (.ps1).
- **Dependencias:** Ninguna

#### 1.2 – Crear spec.json del proceso create-skill
- **Id:** 1.2
- **Acción:** Crear
- **Ruta:** `SddIA/process/create-skill/spec.json`
- **Propuesta:** Metadatos machine-readable: process_id "create-skill", name, description, phases, persist_ref (paths.featurePath/create-skill-\<skill-id\>), contract_ref, related_actions, related_skills.
- **Dependencias:** 1.1

#### 1.3 – Actualizar process README e índices
- **Id:** 1.3
- **Acción:** Modificar
- **Ruta:** `SddIA/process/README.md`
- **Propuesta:** Añadir create-skill a la tabla "Procesos disponibles". Quitar sección "TODO: create-skill (pendiente)". Añadir entrada en "Uso" para create-skill.
- **Dependencias:** 1.1, 1.2

---

### Fase 2: Contrato JSON para skills

#### 2.1 – Añadir sección JSON entrada en skills-contract
- **Id:** 2.1
- **Acción:** Modificar
- **Ruta:** `SddIA/skills/skills-contract.md`
- **Propuesta:** Añadir sección "JSON de entrada" con esquema: skillId, parámetros por skill (BranchType, BranchName, etc.) vía stdin o `--input-path`. Ejemplo para iniciar-rama: `{"skillId":"iniciar-rama","BranchType":"feat","BranchName":"mi-feature"}`.
- **Dependencias:** Ninguna

#### 2.2 – Añadir sección JSON salida en skills-contract
- **Id:** 2.2
- **Acción:** Modificar
- **Ruta:** `SddIA/skills/skills-contract.md`
- **Propuesta:** Añadir sección "JSON de salida" alineada a tools-contract: skillId, exitCode, success, timestamp, message, feedback[], data (opcional). Documentar soporte dual CLI + JSON input.
- **Dependencias:** Ninguna

---

### Fase 3: Skills Rust + JSON I/O

#### 3.1 – Implementar iniciar_rama.rs completo
- **Id:** 3.1
- **Acción:** Crear/Modificar
- **Ruta:** `scripts/skills-rs/src/bin/iniciar_rama.rs`
- **Propuesta:** Implementar lógica completa: fetch, checkout troncal, pull, checkout -b feat/fix. Argumentos CLI (--branch-type, --branch-name) y JSON input (--input-path o stdin). Salida JSON por stdout o --output-path. Estructura feedback[] por fases.
- **Dependencias:** 2.1, 2.2

#### 3.2 – Completar invoke_command.rs con JSON I/O
- **Id:** 3.2
- **Acción:** Modificar
- **Ruta:** `scripts/skills-rs/src/bin/invoke_command.rs`
- **Propuesta:** Añadir --input-path, --output-path. Salida JSON con skillId, exitCode, success, message, feedback. Integrar con execution_history.json si aplica.
- **Dependencias:** 2.1, 2.2

#### 3.3 – Completar invoke_commit.rs con soporte dual
- **Id:** 3.3
- **Acción:** Modificar
- **Ruta:** `scripts/skills-rs/src/bin/invoke_commit.rs`
- **Propuesta:** Soporte --input-path para JSON con message, files, all. Salida JSON conforme a skills-contract.
- **Dependencias:** 2.1, 2.2

#### 3.4 – Completar finalizar-git (merge_to_master_cleanup, push_and_create_pr)
- **Id:** 3.4
- **Acción:** Modificar
- **Rutas:** `scripts/skills-rs/src/bin/merge_to_master_cleanup.rs`, `scripts/skills-rs/src/bin/push_and_create_pr.rs`
- **Propuesta:** Ambos binarios con JSON I/O completo. CLI + --input-path. Salida con feedback[] por fase.
- **Dependencias:** 2.1, 2.2

---

### Fase 4: Tools Rust + JSON I/O

#### 4.1 – Crear run_tests_local.exe
- **Id:** 4.1
- **Acción:** Crear
- **Rutas:** `scripts/tools-rs/src/bin/run_tests_local.rs`, `scripts/tools-rs/Cargo.toml` (añadir [[bin]]), `scripts/tools/run-tests-local/bin/`
- **Propuesta:** Implementación Rust que ejecuta dotnet test. Salida JSON conforme a tools-contract. Replicar lógica de Run-Tests-Local.ps1. Añadir entrada en Cargo.toml. Actualizar scripts/tools-rs/install.ps1 para copiar a cápsula.
- **Dependencias:** Ninguna (tools-contract ya define salida)

#### 4.2 – Crear postman_mcp_validation.exe
- **Id:** 4.2
- **Acción:** Crear
- **Rutas:** `scripts/tools-rs/src/bin/postman_mcp_validation.rs`, `scripts/tools-rs/Cargo.toml` (añadir [[bin]]), `scripts/tools/postman-mcp-validation/bin/`
- **Propuesta:** Implementación Rust que invoca Newman (colección Postman). Salida JSON tools-contract. Replicar Postman-Mcp-Validation.ps1. Dependencias: newman (npm) o reqwest para API. Actualizar install.ps1.
- **Dependencias:** Ninguna

#### 4.3 – Añadir --input-path a tools existentes
- **Id:** 4.3
- **Acción:** Modificar
- **Rutas:** `scripts/tools-rs/src/bin/invoke_mysql_seeds.rs`, `scripts/tools-rs/src/bin/prepare_full_env.rs`, `scripts/tools-rs/src/bin/start_api.rs`
- **Propuesta:** Añadir soporte --input-path para recibir parámetros en JSON (además de CLI). Mantener compatibilidad con argumentos actuales.
- **Dependencias:** Opcional; puede hacerse en paralelo con 4.1, 4.2

---

### Fase 5: Eliminar .ps1 y actualizar .bat

#### 5.1 – Eliminar .ps1 de cápsulas
- **Id:** 5.1
- **Acción:** Eliminar
- **Rutas:** `scripts/skills/iniciar-rama/Iniciar-Rama.ps1`, `scripts/skills/invoke-command/Invoke-Command.ps1`, `scripts/skills/invoke-commit/` (si existe .ps1), `scripts/skills/finalizar-git/Merge-To-Master-Cleanup.ps1`, `scripts/skills/finalizar-git/Push-And-CreatePR.ps1` (y Unificar-Rama.ps1 si aplica); `scripts/tools/invoke-mysql-seeds/Invoke-MySqlSeeds.ps1`, `scripts/tools/prepare-full-env/Prepare-FullEnv.ps1`, `scripts/tools/run-tests-local/Run-Tests-Local.ps1`, `scripts/tools/postman-mcp-validation/Postman-Mcp-Validation.ps1`, `scripts/tools/start-api/Start-Api.ps1`
- **Propuesta:** Eliminar cada .ps1 solo cuando el .exe correspondiente esté funcional y validado.
- **Dependencias:** 3.1–3.4, 4.1–4.2 completados

#### 5.2 – Actualizar .bat para invocar solo .exe
- **Id:** 5.2
- **Acción:** Modificar
- **Rutas:** Todos los .bat en paths.skillCapsules y paths.toolCapsules
- **Propuesta:** Quitar lógica de fallback a .ps1. Si no existe .exe, mostrar: "ERROR: No se encontró <nombre>.exe. Ejecute scripts/skills-rs/install.ps1" o "scripts/tools-rs/install.ps1" y exit /b 1.
- **Dependencias:** 5.1 (o en paralelo si .exe ya existe)

#### 5.3 – Actualizar manifest.json de cápsulas
- **Id:** 5.3
- **Acción:** Modificar
- **Rutas:** manifest.json en cada cápsula (iniciar-rama, invoke-command, invoke-commit, finalizar-git; invoke-mysql-seeds, prepare-full-env, run-tests-local, postman-mcp-validation, start-api)
- **Propuesta:** Quitar campos launcher_ps1, launcher_ps1_pre_pr, launcher_ps1_post_pr, push_and_create_pr (si referencian .ps1). components: solo launcher_bat, bin, doc, config (si aplica).
- **Dependencias:** 5.1

---

### Fase 6: Validación

#### 6.1 – Verificar .exe en cada cápsula
- **Id:** 6.1
- **Acción:** Verificar
- **Propuesta:** Comprobar que `bin/<nombre>.exe` existe en cada cápsula. Ejecutar scripts/skills-rs/install.ps1 y scripts/tools-rs/install.ps1.
- **Dependencias:** 3.1–3.4, 4.1–4.2, 5.1

#### 6.2 – Verificar salida JSON
- **Id:** 6.2
- **Acción:** Verificar
- **Propuesta:** Invocar cada skill/tool con --output-json o -OutputJson y validar estructura (success, exitCode, message, feedback, timestamp).
- **Dependencias:** 6.1

---

### Fase 7: Finalizar (PR, merge, limpieza)

#### 7.1 – Pre-PR: certificar rama y push
- **Id:** 7.1
- **Acción:** Ejecutar
- **Skill:** finalizar-git (pre-pr: Unificar-Rama, Push-And-CreatePR)
- **Propuesta:** Verificar build, documentación y commit final. Push a origin. Persist: docs/features/kaizen-skills-tools-rust-json-contract/
- **Dependencias:** 6.1, 6.2

#### 7.2 – Crear Pull Request
- **Id:** 7.2
- **Acción:** Ejecutar
- **Propuesta:** gh pr create (si gh instalado) o abrir URL manual para crear PR hacia master/main.
- **Dependencias:** 7.1

#### 7.3 – Post-PR: merge y limpieza
- **Id:** 7.3
- **Acción:** Ejecutar
- **Skill:** finalizar-git (post-pr: Merge-To-Master-Cleanup)
- **Propuesta:** Tras merge del PR: Merge-To-Master-Cleanup.bat -BranchName feat/kaizen-skills-tools-rust-json-contract -DeleteRemote. Checkout master, pull, eliminar rama remota.
- **Dependencias:** 7.2 (merge aprobado)

#### 7.4 – Actualizar Evolution Logs
- **Id:** 7.4
- **Acción:** Modificar
- **Ruta:** paths.evolutionPath + paths.evolutionLogFile (docs/evolution/EVOLUTION_LOG.md)
- **Propuesta:** Añadir línea: [YYYY-MM-DD] [feat/kaizen-skills-tools-rust-json-contract] Kaizen skills/tools Rust + JSON según contratos SddIA. [Estado].
- **Dependencias:** 7.3

---

## Estrategia de commits

| Fase | Tipo | Ejemplo |
|------|------|---------|
| pre | chore | docs(kaizen): objectives, spec, clarify, plan |
| 1 | feat | feat(process): crear proceso create-skill |
| 2 | docs | docs(skills): contrato JSON entrada/salida |
| 3 | feat | feat(skills): iniciar_rama, invoke_command, invoke_commit, finalizar-git JSON I/O |
| 4 | feat | feat(tools): run_tests_local, postman_mcp_validation, --input-path |
| 5 | refactor | refactor(skills,tools): eliminar .ps1, actualizar .bat |
| 6 | chore | chore(kaizen): validación pre-PR |

**Skill:** invoke-commit para commits con trazabilidad.

---

## Resumen por archivo/ruta

| Ruta | Ítems |
|------|-------|
| (rama feat/kaizen-skills-tools-rust-json-contract) | pre.1 |
| SddIA/process/create-skill/spec.md | 1.1 |
| SddIA/process/create-skill/spec.json | 1.2 |
| SddIA/process/README.md | 1.3 |
| SddIA/skills/skills-contract.md | 2.1, 2.2 |
| scripts/skills-rs/src/bin/iniciar_rama.rs | 3.1 |
| scripts/skills-rs/src/bin/invoke_command.rs | 3.2 |
| scripts/skills-rs/src/bin/invoke_commit.rs | 3.3 |
| scripts/skills-rs/src/bin/merge_to_master_cleanup.rs | 3.4 |
| scripts/skills-rs/src/bin/push_and_create_pr.rs | 3.4 |
| scripts/tools-rs/src/bin/run_tests_local.rs | 4.1 |
| scripts/tools-rs/src/bin/postman_mcp_validation.rs | 4.2 |
| scripts/tools-rs/src/bin/invoke_mysql_seeds.rs | 4.3 |
| scripts/tools-rs/src/bin/prepare_full_env.rs | 4.3 |
| scripts/tools-rs/src/bin/start_api.rs | 4.3 |
| scripts/skills/*/Iniciar-Rama.bat, etc. | 5.2 |
| scripts/skills/*/manifest.json | 5.3 |
| scripts/tools/*/manifest.json | 5.3 |
| (skill finalizar-git: push, PR, merge, cleanup) | 7.1, 7.2, 7.3 |
| docs/evolution/EVOLUTION_LOG.md | 7.4 |

---

## Orden sugerido

1. **Preparar entorno:** pre.1 (crear rama con skill iniciar-rama).
2. **Fase 1:** 1.1, 1.2, 1.3 (crear proceso create-skill).
3. **Fase 2:** 2.1, 2.2 (en paralelo).
4. **Fase 3:** 3.1 primero; 3.2–3.4 en paralelo tras 3.1.
5. **Fase 4:** 4.1, 4.2 en paralelo; 4.3 opcional.
6. **Fase 5:** 5.1 por cápsula (tras .exe funcional); 5.2, 5.3 en paralelo.
7. **Fase 6:** 6.1, 6.2 (validación pre-PR).
8. **Fase 7:** 7.1–7.4 (PR, merge, limpieza, Evolution Logs).
