---
audit_output_ref: paths.auditsPath/tools/<tool-id>
configuration:
  cleanup_after_audit:
    configurable: true
    default: true
    description: Si true, detener el proceso de la herramienta tras la auditoría.
  evolution_log:
    description: Solo registrar anomalías en Evolution Log.
    when: FAIL o PARTIAL
  health_endpoint:
    description: URL del health endpoint.
    source: config.healthUrl o convención http://localhost:<port>/health
  parameters_source:
    description: 'Fuente de parámetros de invocación: config file de la herramienta.'
    fallback: sin parámetros
    pattern: <tool-id>-config.json
  report_naming:
    description: Versionado por fecha para trazabilidad.
    pattern: audit-report-YYYY-MM-DD.md / audit-result-YYYY-MM-DD.json
contract_ref: paths.processPath/process-contract.md
inputs:
- description: Identificador de la herramienta a auditar (kebab-case).
  name: tool-id
  required: true
  type: string
- description: paths.toolCapsules[<tool-id>]
  name: capsule_path
  required: true
  type: path
outputs:
- description: Informe legible con resultado, evidencias y recomendaciones.
  name: audit-report.md
  type: file
- description: 'Resultado machine-readable: tool_id, audit_date, result, phases_results, evidence.'
  name: audit-result.json
  type: file
persist_ref: paths.featurePath/audit-tool-<tool-id>
phases:
- description: Verificar existencia de herramienta en paths.toolCapsules. Rama opcional feat/audit-tool-<tool-id>.
  id: '0'
  name: Preparar entorno
- description: Documentar criterios de éxito en objectives.md.
  id: '1'
  name: Definir objetivos
- description: Revisar manifest.json y documentación de la herramienta.
  id: '2'
  name: Analizar especificación
- description: 'Definir casos de prueba: invocación, parámetros, validaciones esperadas.'
  id: '3'
  name: Diseñar pruebas
- description: Invocar .exe (o script fallback) y capturar salida JSON.
  id: '4'
  name: Ejecutar herramienta
- description: Verificar estructura según tools-contract.md.
  id: '5'
  name: Validar retorno JSON
- description: Confirmar que la herramienta logra su objetivo declarado.
  id: '6'
  name: Validar objetivos funcionales
- description: Crear audit-report.md y audit-result.json con resultado PASS/FAIL.
  id: '7'
  name: Generar informe
- description: Actualizar paths.auditsPath, opcional Evolution Log.
  id: '8'
  name: Cierre
principles_ref: paths.principlesPath
process_id: audit-tool
related_actions:
- spec
- validate
related_skills: []
spec_version: 1.0.0
templates_ref: paths.processPath/audit-tool/templates/
tools_contract_ref: SddIA/tools/tools-contract.md
---

# Proceso: Auditoría de herramientas (audit-tool)

Este documento define el **proceso de tarea** para auditar una herramienta (tool) existente en el proyecto. Está ubicado en paths.processPath/audit-tool/ (Cúmulo). Las rutas de herramientas y auditorías se obtienen de **Cúmulo** (paths.toolsPath, paths.toolCapsules, paths.auditsPath).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): la tarea de auditoría genera en la carpeta de la tarea (Cúmulo) al menos un **`.md`** (objectives.md, audit-report.md) y al menos un **`.json`** (audit-result.json). El **resultado** es un informe de auditoría en **paths.auditsPath/tools/<tool-id>/** que certifica si la herramienta cumple con sus objetivos.

## Propósito

El proceso **audit-tool** define el procedimiento para verificar empíricamente que una herramienta del ecosistema funciona correctamente según su especificación. Garantiza que:

1. La herramienta se ejecuta sin errores (invocación del `.exe` o script).
2. El retorno JSON cumple el contrato de herramientas (SddIA/tools/tools-contract.md).
3. Los objetivos funcionales declarados en la herramienta se cumplen (validación directa).

## Alcance del procedimiento

- **Documentación de la tarea:** Cúmulo (paths.featurePath/audit-tool-<tool-id>/).
- **Informe de auditoría:** paths.auditsPath/tools/<tool-id>/audit-report.md, audit-result.json.
- **Herramienta objetivo:** paths.toolCapsules[<tool-id>].

## Fases del proceso

| Fase | Nombre | Descripción |
|:-----|:-------|:------------|
| 0 | Preparar entorno | Verificar que la herramienta existe en paths.toolCapsules. Rama opcional feat/audit-tool-<tool-id>. |
| 1 | Definir objetivos | Documentar qué se va a auditar: objectives.md con criterios de éxito. |
| 2 | Analizar especificación | Revisar manifest.json y documentación de la herramienta para identificar criterios de validación. |
| 3 | Diseñar pruebas | Definir casos de prueba: invocación, parámetros, validaciones esperadas. |
| 4 | Ejecutar herramienta | Invocar el .exe (o script) y capturar salida JSON. |
| 5 | Validar retorno JSON | Verificar estructura y campos según tools-contract.md. |
| 6 | Validar objetivos funcionales | Confirmar que la herramienta logra su objetivo (ej: API levantada, health OK). |
| 7 | Generar informe | audit-report.md y audit-result.json con resultado: PASS/FAIL, evidencias. |
| 8 | Cierre | Actualizar paths.auditsPath, opcional Evolution Log. |

## Entradas

- **tool-id:** Identificador de la herramienta a auditar (kebab-case).
- **Cápsula de herramienta:** paths.toolCapsules[<tool-id>].
- **Contrato de herramientas:** SddIA/tools/tools-contract.md.

## Salidas

- **audit-report.md:** Informe legible con resultado, evidencias y recomendaciones.
- **audit-result.json:** Resultado machine-readable: tool_id, audit_date, result (PASS|FAIL), phases_results, evidence.

## Restricciones

- tool-id en kebab-case.
- Entorno: Windows 11, PowerShell 7+.
- La ejecución debe ser con el .exe cuando exista; fallback a .bat/.ps1.
- El resultado JSON de la herramienta debe cumplir el contrato (success, message, data opcional).
- La auditoría debe ser reproducible: documentar comandos ejecutados.

## Configuración

El proceso es configurable según las decisiones de clarificación:

| Opción | Valor por defecto | Descripción |
|--------|-------------------|-------------|
| `cleanup_after_audit` | `true` | Detener proceso de la herramienta tras validación |
| `parameters_source` | `<tool-id>-config.json` | Fuente de parámetros; informar al usuario antes de ejecutar |
| `health_endpoint` | Config o convención | Leer de `healthUrl` en config, fallback a `http://localhost:<port>/health` |
| `report_naming` | Fecha | `audit-report-YYYY-MM-DD.md`, `audit-result-YYYY-MM-DD.json` |
| `evolution_log` | Solo anomalías | Registrar en Evolution Log solo si FAIL o PARTIAL |

### Semántica de resultados

| Resultado | Cuándo aplica |
|-----------|---------------|
| **PASS** | Todas las validaciones (JSON + funcional) correctas |
| **FAIL** | Alguna validación crítica falla (no arranca, JSON inválido, objetivo no cumplido) |
| **PARTIAL** | Herramienta arranca y JSON válido, pero hay warnings o validaciones opcionales fallidas |

## Caso práctico: start-api

Para la herramienta **start-api**, los criterios de validación son:

1. Ejecución exitosa de `start_api.exe`.
2. Retorno JSON con `success: true`.
3. API levantada y accesible.
4. Endpoint de health (`/health` o similar) responde con estado OK.

## Referencias

- Contrato de herramientas: SddIA/tools/tools-contract.md, tools-contract.md.
- Cúmulo: paths.toolCapsules, paths.auditsPath, paths.toolsPath.
- Proceso machine-readable: paths.processPath/audit-tool/spec.json.
