---
type: objective
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
---

# Objetivos

- Definir la herramienta **run-test-e2e-local** en `paths.toolsDefinitionPath` (SddIA) con contrato tools y referencia a `paths.toolCapsules.run-test-e2e-local`.
- Especificar la orquestación: entorno local preparado (prepare-full-env, invoke-mysql-seeds), comprobación de health de **Admin** y **Product**, y ejecución de **todos** los tests E2E (`Category=E2E`) con parámetros de entrada **AdminApiUrl** y **ProductApiUrl**.
- Registrar la cápsula en Cúmulo (`cumulo.paths.json`) y en `scripts/tools/index.json`.
- Dejar documentada la fase siguiente: implementación (ejecutable + launcher) según `tools-contract.md`.
- **Clarify completado** — `docs/features/create-tool-run-test-e2e-local/clarify.md` (decisiones explícitas para implementación).
