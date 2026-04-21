---
type: spec
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
tool_id: run-test-e2e-local
implementation_path_ref: paths.toolCapsules.run-test-e2e-local
clarify_ref: docs/features/create-tool-run-test-e2e-local/clarify.md
clarify_status: completed
---

# Especificación de tarea (proceso create-tool)

## Alcance de esta fase (spec iniciada)

Se ha creado la **definición canónica** de la herramienta en:

- `SddIA/tools/run-test-e2e-local/spec.md` — especificación legible (frontmatter YAML + cuerpo).
- `SddIA/tools/run-test-e2e-local/spec.json` — metadatos e inputs para consumo machine-readable.

La **implementación ejecutable** (Rust `.exe`, launcher `.bat`, manifest en cápsula) queda **pendiente** de la fase **implementation** del proceso create-tool.

## Comportamiento acordado (resumen)

| Entrada | Uso |
|---------|-----|
| **AdminApiUrl** | URL base API Admin (default `http://localhost:5010`): probe `GET /health`; referencia para alinear arranque de Product (`AdminApi__BaseUrl`) si la tool arranca el proceso. |
| **ProductApiUrl** | URL base API Product (default `http://localhost:5020`): se exporta como **`E2E_BASE_URL`** al ejecutar `dotnet test` sobre `GesFer.Product.Back.E2ETests` con filtro `Category=E2E`. |

Orden lógico de fases: **init** → **prepare** (opcional) → **seeds** (opcional) → **probe** health Admin/Product → **build** → **tests** → **done** / **error**. Salida JSON según `SddIA/tools/tools-contract.md`.

## Dependencias de otras tools

- `prepare-full-env`
- `invoke-mysql-seeds`

## Criterios de cierre de la fase spec

- [x] `spec.md` y `spec.json` presentes bajo `SddIA/tools/run-test-e2e-local/`.
- [x] `implementation_path_ref` apunta a `paths.toolCapsules.run-test-e2e-local`.
- [x] Cúmulo e índice de tools actualizados con el `toolId` y la ruta de cápsula.
- [x] Implementación en cápsula (`Run-Test-E2E-Local.ps1`, `.bat`, manifest, doc).

## Fase clarify

- [x] `clarify.md` — decisiones sobre probe, arranque de API, variables de entorno, orden de fases, transición Rust/PowerShell: ver `clarify.md`.
