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

## Paso 1 y 2 realizados (2026-03-19)

### 1. Rama creada
- **Skill:** iniciar-rama
- **Invocación:** `.\scripts\skills\iniciar-rama\bin\iniciar_rama.exe --branch-type feat --branch-name kaizen-skills-tools-rust-json-contract --main-branch main`
- **Nota:** Este repo usa `main` como troncal; si Iniciar-Rama.bat feat X falla con "pathspec 'master'", usar `--main-branch main` o invocar el .exe directamente.

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
