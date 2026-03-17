# Normas de interacción: disparadores (AGENTS)

Este fichero es la **sección de disparadores de interacción** del protocolo maestro. Se referencia desde [AGENTS.md](./AGENTS.md). El comportamiento ante estos disparadores lo define el dominio SddIA.

---

## DISPARADORES DE INTERACCIÓN (SddIA/norms/)

Cuando el usuario escriba uno de los **disparadores** definidos en SddIA, aplicar la norma correspondiente. El comportamiento ante estos disparadores lo define el dominio SddIA, no reglas externas.

| Disparador | Norma | Comportamiento |
| :--- | :--- | :--- |
| **#Skill** | [`SddIA/norms/interaction-triggers.md`](./SddIA/norms/interaction-triggers.md) | Sugerir las skills existentes del proyecto (listado en `SddIA/skills/README.md` y paths.skillCapsules). Mostrar skill_id y descripción; ofrecer elegir una o indicar qué necesita. |
| **#Action** | [`SddIA/norms/interaction-triggers.md`](./SddIA/norms/interaction-triggers.md) | Sugerir las acciones existentes del ciclo (listado en `SddIA/actions/`: spec, clarify, planning, implementation, execution, validate, finalize). Mostrar action_id y propósito; ofrecer elegir una o pedir detalle. |
| **#Process** | [`SddIA/norms/interaction-triggers.md`](./SddIA/norms/interaction-triggers.md) | Sugerir los procesos existentes de tarea (listado en paths.processPath/README.md: feature, bug-fix, refactorization, create-tool). Mostrar process_id y descripción; ofrecer elegir uno o pedir detalle. |
| **subir** | [`SddIA/norms/interaction-triggers.md`](./SddIA/norms/interaction-triggers.md) | **Ejecutar** `git push`: obtener rama con `git branch --show-current`, luego `git push -u origin <rama>`. PowerShell. Comprobar salida y reportar éxito o error. No solo documentar; ejecutar el comando. |

Listado canónico de disparadores (machine-readable): [`SddIA/norms/interaction-triggers.json`](./SddIA/norms/interaction-triggers.json). Si se añaden disparadores (p. ej. #Tool), documentarlos en `SddIA/norms/` y en esta tabla.

## Protocolos Mandatorios

| Protocolo | Norma | Descripción |
| :--- | :--- | :--- |
| **PR Acceptance** | [`SddIA/norms/pr-acceptance-protocol.md`](./SddIA/norms/pr-acceptance-protocol.md) | **OBLIGATORIO:** Compilación, Tests y Nomenclatura deben pasar antes de cualquier PR. |

---
*Referencia: AGENTS.md (protocolo maestro).*
