# Normas SddIA

Este directorio contiene **normas de comportamiento** del agente definidas en el dominio SddIA. El agente debe consultarlas para responder ante ciertos disparadores del usuario.

| Documento | Descripción |
|------------|-------------|
| **interaction-triggers.md** | Disparadores de interacción (#Skill, #Action, #Process): cuándo aplican y qué comportamiento seguir. |
| **interaction-triggers.json** | Versión machine-readable de los disparadores. |
| **paths-via-cumulo.md** | Rutas solo desde Cúmulo (contrato de paths); no rutas literales. |
| **git-via-skills-or-process.md** | La IA nunca ejecuta git directamente; solo vía skill, herramienta, acción o proceso. |
| **commands-via-skills-or-tools.md** | La IA nunca ejecuta comandos de sistema directamente; solo vía skill, herramienta, acción o proceso. Implementación estándar: Rust (paths.skillsRustPath, paths.toolsRustPath). |
| **entidades-dominio-ecosistema-sddia.md** | Definición de entidades de dominio (ecosistema SddIA): las que integran el contrato/ítem Token; obligación de estructura (archivo .md con frontmatter YAML); excepción tokens JSON-only. |
| **agents-principles-contract.md** | Implementación del contrato de principios en agentes (principlesContract). |
| **patterns-in-planning-implementation-execution.md** | Aplicación de patrones en planning, implementation, execution. |
| **obediencia-procesos.md** | La IA no debe validar o cuestionar las indicaciones dadas por el proceso, sino ejecutarlas literalmente. |

**Referencia en protocolo:** AGENTS.md (leyes universales, disparadores) indica que el comportamiento lo define SddIA y remite a este directorio.
