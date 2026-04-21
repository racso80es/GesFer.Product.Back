---
action_id: sddia-difusion
contract_ref: actions-contract.md
flow_steps:
- Comprobar fuentes canónicas
- Comparar .cursor/rules
- Actualizar reglas
- Documentar .github y otros
inputs:
- Estado SddIA
- Estado touchpoints
outputs:
- .cursor/rules actualizados
- Documentación touchpoints
---

# Action: Difusión de SddIA

## Propósito

La acción **difusión de SddIA** (sddia-difusion) mantiene alineados los **puntos de interacción con la IA** (gestores como Cursor, Jules, y artefactos en .github) con las normas y el protocolo definidos en **SddIA** (AGENTS.md, SddIA/norms/, SddIA/agents/). Objetivo: que cualquier gestor de IA que opere sobre el repositorio respete las mismas leyes, disparadores y referencias (Cúmulo) que define el dominio SddIA.

**Principio:** SddIA es la **única fuente de verdad** (SSOT) para comportamiento de agentes. Los archivos en `.cursor/rules`, `.github` o configuraciones de otros gestores son **difusión**: reflejo actualizado de SddIA para ese canal, no fuentes alternativas.

## Fuentes canónicas (SddIA)

| Fuente | Contenido |
|--------|-----------|
| **AGENTS.md** | Protocolo maestro: leyes universales, procesos, activación de roles, disparadores. |
| **SddIA/norms/** | Normas de interacción (interaction-triggers.md, paths-via-cumulo.md) y listados canónicos. |
| **SddIA/agents/** | Definiciones de agentes y Cúmulo (paths, instructions). |
| **SddIA/process/** | Procesos de tarea (listado canónico en paths.processPath/README.md: feature, bug-fix, refactorization, create-tool, correccion-auditorias, create-pattern, create-principle, create-template, audit-tool, create-skill, validate-pull-requests, …). |
| **SddIA/actions/** | Acciones del ciclo (spec, clarify, …, finalize, **sddia-difusion**). |

Las rutas concretas se resuelven desde **Cúmulo** (paths.actionsPath, paths.processPath, paths.normsPath, etc.); no usar rutas literales en documentación de comportamiento.

## Puntos de interacción (touchpoints)

| Gestor / Ubicación | Propósito | Cómo mantener alineado |
|-------------------|-----------|-------------------------|
| **.cursor/rules/** | Reglas que Cursor aplica al asistente. | Cada .mdc debe declarar que su contenido es difusión de SddIA; listados y comportamiento deben coincidir con SddIA/norms e interaction-triggers. Revisar al cambiar procesos, acciones o skills. |
| **.github/** | Workflows, issue templates, PR templates. | Al añadir o modificar .github, referenciar AGENTS.md y SddIA (leyes GIT, procesos, rutas vía Cúmulo). No duplicar normas; enlazar a docs/ o SddIA. |
| **Jules (u otros)** | Configuración específica del gestor. | Misma regla: SddIA es SSOT. Documentar en SddIA/norms o en docs/ cómo se difunde a Jules (archivo de config, prompt de sistema, etc.). |

## Entradas

- **Estado actual de SddIA:** AGENTS.md, SddIA/norms/interaction-triggers.md, SddIA/process/README.md, listados en SddIA/skills y SddIA/actions.
- **Estado actual de los touchpoints:** Contenido de .cursor/rules, .github (si existe), y cualquier doc de integración con Jules u otros.

## Salidas

- **.cursor/rules actualizados:** Reglas que reflejen procesos (incl. refactorization), acciones, skills y rutas vía Cúmulo; y una regla maestra que indique que SddIA es SSOT.
- **Documentación de touchpoints:** Fichero o sección que describa qué artefactos son difusión y cómo revisarlos (p. ej. en SddIA/norms o docs/).

## Flujo de ejecución (propuesto)

1. **Comprobar fuentes canónicas:** Leer AGENTS.md, interaction-triggers.md, process/README.md, listados de actions y skills.
2. **Comparar con .cursor/rules:** Verificar que cada disparador (#Skill, #Action, #Process, subir) y listados coincidan con SddIA; que no se usen rutas literales en contradicción con paths-via-cumulo.
3. **Actualizar .cursor/rules:** Ajustar tablas, descripciones y fuentes; añadir regla maestra «SddIA como SSOT / difusión».
4. **Documentar .github y otros gestores:** Si existe .github, revisar que no contradiga SddIA; si se usa Jules, documentar dónde y cómo se aplican las normas (SddIA/norms o docs/).

## Integración con agentes

- **Cúmulo:** Proporciona paths (paths.actionsPath, paths.processPath, paths.normsPath) para referenciar fuentes.
- **Arquitecto / Tekton:** Pueden ejecutar esta acción cuando se añadan procesos, acciones o disparadores en SddIA, para propagar los cambios a .cursor y, si aplica, .github.

## Criterios de aceptación

- [ ] Existe una regla en .cursor/rules que declare SddIA como SSOT para comportamiento del asistente (sddia-ssot.mdc).
- [ ] Existe una regla en .cursor/rules que difunda la norma de ejecución de comandos solo vía skills o tools (commands-via-skills-tools.mdc), alineada con SddIA/norms/commands-via-skills-or-tools.md.
- [ ] Las reglas de Cursor reflejan que la implementación estándar de skills y tools es Rust (paths.skillsRustPath, paths.toolsRustPath; Cúmulo).
- [ ] Los listados en .cursor/rules (process, action, skill) coinciden con SddIA/norms e interaction-triggers.
- [ ] Las reglas de Cursor no usan rutas literales que contradigan paths-via-cumulo (preferir paths.featurePath, paths.skillCapsules, etc.).
- [ ] Queda documentado cómo mantener alineados .github y otros gestores (Jules) con SddIA (SddIA/norms/touchpoints-ia.md).
- [ ] **Paridad MD/JSON:** Las definiciones en paths.skillsDefinitionPath y paths.processPath que se difunden (listados, contratos) tienen spec.md y spec.json coherentes; al actualizar un .md de skill o process, se actualiza el .json correspondiente. Ref: refactorization-sincronidad-md-json. // [ACTION-REQUIRED]: [REF-SddIA] sddia-difusion — acción refactorization-sincronidad-md-json.

---
*Acción de soporte al ciclo: asegura que los gestores de IA respeten las normas indicadas en AGENTS y SddIA.*
