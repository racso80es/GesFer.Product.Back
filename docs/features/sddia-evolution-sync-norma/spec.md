---
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
base: Requerimiento usuario 2026-03-27 (trazabilidad SddIA, contrato YAML v1.0, evolution centralizado)
status: planning
spec_version: 1.2-plan
---

# Especificación técnica — Norma de Sincronismo SddIA (evolution)

Este documento es la **propuesta formal** para revisión. No sustituye la aprobación explícita del usuario antes de fases **planning → execution**.

---

## 1. Resumen ejecutivo

| Ítem | Propuesta |
| :--- | :--- |
| **Problema** | Cambios en `./SddIA/` pueden quedar sin auditoría ni formato replicable entre entornos SSDD IA. |
| **Solución** | Contrato de Cambio **YAML v1.0** obligatorio + doble persistencia: índice maestro + fichero `{id_cambio}.md` por intervención. |
| **ID de cambio** | **GUID (UUID v4)** en campo `id_cambio`; fichero atómico `{id_cambio}.md`. Plan: `docs/features/sddia-evolution-sync-norma/plan.md`. |
| **SSOT rutas** | Nuevas claves en `SddIA/agents/cumulo.paths.json` (ver §3). |
| **Trigger** | Registro **obligatorio** para agentes; **watcher automático en alcance v1** (§6); implementación **solo** vía binario Rust y contrato capsule-json-io — **sin** `.ps1` como entrega principal (clarify CL-003). |

---

## 2. Alcance funcional

### 2.1 Incluye

1. **Norma** `SddIA/norms/sddia-evolution-sync.md` (comportamiento, ámbito `./SddIA/`, obligaciones).
2. **Contrato canónico** en `paths.sddiaEvolutionPath` + `paths.sddiaEvolutionContractFile` (`evolution_contract.md`): esquema YAML v1.0, campos obligatorios, reglas de hash (§4).
3. **Índice maestro** `Evolution_log.md` (nombre vía `paths.sddiaEvolutionLogFile`): tabla o líneas cronológicas con `id`, `fecha`, `descripcion_breve`.
4. **Detalle atómico** `{id_cambio}.md` (nombre = GUID): frontmatter YAML (contrato) + Markdown resumen.
5. **Actualización Cúmulo** y norma `paths-via-cumulo.md` con las nuevas claves.
6. **Constitución** (`SddIA/CONSTITUTION.md` + ley en `SddIA/constitution.json`): trazabilidad obligatoria del protocolo SddIA.
7. **Difusión IDE** (`.cursor/rules/sddia-evolution-sync.mdc` o equivalente): recordatorio de cumplimiento para Cursor/agentes.
8. **`.github/`** — Workflows y documentación alineados con el cumplimiento del registro cuando el diff toca `./SddIA/`, y con la difusión hacia **Jules** y CI (clarify CL-004; ver `SddIA/norms/touchpoints-ia.md`, acción `sddia-difusion`).

### 2.2 Excluye (v1)

- Reemplazar o migrar `docs/evolution/EVOLUTION_LOG.md` (se mantiene para cierres de producto/feature).
- Hook Git pre-commit **local** obligatorio (opcional; la prioridad es watcher + binario Rust + validación en CI según planning).
- **Scripts `.ps1`** como solución de registro o watcher: **excluidos** como entrega; sustituidos por binario Rust con contrato de cápsula (clarify CL-003).

---

## 3. Modelo de datos y rutas (Cúmulo)

Propuesta de claves en `cumulo.paths.json`:

| Clave | Valor propuesto |
| :--- | :--- |
| `sddiaEvolutionPath` | `./SddIA/evolution/` |
| `sddiaEvolutionLogFile` | `Evolution_log.md` |
| `sddiaEvolutionContractFile` | `evolution_contract.md` |

Toda documentación de comportamiento debe referir `paths.*`, no rutas literales.

---

## 4. Contrato YAML v1.1 (obligatorio en frontmatter del detalle)

Campos mínimos (planificación 1.2):

- `contrato_version` (subir a **1.1** con GUID), `id_cambio` (**UUID v4**), `fecha`, `autor`, `proyecto_origen_cambio`, `contexto`, `descripcion_breve`
- **Tipología de cambio (contexto amplio):** cada intervención se clasifica como **alta** (nuevo), **baja** (eliminación/deprecación explícita) o **modificación** (cambio sin alta/baja estructural clara). Se expresa mediante campo dedicado (p. ej. `tipo_operacion` o lista tipada) y/o entradas coherentes en `cambios_realizados` (clarify CL-002). Detalle exacto en `evolution_contract.md` tras planning.
- `cambios_realizados`: lista de `{ anterior, nuevo }` (en **baja**, documentar path retirado y sustituto o referencia al histórico si aplica).
- `impacto`: `Bajo` \| `Medio` \| `Alto`
- `replicacion.instrucciones`, `replicacion.hash_integridad`

**Hash:** SHA-256 en hex minúsculas del bloque YAML canónico (definición exacta en `evolution_contract.md` para evitar ambigüedad), o literal `SHA-256-PENDIENTE` hasta calcular.

---

## 5. Comportamiento de agentes e IDEs

- **Innegociable:** cualquier **alta, baja o modificación** bajo `./SddIA/` debe quedar registrada: generar `id`, actualizar índice, crear `{id}.md` con tipo de cambio y contrato completo — en la misma intervención o en el mismo PR (clarify CL-002).
- La norma se declara explícita en `SddIA/CONSTITUTION.md` y en **touchpoints** para alinear **Jules**, **Cursor** y CI con el protocolo (`SddIA/norms/touchpoints-ia.md`).

---

## 6. Automatización (trigger) — post clarify

| Componente | Descripción | Decisión |
| :--- | :--- | :--- |
| **Binario Rust** | Registro (ID, índice, detalle, hash) invocable por agentes/CI con **contrato JSON** capsule-json-io; cápsula registrada en Cúmulo (`paths.skillCapsules` o `paths.toolCapsules`). | **Obligatorio** (CL-003). Sin `.ps1` como entrega. |
| **Watcher automático** | Observación de cambios bajo `./SddIA/` disparando registro o validación según norma. | **En alcance v1** (CL-001). Diseño en planning: local, CI o ambos. |
| **CI / `.github`** | Paso(s) en `pr-validation.yml` (o workflow hermano) que validen trazabilidad en PRs que alteren `./SddIA/`, incluyendo flujos **Jules**/agente. | **Incluido** (CL-004). |
| **Checklist solo** | Norma sin ejecutable | Insuficiente como única medida; complementa al binario y CI. |

---

## 7. Git y proceso feature

- **Rama:** `feat/sddia-evolution-sync-norma` (creación solo vía skill **iniciar-rama**, no git directo en agente).
- **Documentación de tarea:** `paths.featurePath/sddia-evolution-sync-norma/` (esta carpeta).
- **Planning:** `plan.md` (v1.0). Siguiente fase: **implementation** → **execution**.

---

## 8. Estado del repositorio (transparencia)

Consta **borrador exploratorio** previo a esta SPEC (archivos bajo `SddIA/evolution/`, posible ajuste de `cumulo.paths.json`). Cualquier **`*.ps1`** de registro debe **retirarse** al implementar el binario Rust (clarify CL-003). Debe **revisarse y alinearse** con el SPEC y `clarify.md` en ejecución.

---

## 9. Criterios de aceptación (definición de hecho)

1. Cúmulo expone `paths.sddiaEvolutionPath`, `paths.sddiaEvolutionLogFile`, `paths.sddiaEvolutionContractFile` y la norma `paths-via-cumulo.md` las documenta.
2. Existen `evolution_contract.md` y contrato v1.0 con tipología **alta / baja / modificación**; el binario Rust genera o valida índice + detalle.
3. Norma `sddia-evolution-sync.md` publicada y coherente con `CONSTITUTION.md` / `constitution.json`.
4. Regla Cursor (o equivalente) difunde la obligación a agentes.
5. **Watcher** y/o validación en **CI** operativos según `plan.md` (alcance v1).
6. **`.github/`** actualizado: workflow(s) y/o README que no contradigan SddIA y refuercen cumplimiento en PRs que toquen `./SddIA/` (Jules / agentes).
7. Feature documentada en esta carpeta con `validacion.md` que confirma trazabilidad y revisión del pipeline.

---

## 10. Preguntas abiertas

- **Trasladadas a `clarify.md` §2** (watcher local vs CI, deprecación en bajas, colisión de IDs, bootstrap). Las de separación `SddIA/evolution/` vs `docs/evolution/` siguen siendo válidas: **sí** por defecto en SPEC; confirma si quieres excepciones.

---

## 11. Propuesta al usuario (decisión)

**Se solicita aprobación de este SPEC** para continuar con **planning** y luego implementación alineada (rama `feat/sddia-evolution-sync-norma` vía skill **iniciar-rama**).

Si deseas cambios: indica ajustes en §2–§6 o en las preguntas §10; se versionará `spec_version` en este mismo fichero.
