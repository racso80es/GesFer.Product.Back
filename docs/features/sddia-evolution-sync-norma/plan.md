---
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
purpose: Plan de implementación y ejecución tras respuestas clarify §2 (watcher dual, bajas, id GUID, bootstrap binario).
spec_ref: docs/features/sddia-evolution-sync-norma/spec.md
clarify_ref: docs/features/sddia-evolution-sync-norma/clarify.md
plan_version: "1.0"
decisions_planning:
  watcher_scope: "local_y_ci"
  baja_documentacion: "path_eliminado_mas_referencia_commit_previo"
  id_esquema: "guid_uuid_v4_como_id_cambio_en_yaml_y_nombre_fichero"
  bootstrap: "eliminar_borrador_SSDD_LOG_regenerar_con_primer_binario"
phases:
  - id: P1
    name: Contrato YAML v1.1 y SSOT rutas
  - id: P2
    name: Binarios Rust (registro, validación, watcher local)
  - id: P3
    name: Integración CI y .github
  - id: P4
    name: Norma, constitución, difusión Cursor y limpieza bootstrap
  - id: P5
    name: Validación y cierre documental
---

# Plan — sddia-evolution-sync-norma

Plan ejecutable alineado con **SPEC 1.1-clarify** y respuestas del usuario a `clarify.md` §2.

---

## Decisiones de planning (cerradas)

| # | Tema | Decisión |
| :---: | :--- | :--- |
| 1 | Watcher | **Ambos:** proceso local (desarrollo) **y** validación en **GitHub Actions** en PRs que tocan `./SddIA/`. |
| 2 | Bajas | **OK** al criterio SPEC: en `tipo_operacion: baja`, el detalle incluye **path eliminado** y **referencia al commit previo** (SHA corto o URL) donde el path aún existía. |
| 3 | Identificador | **`id_cambio`** en YAML = **GUID** (UUID v4, string canónico con guiones). Es el **único id** machine-readable; el nombre del fichero atómico será **`{id_cambio}.md`** (ej. `a1b2c3d4-e5f6-47a8-9abc-def012345678.md`). Ya no se usa `SSDD-LOG-YYYYMMDD-HHMM` como id principal (elimina colisiones por minuto). Opcional en índice: columna adicional `etiqueta` legible generada por herramienta si hace falta. |
| 4 | Bootstrap | **No** conservar `SSDD-LOG-20260327-1430`: eliminar borrador; **primera entrada oficial** = salida del **primer** registro exitoso del binario (post-implementación). |

---

## Fase P1 — Contrato YAML v1.1 y SSOT rutas

| ID | Tarea | Entregable |
| :--- | :--- | :--- |
| P1.1 | Versionar contrato a **v1.1** en `evolution_contract.md`: `id_cambio` (GUID), `tipo_operacion` (`alta` \| `baja` \| `modificacion`), campos opcionales para baja (`rutas_eliminadas`, `commit_referencia_previo`). | `SddIA/evolution/evolution_contract.md` |
| P1.2 | Asegurar `cumulo.paths.json` con `sddiaEvolutionPath`, `sddiaEvolutionLogFile`, `sddiaEvolutionContractFile`; documentar en `paths-via-cumulo.md`. | Cúmulo + norma paths |
| P1.3 | Actualizar `spec.md` (versión **1.2-plan**) tabla resumen: ID = GUID. | `docs/features/.../spec.md` |

---

## Fase P2 — Binarios Rust (`scripts/skills-rs`)

**Ubicación:** nuevo binario (o binarios) bajo `scripts/skills-rs/src/bin/`, copia vía `install.ps1` a cápsula en `scripts/skills/<id>/` o `scripts/tools/<id>/` según clasificación.

| ID | Tarea | Notas |
| :--- | :--- | :--- |
| P2.1 | **`sddia_evolution_register`** (nombre tentativo): envelope capsule-json-io; `request` con metadatos del cambio (autor, descripcion_breve, tipo_operacion, cambios_realizados, etc.); genera GUID; escribe `{guid}.md` y actualiza `Evolution_log.md`; calcula `hash_integridad`. | Skill o tool; definir en `SddIA/skills/` o `SddIA/tools/` + manifest. |
| P2.2 | **`sddia_evolution_validate`:** modo validación sin escribir (o solo dry-run): dado un árbol git o diff, comprueba que cada cambio bajo `./SddIA/` tenga entrada coherente en índice + fichero detalle esperado. Salida JSON para CI. | Invocado desde GA. |
| P2.3 | **Watcher local:** binario o subcomando (p. ej. `sddia_evolution_watch`) usando `notify` (o crate equivalente) sobre `./SddIA/` con debounce; ante cambio, invoca lógica de registro interactiva o escribe pendiente según diseño (mínimo: alerta + instrucción de ejecutar register con parámetros). | Documentar uso en cápsula; sin `.ps1`. |
| P2.4 | Dependencias: `uuid` para v4; reutilizar `gesfer-capsule` para I/O JSON. | `Cargo.toml` |
| P2.5 | Actualizar `scripts/skills-rs/install.ps1` para copiar los nuevos `.exe` a la cápsula acordada. | install.ps1 |

---

## Fase P3 — Integración CI y `.github`

| ID | Tarea | Entregable |
| :--- | :--- | :--- |
| P3.1 | Añadir paso en **`.github/workflows/pr-validation.yml`** (o job paralelo): checkout + `cargo build` + ejecutar **`sddia_evolution_validate`** con rango `base...HEAD` cuando `git diff --name-only` incluya `SddIA/**`. | Workflow YAML |
| P3.2 | Documentar en **`.github/README.md`** el vínculo con Jules/agentes: PRs que alteran SddIA deben pasar validación evolution; enlace a `touchpoints-ia.md` y norma `sddia-evolution-sync`. | README |
| P3.3 | Revisar **PULL_REQUEST_TEMPLATE.md**: checklist opcional “Si toca `./SddIA/`, evolution registrado”. | Template |

---

## Fase P4 — Norma, constitución, Cursor, bootstrap

| ID | Tarea | Entregable |
| :--- | :--- | :--- |
| P4.1 | Alinear `SddIA/norms/sddia-evolution-sync.md` con GUID, watcher dual, sin referencias obsoletas a SSDD-LOG tiempo. | Norma |
| P4.2 | Ley en `SddIA/constitution.json` + sección en `SddIA/CONSTITUTION.md` (trazabilidad protocolo, GUID). | Constitución |
| P4.3 | Crear/actualizar **`.cursor/rules/sddia-evolution-sync.mdc`** (difusión). | Cursor |
| P4.4 | **Limpieza bootstrap:** eliminar `SSDD-LOG-20260327-1430.md` (si existe); dejar `Evolution_log.md` con tabla vacía o fila placeholder “(pendiente primer registro por binario)” hasta primera ejecución; tras primera ejecución, índice con GUID real. | `SddIA/evolution/` |

---

## Fase P5 — Validación y cierre documental

| ID | Tarea |
| :--- | :--- |
| P5.1 | Redactar `validacion.md` con checks: build Rust, validate en CI simulado local, revisión manual índice + un `{guid}.md` de prueba. |
| P5.2 | Actualizar `execution.md` cuando Tekton ejecute (post-implementación). |

---

## Orden sugerido de implementación

```text
P1 → P2.1 + P2.4 → P2.2 → P2.5 → P4.4 (limpieza + primer register) → P2.3 → P3 → P4.1–P4.3 → P5
```

**Nota:** La primera ejecución exitosa de **register** tras P2.1 sustituye el bootstrap (decisión usuario #4).

---

## Riesgos y mitigaciones

| Riesgo | Mitigación |
| :--- | :--- |
| Validación CI demasiado estricta (falsos positivos) | Empezar con comprobación mínima: si hay diff en `SddIA/`, debe existir al menos un `.md` nuevo en `SddIA/evolution/` o línea nueva en índice; afinar en iteraciones. |
| Watcher local ruidoso | Debounce 1–2 s; ignorar `.git` y ficheros temporales. |
| GUID en nombres de archivo en Windows | UUID v4 usa caracteres válidos para nombres de fichero. |

---

## Rama y ejecución

- Rama de trabajo: **`feat/sddia-evolution-sync-norma`** (creación mediante skill **iniciar-rama**, no git directo en agente).
- Siguiente acción del proceso feature: **implementation** (`implementation.md`) desglosando touchpoints de código, luego **execution**.
