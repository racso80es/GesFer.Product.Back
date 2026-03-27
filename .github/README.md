# .github — Difusión de SddIA

Los artefactos en esta carpeta (templates, workflows) son **difusión** de las normas definidas en **SddIA**. No duplican reglas; enlazan a la fuente canónica.

- **Protocolo maestro:** [AGENTS.md](../AGENTS.md) (leyes universales, procesos, roles).
- **Normas de interacción:** [SddIA/norms/](../SddIA/norms/) (interaction-triggers, paths-via-cumulo).
- **Touchpoints:** [SddIA/norms/touchpoints-ia.md](../SddIA/norms/touchpoints-ia.md).

Al modificar .github, mantener coherencia con SddIA (rutas vía Cúmulo, procesos feature/bug-fix/refactorization/create-tool). Acción de revisión: [SddIA/actions/sddia-difusion/](../SddIA/actions/sddia-difusion/).

## PRs que alteran `./SddIA/`

El workflow ejecuta `sddia_evolution_validate` cuando el diff incluye rutas bajo `SddIA/`. Los cambios deben ir acompañados de registro en el protocolo evolution (UUID, índice), según [SddIA/norms/sddia-evolution-sync.md](../SddIA/norms/sddia-evolution-sync.md). Alineado con [SddIA/norms/touchpoints-ia.md](../SddIA/norms/touchpoints-ia.md) (Jules / agentes).
