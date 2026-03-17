# Skill invoke-command — Cápsula de implementación

**skillId:** invoke-command  
**Ruta canónica:** Cúmulo paths.skillCapsules["invoke-command"] (scripts/skills/invoke-command/)

## Uso

Cualquier comando de sistema que el agente deba ejecutar ha de invocarse a través de esta skill. Desde la raíz del repo:

```powershell
.\scripts\skills\invoke-command\Invoke-Command.ps1 -Command 'git status' -Fase Accion
.\scripts\skills\invoke-command\Invoke-Command.bat --command "git status" --fase Accion
.\scripts\skills\invoke-command\Invoke-Command.bat --command-file "docs\features\mi-feature\commit_cmd.txt" --fase Accion
```

Parámetros: **Command** (obligatorio) o **--command-file** &lt;ruta&gt; (lee el comando desde un archivo; evita inyección de trailers en entornos automatizados), Contexto (default GesFer), Fase (Triaje|Analisis|Evaluacion|Marcado|Accion). Si existe `bin/invoke_command.exe` (Rust), el .bat lo invoca. Definición: SddIA/skills/invoke-command/spec.md.

## Rutas con --command-file

- **Con el .bat:** Usar **ruta absoluta** al archivo de comando para que el exe resuelva bien desde cualquier directorio de trabajo, por ejemplo: `--command-file "c:\Proyectos\MiRepo\docs\features\mi-feature\commit_cmd.txt"`.
- **Alternativa:** Ejecutar el exe directamente desde la **raíz del repo** (`.\scripts\skills\invoke-command\bin\invoke_command.exe --command-file "docs\features\mi-feature\commit_cmd.txt"`); entonces las rutas relativas al archivo se resuelven correctamente.
