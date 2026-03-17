---
contract_ref: SddIA/skills/skills-contract.md
implementation_path_ref: paths.skillCapsules.invoke-commit
parameters:
  all:
    aliases:
    - -a
    description: git add -A.
    required: false
    type: flag
  contexto:
    default: GesFer
    required: false
  fase:
    default: Accion
    required: false
  files:
    description: Rutas separadas por coma.
    required: false
  message:
    aliases:
    - -m
    description: Mensaje del commit.
    required: true
  scope:
    description: Scope Conventional Commits.
    required: false
  type:
    default: feat
    enum:
    - feat
    - fix
    - chore
    - docs
    - refactor
    required: false
rules:
- Para commits, invocar invoke-commit directamente; no es necesario invoke-command.
- Registro en docs/diagnostics/{branch}/execution_history.json.
skill_id: invoke-commit
---

# Skill: Invoke Commit

**skill_id:** invoke-commit

## Objetivo

Centralizar las operaciones de **commit** en Git con parámetros directos, consumible por acciones, procesos y flujos SddIA. Evita la generación de ficheros `.txt` intermedios (como commit_cmd.txt).

## Alcance

- **Entrada:** --message (obligatorio), --files (comma-separated) o --all, --type, --scope, --fase, --contexto.
- **Salida:** git add + git commit; registro en docs/diagnostics/{branch}/execution_history.json.

## Especificación

### Entradas

| Entrada | Tipo | Obligatorio | Descripción |
|---------|------|-------------|-------------|
| --message, -m | string | Sí | Mensaje del commit. |
| --files | string | Condicional | Rutas separadas por coma (ej. "a.md,b.json"). |
| --all, -a | flag | Condicional | git add -A. Excluyente con --files. |
| --type | string | No | feat, fix, chore, docs, refactor (default: feat). |
| --scope | string | No | Scope Conventional Commits. |
| --fase | string | No | Fase para telemetría (default: Accion). |
| --contexto | string | No | Contexto para registro (default: GesFer). |

### Salidas

- exitCode 0 si commit exitoso.
- Registro en docs/diagnostics/{branch}/execution_history.json.

### Relación con invoke-command

Para operaciones de **commit**, invoke-commit se invoca directamente; no es necesario usar invoke-command. invoke-commit es la vía preferida para commits.

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:** paths.skillCapsules["invoke-commit"]/bin/invoke_commit.exe  
**Fuente:** scripts/skills-rs/src/bin/invoke_commit.rs  
**Launcher:** Invoke-Commit.bat

### Invocación

```powershell
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "descripción" --files "a.md,b.json"
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "descripción" --all
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "detectar MySQL" --files "spec.md" --type feat --scope start-api
```

---
*Definición en paths.skillsDefinitionPath/invoke-commit/ (contrato SddIA/skills/skills-contract.md).*
