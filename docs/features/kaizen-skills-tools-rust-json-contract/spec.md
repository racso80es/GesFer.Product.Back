---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
base: origin/main
scope: scripts/skills-rs/, scripts/tools-rs/, scripts/skills/, scripts/tools/, SddIA/skills/, SddIA/tools/
contract_ref: SddIA/tools/tools-contract.md, SddIA/skills/skills-contract.md
---

# SPEC: Kaizen scripts, skills y tools — Rust + JSON según contratos SddIA

## Contexto

Los contratos SddIA (constitution.json, tools-contract.md, skills-contract.md) establecen que:

- **Implementación por defecto:** Rust (binarios `.exe`).
- **Prohibido:** `.ps1`, `.bat`, `.sh` como implementación principal.
- **Tools:** Salida JSON obligatoria (toolId, exitCode, success, timestamp, message, feedback[]).
- **Skills:** Implementación Rust obligatoria; el contrato de skills no define explícitamente JSON entrada/salida.

Actualmente existen fallbacks `.ps1` en cápsulas y binarios Rust incompletos o vacíos.

---

## Estado actual (inventario)

### Skills (paths.skillCapsules)

| skill_id      | Binario Rust              | Estado binario      | Fallback .ps1 | JSON I/O |
|---------------|---------------------------|---------------------|---------------|----------|
| iniciar-rama  | iniciar_rama.exe          | Vacío (fn main(){}) | Iniciar-Rama.ps1 | No definido |
| finalizar-git | merge_to_master_cleanup.exe, push_and_create_pr.exe | Parcial | Merge-To-Master-Cleanup.ps1, Push-And-CreatePR.ps1 | No definido |
| invoke-command| invoke_command.exe        | Parcial             | Invoke-Command.ps1 | Parcial (execution_history.json) |
| invoke-commit | invoke_commit.exe         | Parcial             | —             | Parcial |

### Tools (paths.toolCapsules)

| tool_id               | Binario Rust           | Estado binario | Fallback .ps1 | JSON I/O |
|-----------------------|------------------------|----------------|---------------|----------|
| invoke-mysql-seeds     | invoke_mysql_seeds.exe | Implementado   | Invoke-MySqlSeeds.ps1 | Sí (tools-contract) |
| prepare-full-env      | prepare_full_env.exe   | Implementado   | Prepare-FullEnv.ps1 | Sí |
| run-tests-local       | —                      | No existe      | Run-Tests-Local.ps1 | Parcial |
| postman-mcp-validation| —                      | No existe      | Postman-Mcp-Validation.ps1 | Parcial |
| start-api             | start_api.exe          | Implementado   | Start-Api.ps1 | Sí |

---

## Especificación técnica

### Punto 1: Contrato JSON para skills

**Descripción:** Definir un contrato JSON de entrada/salida para skills ejecutables, análogo al de tools, para interoperabilidad con IA y pipelines.

**Propuesta de esquema (skills):**

- **Entrada:** JSON por stdin o `--input-path` con parámetros del skill (ej. BranchType, BranchName para iniciar-rama).
- **Salida:** JSON por stdout o `--output-path` con: skillId, exitCode, success, timestamp, message, feedback[], data (opcional).

**Criterio de aceptación:** Documentar en SddIA/skills/skills-contract.md (o skills-contract.json) el esquema de entrada/salida JSON; alinear con tools-contract donde sea posible.

---

### Punto 2: Completar binarios Rust de skills

**Descripción:** Implementar o completar los binarios Rust en paths.skillsRustPath para cada skill con ejecutable.

| skill_id      | Binario                 | Acción |
|---------------|-------------------------|--------|
| iniciar-rama  | iniciar_rama.exe        | Implementar (actualmente vacío) |
| finalizar-git | merge_to_master_cleanup, push_and_create_pr | Completar si faltan funcionalidades |
| invoke-command| invoke_command.exe      | Completar JSON I/O |
| invoke-commit | invoke_commit.exe       | Completar JSON I/O |

**Criterio de aceptación:** Cada skill invocable produce `.exe` funcional que cumple contrato JSON; el `.bat` invoca solo el `.exe` (sin fallback .ps1 en flujo estándar).

---

### Punto 3: Completar binarios Rust de tools

**Descripción:** Crear o completar binarios Rust para tools que aún no tienen implementación.

| tool_id               | Binario                 | Acción |
|-----------------------|-------------------------|--------|
| run-tests-local       | run_tests_local.exe     | Crear implementación Rust |
| postman-mcp-validation | postman_mcp_validation.exe | Crear implementación Rust |

**Criterio de aceptación:** Todas las tools en paths.toolCapsules tienen binario `.exe` que cumple tools-contract (salida JSON con campos requeridos).

---

### Punto 4: Eliminación de fallbacks .ps1 (estrategia)

**Descripción:** Los contratos prohíben `.ps1` como implementación principal. Definir estrategia de migración.

**Opciones:**
- **A)** Eliminar `.ps1` cuando exista `.exe` funcional; el `.bat` solo invoca `.exe`.
- **B)** Mantener `.ps1` como fallback documentado "pendiente de migración" hasta que todos los consumidores usen `.exe`.
- **C)** Eliminar `.ps1` de forma gradual por skill/tool según se complete cada binario.

**Criterio de aceptación:** Decisión documentada en clarify.md; manifest.json de cada cápsula actualizado según la estrategia.

---

### Punto 5: Validación de cumplimiento

**Descripción:** Verificar que cada skill y tool cumple su contrato (Rust + JSON).

**Checks:**
- Cada skill/tool con ejecutable tiene `.exe` en `<cápsula>/bin/`.
- La salida JSON cumple campos requeridos (success, exitCode, message, feedback, timestamp).
- `scripts/skills-rs/install.ps1` y `scripts/tools-rs/install.ps1` copian correctamente los binarios.

**Criterio de aceptación:** Checklist de validación en validacion.md; proceso audit-tool aplicable a cada herramienta.

---

## Dependencias

- **Rust:** Toolchain instalado para compilar scripts/skills-rs y scripts/tools-rs.
- **Cúmulo:** Rutas canónicas en SddIA/agents/cumulo.paths.json.
- **Karma2Token:** Contexto de seguridad para trazabilidad (contratos).
