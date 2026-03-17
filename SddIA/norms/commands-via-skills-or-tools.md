# Norma: Comandos solo vía skill, herramienta, acción o proceso

**Fuente:** SddIA/norms. Aplicable a todo agente o IA que opere en el repositorio.

## Principio

**La IA nunca debe ejecutar comandos de sistema directamente** en la shell (ni `git`, `dotnet`, `npm`, `pwsh`, `cargo`, ni cualquier otro comando). Toda ejecución de comandos ha de realizarse **siempre** a través de al menos uno de los siguientes canales:

- **Skill:** p. ej. paths.skillCapsules.invoke-command (interceptor de comandos de sistema), paths.skillCapsules.iniciar-rama, paths.skillCapsules.finalizar-git. Contrato: paths.skillsDefinitionPath/\<skill-id\>/ (archivo .md con frontmatter YAML). Implementación por defecto en Rust (paths.skillsRustPath); fallback PS1/bat en cápsulas.
- **Herramienta (tool):** definida en paths.toolsDefinitionPath, implementación en paths.toolCapsules[tool-id] o paths.toolsRustPath. Implementación por defecto en Rust (binarios .exe).
- **Acción:** definida en paths.actionsPath (spec, planning, implementation, execution, validate, finalize, sddia-difusion, etc.). Las acciones orquestan skills o herramientas que ejecutan los comandos.
- **Proceso:** definido en paths.processPath (feature, bug-fix, create-tool, etc.). Los procesos invocan acciones y skills; la IA sigue el proceso en lugar de lanzar comandos por su cuenta.

## Contexto de implementación (Rust)

En SddIA, la **implementación estándar** de skills y tools es **Rust** (binarios .exe). Las rutas paths.skillsRustPath y paths.toolsRustPath (Cúmulo) identifican los proyectos Rust; las cápsulas (paths.skillCapsules, paths.toolCapsules) exponen launchers .bat/.ps1 que invocan el .exe si existe y, si no, el script de fallback.

## Justificación

- **Trazabilidad:** Las skills/herramientas/acciones registran y auditan las operaciones (p. ej. Karma2Token, invoke-command).
- **Consistencia:** Mismo flujo para humanos y agentes; no hay "atajos" que eviten la validación.
- **Cumplimiento:** Ramas feat/fix, commits convencionales y nomenclatura se aplican cuando el flujo pasa por los artefactos definidos en SddIA.

## Aplicación

- Si la IA necesita ejecutar cualquier comando (git, dotnet, npm, etc.), debe **proponer** o **ejecutar** la invocación mediante la skill, herramienta, acción o proceso correspondiente (p. ej. "Ejecutar skill iniciar-rama" o "Invocar invoke-command con el comando indicado"), **nunca** escribiendo el comando directamente en la terminal.
- Los agentes que ejecutan código (p. ej. Tekton) tienen esta restricción en su contrato; esta norma la extiende a **cualquier** contexto de IA en el proyecto.
- **Referencia en protocolo:** AGENTS.md (ley GIT y esta norma). Norma específica para git: git-via-skills-or-process.md. Skills: paths.skillCapsules.invoke-command, paths.skillCapsules.iniciar-rama, paths.skillCapsules.finalizar-git.
