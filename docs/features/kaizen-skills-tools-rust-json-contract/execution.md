---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
plan_ref: docs/features/kaizen-skills-tools-rust-json-contract/plan.md
implementation_ref: docs/features/kaizen-skills-tools-rust-json-contract/implementation.md
items_applied:
  - id: 1.1
    status: applied
  - id: 1.2
    status: applied
  - id: 1.3
    status: applied
  - id: 2.1
    status: applied
  - id: 2.2
    status: applied
  - id: 3.1
    status: applied
  - id: 3.2
    status: applied
  - id: 3.3
    status: applied
  - id: 3.4
    status: applied
  - id: 4.1
    status: applied
  - id: 4.2
    status: applied
  - id: 5.1
    status: applied
  - id: 5.2
    status: applied
  - id: 5.3
    status: applied
  - id: 6.1
    status: applied
  - id: 6.2
    status: applied
  - id: 7.1
    status: applied
  - id: 7.2
    status: applied
  - id: 7.3
    status: applied
  - id: 7.4
    status: applied
---

# Execution: Kaizen skills/tools Rust + JSON

## Ítems aplicados

### Fase 1: Crear proceso create-skill
- **1.1** Creado SddIA/process/create-skill/spec.md
- **1.2** Creado SddIA/process/create-skill/spec.json
- **1.3** Actualizado SddIA/process/README.md (create-skill en tabla, quitado TODO)

### Fase 2: Contrato JSON skills
- **2.1, 2.2** Añadidas secciones "JSON de entrada" y "JSON de salida" en SddIA/skills/skills-contract.md

### Fase 3: Skills Rust — iniciar_rama
- **3.1** Implementado scripts/skills-rs/src/bin/iniciar_rama.rs completo:
  - CLI: --branch-type, --branch-name, --main-branch, --skip-pull
  - JSON input: --input-path
  - JSON output: --output-json, --output-path
  - Flujo git: fetch, checkout, pull, checkout -b
  - Salida conforme a skills-contract (skillId, exitCode, success, feedback[])
- Añadido gesfer-skills lib (SkillResult, FeedbackEntry, to_skill_json)
- Añadido clap a skills-rs
- Actualizado Iniciar-Rama.bat: solo invoca .exe; sin fallback .ps1; conversión feat/fix + nombre a --branch-type --branch-name
- Ejecutado scripts/skills-rs/install.ps1: iniciar_rama.exe copiado a scripts/skills/iniciar-rama/bin/

### Fase 3: Skills Rust (continuación)
- **3.2** invoke_command.rs: CLI (--command, --command-file, --fase, --contexto), --input-path, --output-path, --output-json. Registro en docs/diagnostics/{branch}/execution_history.json.
- **3.3** invoke_commit.rs: Añadido --input-path para JSON con message, files, all, type, scope.
- **3.4** merge_to_master_cleanup.rs y push_and_create_pr.rs: Implementación completa con JSON I/O. Creado Push-And-CreatePR.bat.

### Fase 4: Tools Rust
- **4.1** run_tests_local.rs: dotnet build + dotnet test según TestScope (unit, integration, e2e, all). CLI + --input-path.
- **4.2** postman_mcp_validation.rs: Newman run con config desde postman-mcp-validation-config.json. CLI + --input-path.
- Añadidos [[bin]] en tools-rs/Cargo.toml; actualizado install.ps1 para copiar a run-tests-local y postman-mcp-validation.
- Eliminado gesfer_manager de Cargo.toml (archivo inexistente).

### Fase 5: Eliminar .ps1 y actualizar .bat
- **5.1–5.3** .bat actualizados para invocar solo .exe; sin fallback a .ps1. Mensaje de error si .exe no existe. manifest.json sin launcher_ps1.

### Fase 6: Validación pre-PR
- **6.1** Verificado .exe en cada cápsula (skills y tools).
- **6.2** Validada salida JSON: invoke_command y run_tests_local emiten skillId/toolId, exitCode, success, message, feedback[], timestamp. Ver validacion.md.

### Fase 7: Finalizar
- **7.1–7.2** Push y PR creado (skill finalizar-git). PR #6: feat(kaizen): skills/tools Rust + JSON según contratos SddIA.
- **7.3** Merge-To-Master-Cleanup ejecutado: `--branch-name feat/kaizen-skills-tools-rust-json-contract --delete-remote`
- **7.4** Evolution Log actualizado a "Completado".

## Paso 1 y 2 realizados (2026-03-19)

### 1. Rama creada
- **Skill:** iniciar-rama
- **Invocación:** `.\scripts\skills\iniciar-rama\bin\iniciar_rama.exe --branch-type feat --branch-name kaizen-skills-tools-rust-json-contract --main-branch main`
- **Nota:** iniciar_rama detecta automáticamente main vs master (rev-parse --verify). Iniciar-Rama.bat feat X funciona en repos con main o master.

### 2. Commit realizado
- **Skill:** invoke-commit
- **Invocación:** `.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "crear proceso create-skill; contrato JSON skills; iniciar_rama e invoke_commit Rust" --all --type feat --scope kaizen`
- **Commit:** f5af8e0 feat(kaizen): crear proceso create-skill; contrato JSON skills; iniciar_rama e invoke_commit Rust

---

## Documentación para proseguir

### Estado actual
- **Rama:** feat/kaizen-skills-tools-rust-json-contract
- **Último commit:** feat(kaizen): crear proceso create-skill; contrato JSON skills; iniciar_rama e invoke_commit Rust

### Próximos pasos (orden sugerido)

1. **Push y PR:** `.\scripts\skills\finalizar-git\Push-And-CreatePR.ps1` o Merge-To-Master-Cleanup.bat según documentación (pre-pr vs post-pr). Persist: `docs/features/kaizen-skills-tools-rust-json-contract/`

2. **Fase 3 (continuar):** invoke_command (3.2), finalizar-git merge_to_master_cleanup y push_and_create_pr (3.4) con JSON I/O

3. **Fase 4:** run_tests_local.exe, postman_mcp_validation.exe

4. **Fase 5:** Eliminar .ps1 de cápsulas; actualizar .bat y manifest

5. **Fase 6:** Validación pre-PR

6. **Fase 7:** Finalizar — merge, limpieza, Evolution Logs

### Invocaciones de referencia

| Acción | Skill | Comando |
|--------|-------|---------|
| Crear rama (repo con main) | iniciar-rama | `iniciar_rama.exe --branch-type feat --branch-name X --main-branch main` |
| Commit | invoke-commit | `Invoke-Commit.bat --message "msg" --all` o `--files "a,b"` |
| Push + PR | finalizar-git | Push-And-CreatePR (pre-pr) |
| Merge + limpieza | finalizar-git | Merge-To-Master-Cleanup (post-pr) |

### Archivos clave
- Plan: `docs/features/kaizen-skills-tools-rust-json-contract/plan.md`
- Implementación: `docs/features/kaizen-skills-tools-rust-json-contract/implementation.md`

---

## Pendiente

- 3.2–3.4: invoke_command, finalizar-git (invoke_commit ya implementado)
- 4.1–4.3: run_tests_local, postman_mcp_validation, --input-path en tools
- 5.1–5.3: Eliminar .ps1, actualizar .bat y manifest en resto de cápsulas
- 6.1–6.2: Validación
- 7.1–7.4: Finalizar (PR, merge, limpieza)

---

## Plan de continuación (contexto)

### Estado de artefactos (2026-03-19)

| Ítem | Artefacto | Estado |
|------|-----------|--------|
| 3.1 | iniciar_rama.rs | ✅ Completo (CLI + JSON I/O) |
| 3.2 | invoke_command.rs | ⬜ Stub (`fn main() {}`) |
| 3.3 | invoke_commit.rs | ⚠️ Parcial: CLI + output_json/output_path; falta `--input-path` |
| 3.4 | merge_to_master_cleanup.rs | ⬜ Stub |
| 3.4 | push_and_create_pr.rs | ⬜ Stub |
| 4.1 | run_tests_local.rs | ⬜ No existe en tools-rs |
| 4.2 | postman_mcp_validation.rs | ⬜ No existe en tools-rs |
| 4.3 | invoke_mysql_seeds, prepare_full_env, start_api | ⬜ Falta `--input-path` |
| 5.x | .ps1, .bat, manifest | ⬜ Pendiente (tras .exe funcionales) |
| 6.x | Validación | ⬜ Pendiente |
| 7.x | Finalizar | ⬜ Pendiente |

### Orden de ejecución sugerido

#### Bloque A: Skills Rust (Fase 3)

1. **3.2 invoke_command.rs**
   - Referencia: `Invoke-Command.ps1` en paths.skillCapsules.invoke-command
   - CLI: `--command`, `--working-dir`, `--input-path`, `--output-path`, `--output-json`
   - JSON input: `{ "skillId": "invoke-command", "command": "...", "workingDir": "..." }`
   - Salida: SkillResult (skillId, exitCode, success, feedback[])
   - Integrar con execution_history.json si aplica (telemetría por rama)

2. **3.3 invoke_commit.rs — completar `--input-path`**
   - Añadir `--input-path` para leer `{ "message", "files"|"all", "type", "scope" }`
   - Si `--input-path` presente, ignorar args CLI para esos campos
   - Mantener compatibilidad con CLI actual

3. **3.4 merge_to_master_cleanup.rs**
   - Referencia: `Merge-To-Master-Cleanup.ps1` / `.bat`
   - CLI: `--branch-name`, `--delete-remote`, `--main-branch`, `--input-path`, `--output-path`
   - Flujo: checkout main/master, pull, merge, delete remote branch
   - Salida JSON conforme a skills-contract

4. **3.4 push_and_create_pr.rs**
   - Referencia: `Push-And-CreatePR.ps1` / `Unificar-Rama.ps1`
   - CLI: `--branch-name`, `--base-branch`, `--input-path`, `--output-path`
   - Flujo: push, gh pr create (o equivalente)
   - Salida JSON conforme a skills-contract

5. **Commit Fase 3:** `feat(skills): invoke_command, invoke_commit --input-path, finalizar-git merge y push con JSON I/O`

#### Bloque B: Tools Rust (Fase 4)

6. **4.1 run_tests_local.rs**
   - Replicar lógica de `Run-Tests-Local.ps1` (TestScope, SkipPrepare, SkipSeeds, OnlyTests, E2EBaseUrl)
   - CLI + `--input-path`, `--output-path`, `--output-json`
   - Ejecutar `dotnet test` según scope; salida tools-contract
   - Añadir `[[bin]]` en tools-rs/Cargo.toml; actualizar install.ps1

7. **4.2 postman_mcp_validation.rs**
   - Replicar lógica de `Postman-Mcp-Validation.ps1` (CollectionPath, BaseUrl, InternalSecret, EnvironmentPath)
   - Invocar Newman (npx newman run …) o reqwest si API directa
   - CLI + `--input-path`, `--output-path`, `--output-json`
   - Añadir `[[bin]]` en tools-rs/Cargo.toml; actualizar install.ps1

8. **4.3 (opcional)** — Añadir `--input-path` a invoke_mysql_seeds, prepare_full_env, start_api

9. **Commit Fase 4:** `feat(tools): run_tests_local, postman_mcp_validation, --input-path en tools`

#### Bloque C: Eliminar .ps1 (Fase 5)

10. **5.1–5.3** — Eliminar .ps1 de cápsulas donde .exe exista; actualizar .bat (sin fallback .ps1); actualizar manifest.json

11. **Commit Fase 5:** `refactor(skills,tools): eliminar .ps1, actualizar .bat y manifest`

#### Bloque D: Validación y cierre (Fases 6–7)

12. **6.1–6.2** — Verificar .exe en cada cápsula; validar salida JSON

13. **7.1–7.4** — Push, PR, merge, limpieza, Evolution Logs (skill finalizar-git)

### Touchpoints por archivo

| Ruta | Acciones |
|------|----------|
| `scripts/skills-rs/src/bin/invoke_command.rs` | Implementar completo (CLI + JSON I/O) |
| `scripts/skills-rs/src/bin/invoke_commit.rs` | Añadir `--input-path` |
| `scripts/skills-rs/src/bin/merge_to_master_cleanup.rs` | Implementar completo |
| `scripts/skills-rs/src/bin/push_and_create_pr.rs` | Implementar completo |
| `scripts/skills-rs/src/lib.rs` | Reutilizar SkillResult, FeedbackEntry, to_skill_json |
| `scripts/tools-rs/src/bin/run_tests_local.rs` | Crear (nuevo) |
| `scripts/tools-rs/src/bin/postman_mcp_validation.rs` | Crear (nuevo) |
| `scripts/tools-rs/Cargo.toml` | Añadir [[bin]] run_tests_local, postman_mcp_validation |
| `scripts/tools-rs/install.ps1` | Copiar run_tests_local.exe, postman_mcp_validation.exe a cápsulas |
| `scripts/skills/invoke-command/Invoke-Command.bat` | Invocar solo .exe (tras 3.2) |
| `scripts/skills/finalizar-git/*.bat` | Invocar solo .exe (tras 3.4) |
| `scripts/tools/run-tests-local/`, `postman-mcp-validation/` | .bat solo .exe; eliminar .ps1 |

### Criterios de done por ítem

- **3.2:** invoke_command.exe en bin/; acepta --input-path; emite JSON
- **3.3:** invoke_commit acepta --input-path; prioridad input-path sobre CLI
- **3.4:** merge_to_master_cleanup.exe y push_and_create_pr.exe en finalizar-git/bin/
- **4.1:** run_tests_local.exe en run-tests-local/bin/; dotnet test ejecutado
- **4.2:** postman_mcp_validation.exe en postman-mcp-validation/bin/; Newman ejecutado
