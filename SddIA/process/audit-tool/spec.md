---
audit_output_ref: paths.auditsPath/tools/<tool-id>
configuration:
  cleanup_after_audit:
    configurable: true
    default: true
    description: >
      Limpieza contextual según tipología.
      - daemon: terminar proceso (graceful/kill) y subprocesos si aplica
      - batch: ejecutar reversión SOLO si está definida explícitamente por la tool (ver sección Limpieza)
      - pure-cli: eliminar temporales si aplica (p. ej. output-path)
  evolution_log:
    description: Solo registrar anomalías en Evolution Log.
    when: FAIL o PARTIAL
  parameters_source:
    description: 'Fuente de parámetros de invocación: config file de la herramienta.'
    fallback: sin parámetros
    pattern: <tool-id>-config.json
  report_naming:
    description: Versionado por fecha para trazabilidad.
    pattern: audit-report-YYYY-MM-DD-##.md / audit-result-YYYY-MM-DD-##.json
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
- description: Especificación técnica individual de la herramienta (spec.md en paths.toolsDefinitionPath/<tool-id>/).
  name: tool_spec_ref
  required: true
  type: reference
outputs:
- description: Informe legible con resultado, evidencias y recomendaciones.
  name: audit-report.md
  type: file
- description: 'Resultado machine-readable: tool_id, audit_date, result, phases_results, evidence.'
  name: audit-result.json
  type: file
persist_ref: paths.featurePath/audit-tool-<tool-id>
phases:
- description: Ejecutar git-workspace-recon para validar entorno limpio. Opcionalmente aislar con git-branch-manager (p. ej. feat/audit-tool-<tool-id>). Verificar existencia en paths.toolCapsules y clasificar tipología (Daemon, Batch, Pure-CLI).
  id: '0'
  name: Preparar entorno y Clasificar
- description: Documentar criterios de éxito en objectives.md, anclados al tool_spec_ref.
  id: '1'
  name: Definir objetivos dinámicos
- description: Extraer del tool_spec_ref promesas (inputs, flags) y fases esperadas (output.phases_feedback). Validar compatibilidad con tools-contract v2.
  id: '2'
  name: Analizar especificación y Contrato
- description: Crear casos de prueba garantizando cobertura contractual para cada Input y flag definidos en la especificación.
  id: '3'
  name: Diseñar pruebas
- description: Invocar ejecutable (.exe). Capturar JSON emitido por stdout o por archivo configurado por la propia tool (si aplica). Si la auditoría muta el repo (informes, notas), consolidar hitos con git-save-snapshot; ante fallo estructural, git-tactical-retreat.
  id: '4'
  name: Ejecutar herramienta
- description: Validar estructura contra tools-contract.md (v2) y comparar feedback[].phase contra tool_spec_ref.output.phases_feedback.
  id: '5'
  name: Validar retorno JSON y Trazabilidad
- description: Ejecutar aserciones de estado funcional basadas estrictamente en la sección 'Objetivo' del tool_spec_ref, adaptadas a la tipología.
  id: '6'
  name: Validar objetivos funcionales
- description: Crear audit-report.md y audit-result.json con veredicto PASS/FAIL/PARTIAL.
  id: '7'
  name: Generar informe
- description: Ejecutar cleanup_after_audit según tipología y escribir entregables bajo audit_output_ref. Evolution Log solo en FAIL/PARTIAL. Publicar cambios con git-sync-remote; abrir git-create-pr enlazando objectives.md, validacion.md y rutas de audit_output_ref en el cuerpo del Pull Request cuando corresponda integración.
  id: '8'
  name: Cierre y Limpieza
principles_ref: paths.principlesPath
process_id: audit-tool
name: Auditoría y Validación de Herramientas
description: >
  Proceso para auditar empírica y contractualmente una herramienta: ejecutabilidad,
  cumplimiento del envelope JSON (tools-contract v2) y cumplimiento funcional del objetivo declarado en su spec.
related_actions:
- spec
- validate
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
spec_version: 4.0.0
tools_contract_ref: SddIA/tools/tools-contract.md
---

# Proceso: Auditoría de herramientas (audit-tool)

Este documento define el **proceso de tarea** para auditar una herramienta (tool) existente en el proyecto. Está ubicado en paths.processPath/audit-tool/ (Cúmulo). Las rutas de herramientas y auditorías se obtienen de **Cúmulo** (paths.toolsPath, paths.toolCapsules, paths.auditsPath).

## Interfaz de proceso vs entregables de auditoría

**Carpeta de la tarea (persist_ref):** solo artefactos `.md` con frontmatter YAML + cuerpo (ej. objectives.md, plan.md, validacion.md). **No se generan `.json` en la carpeta de la tarea** (compatibilidad con `paths.processPath/process-contract.md`).

**Carpeta de auditoría (audit_output_ref):** entregables finales:
- `audit-report-YYYY-MM-DD-##.md`
- `audit-result-YYYY-MM-DD-##.json`

## Propósito

El proceso **audit-tool** define el procedimiento para verificar empíricamente que una herramienta del ecosistema funciona correctamente según su especificación. Garantiza que:

1. La herramienta se ejecuta sin errores (invocación del `.exe` o script).
2. El retorno JSON cumple el contrato de herramientas (SddIA/tools/tools-contract.md).
3. Los objetivos funcionales declarados en la herramienta se cumplen (validación directa).

## Taxonomía de herramientas (clasificación operativa)

En Fase 0 la herramienta se clasifica en una tipología que determina la estrategia de validación (Fase 6) y de limpieza (Fase 8):

1. **Daemon / Servicio:** proceso continuo que expone interfaces.
   - Validación: disponibilidad (health/puerto/interfaz).
   - Limpieza: terminación del proceso y subprocesos si aplica.
2. **Batch / Mutador de entorno:** ejecución finita con efecto secundario (Docker, migraciones, seeds).
   - Validación: efecto secundario observable.
   - Limpieza: reversión SOLO si está definida explícitamente por la tool (ver sección Limpieza).
3. **Pure-CLI / Transformador:** ejecución finita sin efectos secundarios ambientales.
   - Validación: output/artefactos y coherencia de `result`.
   - Limpieza: borrar temporales si aplica.

## Alcance del procedimiento

- **Documentación de la tarea:** Cúmulo (paths.featurePath/audit-tool-<tool-id>/).
- **Informe de auditoría:** paths.auditsPath/tools/<tool-id>/audit-report.md, audit-result.json.
- **Herramienta objetivo:** paths.toolCapsules[<tool-id>].

## Fases del proceso

| Fase | Nombre | Descripción |
|:-----|:-------|:------------|
| 0 | Preparar entorno | **git-workspace-recon**; opcional **git-branch-manager** (feat/audit-tool-<tool-id>). Verificar herramienta en paths.toolCapsules. |
| 1 | Definir objetivos | Documentar qué se va a auditar: objectives.md con criterios de éxito. |
| 2 | Analizar especificación | Revisar manifest.json y documentación de la herramienta para identificar criterios de validación. |
| 3 | Diseñar pruebas | Definir casos de prueba: invocación, parámetros, validaciones esperadas. |
| 4 | Ejecutar herramienta | Invocar el .exe (o script) y capturar salida JSON. |
| 5 | Validar retorno JSON | Verificar estructura y campos según tools-contract.md. |
| 6 | Validar objetivos funcionales | Confirmar que la herramienta logra su objetivo (ej: API levantada, health OK). |
| 7 | Generar informe | audit-report.md y audit-result.json con resultado: PASS/FAIL, evidencias. |
| 8 | Cierre | Actualizar paths.auditsPath; **git-sync-remote** y **git-create-pr** con artefactos de la tarea cuando proceda; opcional Evolution Log. |

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
- La ejecución debe ser con el `.exe` en la raíz de la cápsula. `.bat` (si existe) es solo launcher humano, no fuente de verdad.
- El resultado JSON de la herramienta debe cumplir el contrato (v2): envelope unificado y payload en `result`.
- La auditoría debe ser reproducible: documentar comandos ejecutados.

## Configuración

El proceso es configurable según las decisiones de clarificación:

| Opción | Valor por defecto | Descripción |
|--------|-------------------|-------------|
| `cleanup_after_audit` | `true` | Detener proceso de la herramienta tras validación |
| `parameters_source` | `<tool-id>-config.json` | Fuente de parámetros; informar al usuario antes de ejecutar |
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

## Plantilla canónica: `audit-report.md` (contenido embebido)

El archivo `audit-report-YYYY-MM-DD-##.md` debe seguir este esqueleto.

```markdown
---
process_id: audit-tool
spec_version: 4.0.0
tool_id: <tool-id>
audit_date: YYYY-MM-DD
audit_id: audit-YYYY-MM-DD-##
result: PASS | FAIL | PARTIAL
tools_contract_ref: SddIA/tools/tools-contract.md
tool_spec_ref: paths.toolsDefinitionPath/<tool-id>/spec.md
capsule_path_ref: paths.toolCapsules[<tool-id>]
---

## Resumen ejecutivo

- **Veredicto**: PASS/FAIL/PARTIAL
- **Tipología**: Daemon / Batch / Pure-CLI
- **Hallazgos clave**:
  - (1) …
  - (2) …
- **Recomendaciones**:
  - (P1) …
  - (P2) …

## Contexto y alcance

- **Objetivo de la tool (cita literal de tool_spec_ref/Objetivo)**:
  > “...”
- **Qué se audita**:
  - Ejecutabilidad (.exe)
  - Contrato estructural (tools-contract v2 / envelope)
  - Trazabilidad de fases (feedback[].phase)
  - Cumplimiento funcional del objetivo (aserciones)
- **Qué NO se audita** (si aplica): …

## Entradas y parámetros usados

- **Fuente de parámetros**: `<tool-id>-config.json` | “sin parámetros”
- **Comando(s) ejecutado(s)**:

```powershell
& "<capsule_path>\<tool_bin>.exe" ... --output-json ...
```

- **Variables de entorno** (si aplica):
  - `KEY=VALUE`

## Contrato de salida (tools-contract v2)

### Envelope: campos y coherencia success/exitCode

- **JSON emitido**: stdout | output-path
- **Comprobaciones**:
  - `success` y `exitCode` coherentes
  - `feedback[]` válido (phase/level/message/timestamp)
  - payload en **`result`** (no `data`)
- **Evidencia**: fragmentos mínimos, sanitizados.

### Trazabilidad de fases (feedback)

- **Fases esperadas (SSOT)**: `tool_spec_ref.frontmatter.output.phases_feedback`
- **Fases observadas**: derivadas de `feedback[].phase`
- **Comparación**:
  - Fases faltantes: …
  - Fases inesperadas: …
  - Reglas aplicadas: repetición permitida; done/error según success.

## Validación funcional del objetivo (fase 6)

> Regla: las aserciones salen **solo** de la sección “Objetivo” del tool_spec_ref.

### Aserciones ejecutadas

- **A1**: …  
  - Resultado: OK/FAIL  
  - Evidencia: …
- **A2**: …

## Limpieza / cierre

- `cleanup_after_audit`: true/false
- **Acción de cleanup** (según tipología): …
- **Resultado**: OK/NO-OP/FAIL

## Evidencias

- Archivos: `audit-result-YYYY-MM-DD-##.json`, …
- Logs relevantes (resumen): …

## Anomalías y recomendaciones

### Anomalías
- [CRITICAL] …
- [HIGH] …
- [MEDIUM] …

### Recomendaciones accionables
- P1: …
- P2: …

## Apéndice: Matriz de pruebas (fase 3)

| Caso | Parámetros | Esperado | Observado | Resultado |
|------|------------|----------|-----------|-----------|
| T1 | … | … | … | PASS |
| T2 | … | … | … | FAIL |
```

## Plantilla canónica: `audit-result.json` (contenido embebido)

El archivo `audit-result-YYYY-MM-DD-##.json` debe seguir este esquema (ejemplo).

```json
{
  "schema_version": "1.0.0",
  "process_id": "audit-tool",
  "spec_version": "4.0.0",
  "audit_date": "YYYY-MM-DD",
  "audit_id": "audit-YYYY-MM-DD-##",
  "tool_id": "<tool-id>",
  "capsule_path": "paths.toolCapsules[<tool-id>]",
  "tool_spec_ref": "paths.toolsDefinitionPath/<tool-id>/spec.md",
  "tools_contract_ref": "SddIA/tools/tools-contract.md",
  "result": "PASS",
  "summary": {
    "tipologia": "daemon | batch | pure-cli",
    "executed": true,
    "json_emitted": true,
    "contract_v2_ok": true,
    "trazabilidad_ok": true,
    "objetivos_ok": true
  },
  "inputs_used": {
    "parameters_source": "<tool-id>-config.json | none",
    "command_line": ["<tool_bin>.exe", "..."],
    "env": {}
  },
  "tool_observed_output": {
    "capture_mode": "stdout | file",
    "success": true,
    "exitCode": 0,
    "duration_ms": 0,
    "envelope_fields_present": ["meta", "success", "exitCode", "message", "feedback", "result", "duration_ms"],
    "anomalies": []
  },
  "expected_phases": ["<phase>"],
  "observed_phases": ["<phase>"],
  "traceability": {
    "missing_expected_phases": [],
    "unexpected_phases": [],
    "rules_applied": ["phases_may_repeat", "done_error_rules"]
  },
  "phases_results": [
    {
      "phase_id": "0",
      "name": "Preparar entorno y Clasificar",
      "status": "PASS | FAIL | PARTIAL",
      "evidence": [{ "kind": "<kind>", "detail": "<detail>" }],
      "anomalies": []
    }
  ],
  "evidence": { "files": [], "snippets": [], "metrics": {} },
  "anomalies": [{ "severity": "info | warning | error", "code": "<CODE>", "message": "<message>" }],
  "recommendations": [{ "priority": "low | medium | high", "action": "<action>", "ref": "<ref>" }]
}
```
