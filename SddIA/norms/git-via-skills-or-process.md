# Norma: Git solo vía skill, herramienta, acción o proceso

**Fuente:** SddIA/norms. Aplicable a todo agente o IA que opere en el repositorio.

## Principio

**La IA nunca debe ejecutar comandos git directamente** en la shell (ni `git status`, `git add`, `git commit`, `git push`, `git pull`, `git branch`, `git checkout`, etc.). Cualquier operación git ha de realizarse **siempre** a través de al menos uno de los siguientes canales:

- **Skill:** p. ej. paths.skillCapsules.invoke-command (interceptor de comandos de sistema), paths.skillCapsules.iniciar-rama, paths.skillCapsules.finalizar-git. Contrato: paths.skillsDefinitionPath/\<skill-id\>/ (archivo .md con frontmatter YAML).
- **Herramienta (tool):** definida en paths.toolsDefinitionPath, implementación en paths.toolCapsules[tool-id].
- **Acción:** definida en paths.actionsPath (spec, planning, implementation, execution, validate, finalize, etc.). Las acciones pueden orquestar skills o herramientas que ejecuten git.
- **Proceso:** definido en paths.processPath (feature, bug-fix, create-tool, create-principle, etc.). Los procesos invocan acciones y skills; la IA sigue el proceso en lugar de lanzar git por su cuenta.

## Justificación

- **Trazabilidad:** Las skills/herramientas/acciones registran y auditan las operaciones (p. ej. Karma2Token, invoke-command).
- **Consistencia:** Mismo flujo para humanos y agentes; no hay “atajos” que eviten la validación.
- **Cumplimiento:** Ramas feat/fix, commits convencionales y principio nomenclatura se aplican cuando el flujo pasa por los artefactos definidos en SddIA.

## Aplicación

- Si la IA necesita hacer una operación git (crear rama, commit, push, etc.), debe **proponer** o **ejecutar** la invocación mediante la skill, herramienta, acción o proceso correspondiente (p. ej. “Ejecutar skill iniciar-rama” o “Invocar invoke-command con el comando indicado”), **nunca** escribiendo `git ...` directamente en la terminal.
- Los agentes que ejecutan código (p. ej. Tekton) ya tienen esta restricción en su contrato (execution_contract, constraints); esta norma la extiende a **cualquier** contexto de IA en el proyecto.
- **Referencia en protocolo:** AGENTS.md, ley GIT y esta norma. Skills: paths.skillCapsules.invoke-command, paths.skillCapsules.iniciar-rama, paths.skillCapsules.finalizar-git.
