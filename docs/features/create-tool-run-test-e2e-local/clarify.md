---
type: clarify
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
tool_id: run-test-e2e-local
related_spec: docs/features/create-tool-run-test-e2e-local/spec.md
sddia_spec_ref: SddIA/tools/run-test-e2e-local/spec.md
---

# Clarify — run-test-e2e-local (create-tool)

Documento de **aclaraciones y decisiones** para cerrar ambigüedades de la especificación antes de **plan** / **implementation**. Las respuestas aquí tienen prioridad sobre interpretaciones sueltas del spec hasta que se revise el spec canónico en SddIA.

## 1. Comportamiento ante fallo de health (probe)

| Situación | Decisión |
|-----------|----------|
| `GET {AdminApiUrl}/health` o `GET {ProductApiUrl}/health` no responde 2xx (o timeout) | **Error**: `success: false`, `exitCode != 0`, no ejecutar `dotnet test`. Feedback `phase: probe`, `level: error`. |
| `SkipApiProbe` activado | No comprobar health; continuar con build/tests (riesgo aceptado por el usuario). |

**Motivo:** Los E2E fallarían de forma confusa si la API no está lista; mejor fallar explícito en probe.

## 2. Arranque de la API Product (y Admin)

| Pregunta | Decisión |
|----------|----------|
| ¿La herramienta debe levantar `dotnet run` de Product? | **No en v1.** Se asume que **Product** (y **Admin** si hace falta para datos) ya están levantados o que el usuario los arranca aparte. La tool solo valida health (salvo skip) y lanza tests. |
| ¿Cómo se alinea Admin con el login E2E? | El proceso de **Product** debe usar `AdminApi:BaseUrl` coherente con `AdminApiUrl` (config o `AdminApi__BaseUrl`). La tool **no** modifica un proceso ya en ejecución; puede documentar o exportar variables solo para **nuevos** procesos que ella misma lance en el futuro. |

**Motivo:** Evitar duplicar responsabilidades con `start-api` y mantener la tool enfocada en entorno + tests.

## 3. Variables de entorno para `dotnet test`

| Variable | Fuente | Decisión |
|----------|--------|----------|
| `E2E_BASE_URL` | `ProductApiUrl` normalizada (sin `/` final) | **Obligatoria** en el proceso de test. |
| `E2E_INTERNAL_SECRET` | Parámetro `E2EInternalSecret` o valor por defecto de `run-test-e2e-local-config.json` / spec | **Establecer siempre** en la ejecución de tests para alinear con `InternalSecret` de la API en Development. |
| `AdminApi__BaseUrl` | `AdminApiUrl` | **Solo** si en una versión futura la tool arranca Product; **v1:** no requerida en el proceso de test (los tests HTTP no leen esta variable). |

## 4. Normalización de URLs

- Entrada `AdminApiUrl` y `ProductApiUrl`: trim, eliminar barra final salvo que el spec de implementación use `Uri` de forma que requiera otra convención; documentar una sola convención (**sin barra final** en config y env).

## 5. Orden exacto de fases (sin ambigüedad)

1. **init** — Resolver raíz del repo, rutas a `Prepare-FullEnv.bat`, `Invoke-MySqlSeeds.bat`, `GesFer.Product.Back.E2ETests.csproj`.  
2. **prepare** — Solo si no `OnlyTests` y no `SkipPrepare`.  
3. **seeds** — Solo si no `OnlyTests` y no `SkipSeeds`.  
4. **probe** — Solo si no `SkipApiProbe`.  
5. **build** — `dotnet build` del proyecto E2E (o solución mínima que lo compile).  
6. **tests** — `dotnet test ... --filter "Category=E2E" --no-build` con env aplicado al proceso hijo.  
7. **done** / **error** — JSON contrato.

## 6. Implementación: Rust vs transición

- **Norma create-tool / tools-contract:** objetivo es ejecutable Rust en la cápsula.  
- **Decisión pragmática:** Se permite una **fase de transición** documentada en `implementation.md`: script PowerShell que cumpla el mismo contrato (salida JSON, parámetros, fases) **hasta** existir `run_test_e2e_local.exe`, alineado con el patrón de `run-tests-local` (migración pendiente a Rust). La herramienta no queda “cerrada” en calidad de producto hasta el `.exe`.

## 7. Preguntas cerradas sin pendientes

- **¿Incluir tests fuera de `Category=E2E`?** No; solo E2E de `GesFer.Product.Back.E2ETests` con el filtro indicado.  
- **¿Soporte Linux/macOS?** Fuera de alcance: **Windows 11 + PowerShell** como en el resto de tools del repo.

## 8. Actualización del spec SddIA (opcional)

Si en **implementation** se adopta la decisión §6 sin cambios, no es obligatorio editar `SddIA/tools/run-test-e2e-local/spec.md`; si se desea rigor documental, añadir un párrafo “Transición PowerShell permitida hasta binario Rust” para coherencia con §6.

---

*Tras este clarify, la tarea puede pasar a **plan** / **implementation** sin bloqueos conceptuales.*
