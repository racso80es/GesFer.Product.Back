# Norma: Implementación del contrato de principios en agentes (agents.principles)

**Fuente:** SddIA/norms. Contrato de la entidad principles: SddIA/principles/principles-contract.json.

## Principio

La **implementación del contrato de la entidad principles** en el dominio de agentes se expresa mediante el campo **`principlesContract`** (equivalente a *agents.principles*) en la definición JSON de cada agente que deba aplicar o custodiar principios técnicos.

- **Cúmulo:** Referencia el contrato de principios como SSOT; paths.principlesPath en cumulo.paths.json.
- **Arquitecto, Tekton:** Deben aplicar los principios definidos en paths.principlesPath (contrato principlesContract) en tareas de diseño e implementación.

## Contrato referenciado

- **Ruta canónica:** Cúmulo → paths.principlesPath (SddIA/principles/).
- **Contrato global:** paths.principlesPath/principles-contract.json (definición por principio: spec.md con frontmatter YAML en paths.principlesPath/<principle-id>/).

## Validación en acciones y procesos

- **Acciones** que afecten diseño o implementación (spec, planning, implementation, execution) deben declarar o validar coherencia con principles-contract (campo opcional `principles_applicable` o validación en flujo).
- **Procesos** deben indicar que las fases de diseño e implementación validan los principios (paths.principlesPath); el process-contract y los spec.json de cada proceso pueden incluir `principles_ref: "paths.principlesPath"` o equivalente.

## Aplicación

- Al cargar un agente con `principlesContract`, el ejecutor debe considerar los principios en paths.principlesPath para decisiones de diseño/código.
- Las acciones y procesos consumidores del contrato (actions-contract.json, process-contract.json) exigen validación de principios cuando corresponda.
