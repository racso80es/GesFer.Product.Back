# Informe de Auditoría: {{tool-id}}

**Fecha:** {{audit-date}}  
**Versión herramienta:** {{tool-version}}  
**Auditor:** {{auditor}}

---

## Resumen ejecutivo

| Campo | Valor |
|-------|-------|
| Herramienta | {{tool-id}} |
| Resultado | **{{result}}** |
| Duración | {{duration_ms}} ms |

---

## Resultado por fase

| Fase | Nombre | Resultado | Observaciones |
|------|--------|-----------|---------------|
| 0 | Preparar entorno | {{phase_0_result}} | {{phase_0_message}} |
| 1 | Definir objetivos | {{phase_1_result}} | {{phase_1_message}} |
| 2 | Analizar especificación | {{phase_2_result}} | {{phase_2_message}} |
| 3 | Diseñar pruebas | {{phase_3_result}} | {{phase_3_message}} |
| 4 | Ejecutar herramienta | {{phase_4_result}} | {{phase_4_message}} |
| 5 | Validar retorno JSON | {{phase_5_result}} | {{phase_5_message}} |
| 6 | Validar objetivos funcionales | {{phase_6_result}} | {{phase_6_message}} |
| 7 | Generar informe | {{phase_7_result}} | {{phase_7_message}} |
| 8 | Cierre | {{phase_8_result}} | {{phase_8_message}} |

---

## Validación de retorno JSON

| Campo | Esperado | Encontrado | Válido |
|-------|----------|------------|--------|
| toolId | {{tool-id}} | {{json_toolId}} | {{json_toolId_valid}} |
| exitCode | 0 | {{json_exitCode}} | {{json_exitCode_valid}} |
| success | true | {{json_success}} | {{json_success_valid}} |
| timestamp | ISO 8601 | {{json_timestamp}} | {{json_timestamp_valid}} |
| message | No vacío | {{json_message}} | {{json_message_valid}} |
| feedback | Array | {{json_feedback_count}} entradas | {{json_feedback_valid}} |

**Resultado validación JSON:** {{json_validation_result}}

---

## Validación funcional

{{#each functional_checks}}
### {{name}}

- **Criterio:** {{criteria}}
- **Resultado:** {{result}}
- **Detalle:** {{detail}}

{{/each}}

**Resultado validación funcional:** {{functional_validation_result}}

---

## Evidencias

### Comando ejecutado

```
{{command_executed}}
```

### Parámetros utilizados

```json
{{parameters_used}}
```

### Salida JSON capturada

```json
{{json_output}}
```

---

## Recomendaciones

{{#if recommendations}}
{{#each recommendations}}
- {{this}}
{{/each}}
{{else}}
Sin recomendaciones.
{{/if}}

---

## Referencias

- Herramienta: `{{capsule_path}}`
- Contrato: `SddIA/tools/tools-contract.json`
- Proceso: `SddIA/process/audit-tool/`

---

*Informe generado por proceso audit-tool. Versión del proceso: 1.0.0*
