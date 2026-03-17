---
command_file_routing:
  via_bat: Usar ruta absoluta al archivo (p. ej. --command-file con path absoluto) para que el exe resuelva bien desde cualquier directorio.
  via_exe_from_repo_root: Ejecutar el exe desde la raíz del repo; entonces las rutas relativas al archivo se resuelven correctamente.
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.invoke-command
parameters:
  command:
    description: Comando a ejecutar (obligatorio si no se usa command_file). Acepta -Command/--command.
    required: true
  command_file:
    description: Ruta a archivo cuyo contenido es el comando. Evita inyección en terminal. Acepta --command-file/-CommandFile.
    required: false
  contexto:
    default: GesFer
    required: false
  fase:
    default: Accion
    enum:
    - Triaje
    - Analisis
    - Evaluacion
    - Marcado
    - Accion
    required: false
rules:
- 'MANDATORY: Cualquier comando de sistema que el agente deba ejecutar ha de invocarse a través de esta skill.'
- 'Interface: Command (obligatorio) o --command-file; Contexto (default GesFer); Fase (Triaje|Analisis|Evaluacion|Marcado|Accion). El exe acepta -Command/-Fase y --command-file.'
- 'Compliance: AC-001 validación sintáctica; registro en docs/diagnostics/{branch}/execution_history.json; alineación Protocolo Racso-Tormentosa.'
scope_clarification: 'Aplica siempre que el agente ejecute cualquier comando de sistema: sin excepción para git, dotnet, npm, pwsh. No hay excepciones.'
skill_id: invoke-command
usage_instruction: Cada vez que el agente deba ejecutar un comando, invocar la cápsula invoke-command (paths.skillCapsules.invoke-command). Prohibido ejecutar comandos directamente en el shell.
---

# Skill: Invoke Command (Interceptor de ejecuciones de sistema)

**skill_id:** `invoke-command`

## Objetivo

Skill obligatoria para ejecutar comandos de sistema (git, dotnet, npm, pwsh, etc.) mediante un contrato implementado: telemetría, validación y registro por rama. Cualquier comando de sistema que el agente deba ejecutar ha de invocarse a través de esta skill; no hay excepciones.

## Alcance

- **Entrada:** Command (obligatorio) o --command-file &lt;ruta&gt; (lee el comando desde archivo; evita inyección en terminal), Contexto (default GesFer), Fase (Triaje | Analisis | Evaluacion | Marcado | Accion).
- **Salida:** Ejecución del comando con registro en docs/diagnostics/{branch}/execution_history.json; cumplimiento AC-001 y Protocolo Racso-Tormentosa.

## Reglas

- **MANDATORY:** Cualquier comando de sistema que el agente deba ejecutar ha de invocarse a través de esta skill.
- **Interface:** Usar el script o el .bat con Command (obligatorio) o --command-file, Contexto (default GesFer), Fase (Triaje|Analisis|Evaluacion|Marcado|Accion). El exe (Rust) acepta -Command/-Fase y --command-file.
- **Compliance:** AC-001 validación sintáctica; registro en docs/diagnostics/{branch}/execution_history.json; alineación Protocolo Racso-Tormentosa.
- **Scope:** Aplica siempre que el agente ejecute cualquier comando de sistema: sin excepción para git (status, add, commit, push, pull, branch, checkout), dotnet, npm, pwsh.

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:** `scripts/skills/invoke-command/bin/invoke_command.exe`  
**Fuente Rust:** `scripts/skills-rs/src/invoke_command.rs`

**Estándar:** Solo se generan ejecutables `.exe`. No se deben crear archivos `.ps1`.

## Integración con la cápsula

**Cápsula:** paths.skillCapsules["invoke-command"] (Cúmulo).  
**Launcher:** `Invoke-Command.bat` en la cápsula; invoca `bin/invoke_command.exe`.

### Invocación

```powershell
# Mediante launcher
.\scripts\skills\invoke-command\Invoke-Command.bat --command "git status" --fase Accion
.\scripts\skills\invoke-command\Invoke-Command.bat --command-file "docs\features\<nombre>\commit_cmd.txt" --fase Accion

# Invocación directa del ejecutable
& "scripts/skills/invoke-command/bin/invoke_command.exe" --command "git status" --fase Accion
```

**Rutas con --command-file:** (1) Con el .bat, usar **ruta absoluta** al archivo de comando (p. ej. `--command-file "c:\Proyectos\Repo\docs\features\X\commit_cmd.txt"`) para que el exe resuelva bien desde cualquier directorio. (2) Alternativa: ejecutar el exe directamente desde la **raíz del repo**; entonces las rutas relativas (p. ej. `docs\features\X\commit_cmd.txt`) se resuelven correctamente.

Prohibido ejecutar comandos directamente en el shell sin pasar por esta skill.

---
*Definición en paths.skillsDefinitionPath/invoke-command/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
