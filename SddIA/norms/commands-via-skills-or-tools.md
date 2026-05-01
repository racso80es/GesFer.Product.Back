# Norma: Comandos solo vía skill, herramienta, acción o proceso

**Fuente:** SddIA/norms. Aplicable a todo agente o IA que opere en el repositorio.

## Principio

**La IA nunca debe ejecutar comandos de sistema directamente** en la shell (ni `git`, `dotnet`, `npm`, `pwsh`, `cargo`, ni cualquier otro comando). Toda ejecución de comandos ha de realizarse **siempre** a través de al menos uno de los siguientes canales:

- **Skill:** p. ej. **git-workspace-recon**, **git-branch-manager**, **git-save-snapshot** / **invoke-commit**, **git-sync-remote**, **git-tactical-retreat**, **git-create-pr** (suite táctica Git) y **invoke-command** como interceptor genérico cuando no exista skill dedicada. Contrato: paths.skillsDefinitionPath/\<skill-id\>/. Implementación obligatoria en Rust (paths.skillsRustPath); launcher .bat invoca solo .exe en bin/.
- **Herramienta (tool):** definida en paths.toolsDefinitionPath, implementación en paths.toolCapsules[tool-id] o paths.toolsRustPath. Implementación por defecto en Rust (binarios .exe).
- **Acción:** definida en paths.actionsPath (spec, planning, implementation, execution, validate, finalize-process, sddia-difusion, etc.). Las acciones orquestan skills o herramientas que ejecutan los comandos; no ejecutan el SO ni scripts directamente (contrato paths.actionsPath/actions-contract.md).
- **Proceso:** definido en paths.processPath (feature, bug-fix, create-tool, etc.). Los procesos invocan acciones y skills; la IA sigue el proceso en lugar de lanzar comandos por su cuenta.

## Contexto de implementación (Rust)

En SddIA, la **implementación estándar** de skills y tools es **Rust** (binarios .exe). Las rutas paths.skillsRustPath y paths.toolsRustPath (Cúmulo) identifican los proyectos Rust; las cápsulas (paths.skillCapsules, paths.toolCapsules) exponen launchers .bat que invocan **únicamente** el .exe en `bin/`.

## Condición .exe obligatorio (control IA)

**Regla de control para toda IA:**

1. **Solo ejecutar .exe:** La IA únicamente ha de invocar binarios `.exe` (skills y tools). No ejecutar `.ps1`, `.bat` ni `.sh` como implementación de la skill o tool.
2. **Si no existe .exe:** Cuando la IA necesite una skill o tool y no encuentre el `.exe` en `<cápsula>/bin/`, debe **añadir todo lo necesario para generarlo**:
   - **Skills:** Fuente Rust en paths.skillsRustPath, entrada en Cargo.toml, definición en paths.skillsDefinitionPath, cápsula en paths.skillCapsules (manifest.json, launcher .bat que invoque solo .exe). Ejecutar `scripts/skills-rs/install.ps1` para compilar.
   - **Tools:** Proceso create-tool (paths.processPath/create-tool/): definición en paths.toolsDefinitionPath, fuente Rust en paths.toolsRustPath, cápsula en paths.toolCapsules. Ejecutar `scripts/tools-rs/install.ps1` para compilar y copiar el .exe.
3. **Sin fallback .ps1:** El .bat en la cápsula invoca solo el .exe; si no existe, error explícito indicando ejecutar el install.ps1 correspondiente.

## Justificación

- **Trazabilidad:** Las skills/herramientas/acciones registran y auditan las operaciones (p. ej. Karma2Token, invoke-command).
- **Consistencia:** Mismo flujo para humanos y agentes; no hay "atajos" que eviten la validación.
- **Cumplimiento:** Ramas feat/fix, commits convencionales y nomenclatura se aplican cuando el flujo pasa por los artefactos definidos en SddIA.

## Aplicación

- Si la IA necesita ejecutar cualquier comando (git, dotnet, npm, etc.), debe **proponer** o **ejecutar** la invocación mediante la skill, herramienta, acción o proceso correspondiente (p. ej. "Ejecutar git-workspace-recon y git-branch-manager", "Consolidar con git-save-snapshot o invoke-commit", "Publicar con git-sync-remote", "Crear PR con git-create-pr" o "Invocar invoke-command con el comando indicado"), **nunca** escribiendo el comando directamente en la terminal.
- Los agentes que ejecutan código (p. ej. Tekton) tienen esta restricción en su contrato; esta norma la extiende a **cualquier** contexto de IA en el proyecto.
- **Referencia en protocolo:** AGENTS.md (ley GIT y esta norma). Norma específica para git: git-via-skills-or-process.md. Skills: suite táctica Git (git-workspace-recon, git-branch-manager, git-save-snapshot, git-sync-remote, git-tactical-retreat, git-create-pr) + invoke-commit + invoke-command (fallback).
