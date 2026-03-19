---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
phases:
  - id: 'pre'
    name: Preparar entorno (generar rama)
  - id: '0'
    name: Condición de control (ley IA)
  - id: '1'
    name: Crear proceso create-skill
  - id: '2'
    name: Contrato JSON skills
  - id: '3'
    name: Skills Rust + JSON I/O
  - id: '4'
    name: Tools Rust + JSON I/O
  - id: '5'
    name: Eliminar .ps1 y actualizar .bat
  - id: '6'
    name: Validación
  - id: '7'
    name: Finalizar (PR, merge, limpieza)
spec_ref: docs/features/kaizen-skills-tools-rust-json-contract/spec.md
clarify_ref: docs/features/kaizen-skills-tools-rust-json-contract/clarify.md
---

# Plan: Kaizen skills/tools Rust + JSON

## Preparar entorno: Generar rama

**Skill:** iniciar-rama (paths.skillCapsules.iniciar-rama).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| pre.1 | Crear rama feat desde master actualizado | Rama `feat/kaizen-skills-tools-rust-json-contract` |
| pre.2 | Posicionar repositorio en la rama | Working tree listo para commits |

**Invocación:** `Iniciar-Rama.bat feat kaizen-skills-tools-rust-json-contract` (o equivalente con skill iniciar-rama: BranchType feat, BranchName kaizen-skills-tools-rust-json-contract).

**Criterio:** No trabajar en master; toda la implementación en la rama feat.

---

## Condición de control (ley IA)

**Regla obligatoria para toda IA que opere en el repositorio:**

1. **Solo ejecutar .exe:** La IA únicamente ha de invocar binarios `.exe` (skills y tools). No ejecutar `.ps1`, `.bat` ni `.sh` como implementación.
2. **Si no existe .exe:** Cuando la IA necesite una skill o tool y no encuentre el `.exe` en `<cápsula>/bin/`, debe **añadir todo lo necesario para generarlo**:
   - **Skills:** Fuente Rust en paths.skillsRustPath, entrada en Cargo.toml, definición en paths.skillsDefinitionPath, cápsula en paths.skillCapsules, manifest.json, launcher .bat que invoque solo .exe.
   - **Tools:** Proceso create-tool (paths.processPath/create-tool/) o equivalente: definición en paths.toolsDefinitionPath, fuente Rust en paths.toolsRustPath, cápsula en paths.toolCapsules, manifest.json, launcher .bat que invoque solo .exe.
3. **Compilación:** Tras añadir el código Rust, ejecutar install.ps1 (skills-rs o tools-rs) para compilar y copiar el .exe a la cápsula.

**Referencia:** SddIA/norms/commands-via-skills-or-tools.md; procesos create-tool, feature.

---

## Fase 1: Crear proceso create-skill (Paso 1)

**Descripción:** Definir el proceso **create-skill** para la creación de nuevas skills, adecuando como ejemplo el proceso existente **create-tool** (paths.processPath/create-tool/).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 1.1 | Crear spec.md del proceso create-skill | SddIA/process/create-skill/spec.md con frontmatter YAML y cuerpo (propósito, alcance, fases, restricciones, estructura de cápsula) |
| 1.2 | Crear spec.json del proceso create-skill | SddIA/process/create-skill/spec.json (process_id, name, phases, persist_ref, contract_ref) |
| 1.3 | Actualizar process README e índices | SddIA/process/README.md: añadir create-skill a la tabla de procesos; quitar TODO |

**Estructura (análoga a create-tool):**

| create-tool | create-skill |
|-------------|--------------|
| tool_id | skill_id |
| paths.toolsDefinitionPath | paths.skillsDefinitionPath |
| paths.toolCapsules | paths.skillCapsules |
| paths.toolsRustPath | paths.skillsRustPath |
| paths.toolsIndexPath | paths.skillsIndexPath |
| Rama feat/create-tool-\<id\> | Rama feat/create-skill-\<id\> |
| tools-contract.md | skills-contract.md |

**Criterio:** Proceso create-skill operativo; la IA puede seguir el proceso para crear nuevas skills.

---

## Fase 2: Contrato JSON para skills

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 2.1 | Definir esquema JSON entrada en skills-contract | Campos: skillId, parámetros por skill (BranchType, BranchName, etc.) vía stdin o --input-path |
| 2.2 | Definir esquema JSON salida en skills-contract | Campos: skillId, exitCode, success, timestamp, message, feedback[], data (opcional) — alineado a tools-contract |
| 2.3 | Documentar soporte dual CLI + JSON input | Actualizar SddIA/skills/skills-contract.md |
| 2.4 | Actualizar skills-contract.json si existe | Esquema machine-readable |

**Criterio:** skills-contract.md tiene sección explícita de JSON I/O; coherente con tools-contract.

---

## Fase 3: Skills Rust + JSON I/O

| Tarea | skill_id | Acción | Entregables |
|-------|----------|--------|-------------|
| 3.1 | iniciar-rama | Implementar binario completo | src/bin/iniciar_rama.rs funcional, CLI + JSON input, salida JSON |
| 3.2 | invoke-command | Completar JSON I/O | invoke_command.rs con --input-path, --output-path, salida JSON |
| 3.3 | invoke-commit | Completar JSON I/O | invoke_commit.rs con soporte dual |
| 3.4 | finalizar-git | Completar merge_to_master_cleanup, push_and_create_pr | Ambos binarios con JSON I/O |

**Orden:** 3.1 (iniciar-rama) primero por ser base del flujo; 3.2–3.4 en paralelo si se desea.

---

## Fase 4: Tools Rust + JSON I/O

| Tarea | tool_id | Acción | Entregables |
|-------|---------|--------|-------------|
| 4.1 | run-tests-local | Crear implementación Rust | run_tests_local.exe, src/bin/run_tests_local.rs, Cargo.toml, cápsula bin/ |
| 4.2 | postman-mcp-validation | Crear implementación Rust | postman_mcp_validation.exe, src/bin/postman_mcp_validation.rs, Cargo.toml, cápsula bin/ |
| 4.3 | invoke-mysql-seeds, prepare-full-env, start-api | Añadir soporte --input-path si falta | JSON input dual donde aplique |

**Orden:** 4.1 y 4.2 en paralelo; 4.3 según prioridad.

---

## Fase 5: Eliminar .ps1 y actualizar .bat

| Tarea | Descripción | Criterio |
|-------|-------------|----------|
| 5.1 | Eliminar .ps1 de cada cápsula | Solo cuando el .exe correspondiente esté funcional |
| 5.2 | Actualizar .bat para invocar solo .exe | Sin fallback a .ps1; si no hay .exe, error explícito con mensaje "Ejecute scripts/skills-rs/install.ps1" o "scripts/tools-rs/install.ps1" |
| 5.3 | Actualizar manifest.json | Quitar referencias a launcher_ps1; components sin .ps1 |
| 5.4 | Actualizar documentación en cápsulas | .md sin menciones a .ps1 como flujo estándar |

**Cápsulas afectadas:** iniciar-rama, finalizar-git, invoke-command, invoke-commit; invoke-mysql-seeds, prepare-full-env, run-tests-local, postman-mcp-validation, start-api.

---

## Fase 6: Validación

| Tarea | Descripción | Criterio |
|-------|-------------|----------|
| 6.1 | Verificar .exe en cada cápsula | `bin/<nombre>.exe` existe |
| 6.2 | Verificar salida JSON | Campos requeridos (success, exitCode, message, feedback, timestamp) |
| 6.3 | Verificar install.ps1 | skills-rs e tools-rs copian correctamente a cápsulas |
| 6.4 | Proceso audit-tool | Aplicable a cada tool; skills con checklist equivalente |

---

## Estrategia de commits

**Convención:** Commits convencionales (feat, fix, docs, chore). Un commit por fase o por lote lógico.

| Fase | Tipo commit | Ejemplo mensaje |
|------|-------------|-----------------|
| pre | chore | docs(kaizen): añadir objectives, spec, clarify, plan |
| 1 | feat | feat(process): crear proceso create-skill |
| 2 | docs | docs(skills): añadir contrato JSON entrada/salida |
| 3 | feat | feat(skills): implementar iniciar_rama, invoke_command, invoke_commit, finalizar-git con JSON I/O |
| 4 | feat | feat(tools): crear run_tests_local, postman_mcp_validation; añadir --input-path |
| 5 | refactor | refactor(skills,tools): eliminar .ps1, actualizar .bat y manifest |
| 6 | chore | chore(kaizen): validación pre-PR |

**Skill:** invoke-commit (paths.skillCapsules.invoke-commit) para commits con trazabilidad.

---

## Fase 7: Finalizar (PR, merge, limpieza)

**Skill:** finalizar-git (paths.skillCapsules.finalizar-git).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 7.1 | Pre-PR: certificar rama y push | Unificar-Rama.ps1 / verify-pr-protocol; push a origin |
| 7.2 | Crear Pull Request | gh pr create o URL manual; PR hacia master/main |
| 7.3 | Post-PR: merge y limpieza | Merge-To-Master-Cleanup.bat: merge a master, sync, eliminar rama remota |
| 7.4 | Actualizar Evolution Logs | paths.evolutionPath + paths.evolutionLogFile: entrada con fecha, rama, descripción |

**Flujo:** Pre-PR (push + crear PR) → merge (manual o automático) → Post-PR (Merge-To-Master-Cleanup con -BranchName feat/kaizen-skills-tools-rust-json-contract -DeleteRemote).

**Criterio:** Rama fusionada en master; rama remota eliminada; Evolution Logs actualizados.

---

## Orden de ejecución

1. **Preparar entorno:** Generar rama feat/kaizen-skills-tools-rust-json-contract (skill iniciar-rama).
2. **Fase 0:** Condición de control — difundir en SddIA/norms o .cursor/rules.
3. **Fase 1:** Crear proceso create-skill (Paso 1).
4. **Fase 2:** Contrato JSON skills (prerrequisito para fases 3–5).
5. **Fases 3 y 4:** En paralelo (skills y tools).
6. **Fase 5:** Tras completar cada skill/tool en 3 y 4.
7. **Fase 6:** Validación pre-PR.
8. **Fase 7:** Finalizar — PR, merge, limpieza (skill finalizar-git).

---

## Difusión de la condición de control

La regla "IA solo ejecuta .exe; si no existe, generar skill/tool" debe incorporarse a:

- **SddIA/norms/commands-via-skills-or-tools.md** — añadir subsección "Condición .exe obligatorio".
- **.cursor/rules/** — regla de difusión para Cursor (opcional, vía sddia-difusion).

---

## Referencia: Proceso create-skill (Fase 1)

El proceso create-skill se define en la **Fase 1 (Paso 1)**. Ver estructura en la tabla de equivalencias create-tool → create-skill de esa fase. Ubicación: paths.processPath/create-skill/ (spec.md, spec.json).
