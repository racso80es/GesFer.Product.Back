---
document_type: guia_replicacion
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
audience: otro_repositorio_o_entorno_SddIA
source_spec: docs/features/sddia-evolution-sync-norma/spec.md
source_plan: docs/features/sddia-evolution-sync-norma/plan.md
source_implementation: docs/features/sddia-evolution-sync-norma/implementation.md
contract_ref: SddIA/evolution/evolution_contract.md
paths_ref: SddIA/agents/cumulo.paths.json
schema_version: "1.0"
---

# Guía de replicación — Norma de sincronismo SddIA (evolution)

Este documento concentra **todo lo necesario** para reproducir en **otro entorno o repositorio SddIA** los mismos cambios funcionales: rutas SSOT, contrato YAML, artefactos a crear o modificar, automatización Rust, CI y difusión IDE. Las rutas de disco se dan respecto a la **raíz del repo**; en documentación normativa debe usar **solo** claves `paths.*` del Cúmulo.

**Referencias canónicas en este repo:** `spec.md` (1.2-plan), `plan.md`, `clarify.md`, `implementation.md`.

---

## 1. Principios que debes copiar

| Principio | Descripción |
| :--- | :--- |
| **SSOT rutas** | Toda ruta de persistencia se define en `SddIA/agents/cumulo.paths.json` (contrato `cumulo.paths.json` referenciado por `SddIA/agents/cumulo.json`). No duplicar reglas en gestores sin pasar por `SddIA` (norma `paths-via-cumulo.md`). |
| **Doble registro** | Índice maestro + un fichero detalle por cambio (GUID como nombre). |
| **Sin PS1 como entrega** | Registro y validación vía **binarios Rust** + envelope JSON (`SddIA/norms/capsule-json-io.md`). |
| **Separación** | `paths.sddiaEvolutionPath` (protocolo SddIA) ≠ `paths.evolutionPath` (evolución de producto en `docs/evolution/`). |
| **Difusión Cursor** | `.cursor/rules/*.mdc` refuerza cumplimiento en el IDE; **no** sustituye norma ni CI; mantener vía **sddia-difusion** cuando cambie SddIA. |

---

## 2. Claves obligatorias en Cúmulo (`paths`)

Añadir o fusionar en el objeto `paths` de `cumulo.paths.json`:

| Clave | Valor recomendado |
| :--- | :--- |
| `sddiaEvolutionPath` | `./SddIA/evolution/` |
| `sddiaEvolutionLogFile` | `Evolution_log.md` |
| `sddiaEvolutionContractFile` | `evolution_contract.md` |

Documentar estas claves en `SddIA/norms/paths-via-cumulo.md` (sección evolution SddIA).

**Opcional (tras implementar skills):** `skillCapsules.sddia-evolution-register` → `./scripts/skills/sddia-evolution/` (u otra cápsula acordada).

---

## 3. Carpeta de persistencia del protocolo

| Rol | Ruta física (ajustar si tu repo usa otro nombre de carpeta SddIA) |
| :--- | :--- |
| Índice | `{sddiaEvolutionPath}/{sddiaEvolutionLogFile}` → `./SddIA/evolution/Evolution_log.md` |
| Contrato | `{sddiaEvolutionPath}/{sddiaEvolutionContractFile}` → `./SddIA/evolution/evolution_contract.md` |
| Detalle atómico | `{sddiaEvolutionPath}/{id_cambio}.md` con `id_cambio` = **UUID v4** (nombre de fichero = GUID + `.md`) |

Crear la carpeta `./SddIA/evolution/` si no existe.

---

## 4. Contrato YAML v1.1 (resumen para portar)

Debe coincidir semánticamente con `evolution_contract.md` (versión **1.1**).

### Identificador

- **`id_cambio`:** UUID v4 en string canónico (guiones, minúsculas).
- **Nombre de fichero:** `{id_cambio}.md`.
- El formato histórico `SSDD-LOG-YYYYMMDD-HHMM` **no** es el id machine-readable principal.

### Campos obligatorios (frontmatter del detalle)

- `contrato_version`: `1.1`
- `id_cambio`, `fecha`, `autor`, `proyecto_origen_cambio`, `contexto`, `descripcion_breve`
- `tipo_operacion`: `alta` | `baja` | `modificacion`
- `cambios_realizados` (lista `anterior` / `nuevo`)
- `impacto`: `Bajo` | `Medio` | `Alto`
- `replicacion.instrucciones`, `replicacion.hash_integridad` (SHA-256 del YAML canónico o `SHA-256-PENDIENTE`)

### Campos condicionales (baja)

- `rutas_eliminadas` (lista de strings)
- `commit_referencia_previo` (SHA u objeto de trazabilidad)

### Índice `Evolution_log.md`

Tabla recomendada: **ID (GUID)** | **Fecha** | **Descripción breve**. Tras el primer registro oficial, eliminar filas placeholder “pendiente”.

---

## 5. Normas y constitución

| Artefacto | Acción en el entorno destino |
| :--- | :--- |
| `SddIA/norms/sddia-evolution-sync.md` | Crear o fusionar: ámbito `./SddIA/`, obligación de registro, GUID, watcher local + validación CI, sin `.ps1` como implementación del registro. |
| `SddIA/CONSTITUTION.md` | Añadir sección: trazabilidad del protocolo y remisión a `paths.sddiaEvolutionPath`. |
| `SddIA/constitution.json` | Añadir ley universal o entrada equivalente (p. ej. trazabilidad SddIA evolution con GUID). |

---

## 6. Automatización Rust (`scripts/skills-rs`)

Replicar en el crate de skills (o equivalente) **tres binarios**:

| Binario | Función mínima |
| :--- | :--- |
| `sddia_evolution_register` | JSON capsule-io: genera UUID, escribe `{uuid}.md`, actualiza `Evolution_log.md`, calcula `hash_integridad`. |
| `sddia_evolution_validate` | Compara diff git `base..head`; si hay cambios bajo `SddIA/` (reglas excluyendo evolution según diseño), exige coherencia con índice/detalle; exit ≠ 0 si falla (para CI). |
| `sddia_evolution_watch` | `notify` + debounce sobre `./SddIA/`; aviso para ejecutar register (sin obligar metadatos automáticos en v1). |

**Dependencias típicas:** `uuid`, `notify`, `serde`, `serde_json`, crate interno `gesfer-capsule` (o el equivalente del otro repo).

**Instalación:** actualizar `install.ps1` del crate para copiar `.exe` a la cápsula bajo `scripts/skills/sddia-evolution/` (o ruta que registres en Cúmulo).

**Definición de skill:** `SddIA/skills/sddia-evolution-register/spec.md` + `manifest.json` en la cápsula.

---

## 7. GitHub / CI / Jules

| Artefacto | Acción |
| :--- | :--- |
| `.github/workflows/pr-validation.yml` (o equivalente) | Job condicional: si el PR toca `SddIA/**`, compilar y ejecutar `sddia_evolution_validate` con base `origin/<troncal>` y `HEAD`. |
| `.github/README.md` | Sección que enlace a la norma y explique cumplimiento para PRs que alteren SddIA (Jules / agentes). |
| `.github/PULL_REQUEST_TEMPLATE.md` | Checkbox: cambios en `./SddIA/` requieren registro evolution (GUID + índice). |

Alinear con `SddIA/norms/touchpoints-ia.md` y, si existe, acción `sddia-difusion`.

---

## 8. Difusión Cursor / IDE

Objetivo: **refuerzo de cumplimiento** para quien trabaja en Cursor (sin sustituir CI ni la norma canónica).

| Artefacto | Acción |
| :--- | :--- |
| `.cursor/rules/sddia-evolution-sync.mdc` | Crear regla **corta**: enlaces a `SddIA/norms/sddia-evolution-sync.md`, `SddIA/evolution/evolution_contract.md`, Cúmulo/`paths-via-cumulo`; checklist mínimo (índice + detalle GUID, `tipo_operacion`, binario Rust, bajas); frase de **prevalencia de SddIA** si hay contradicción; **no** pegar el esquema YAML completo (evitar deriva). |
| `globs` (recomendado) | `SddIA/**/*` para cargar la regla al editar bajo `./SddIA/` sin `alwaysApply` global innecesario. |
| **sddia-difusion** | Cualquier cambio futuro en la norma o contrato debe actualizar este `.mdc` en el mismo cambio o PR de difusión. |

**Límite:** la regla Cursor **no bloquea** git; el cumplimiento fuerte sigue en **CI** y revisión.

---

## 9. Orden de aplicación recomendado (checklist)

1. Añadir claves `sddia*` en `cumulo.paths.json` y documentar en `paths-via-cumulo.md`.
2. Crear `./SddIA/evolution/` con `evolution_contract.md` (v1.1) y `Evolution_log.md` (tabla inicial o placeholder hasta primer register).
3. Crear/actualizar `SddIA/norms/sddia-evolution-sync.md`.
4. Actualizar `CONSTITUTION.md` y `constitution.json`.
5. Implementar binarios Rust + cápsula + entrada `skillCapsules` en Cúmulo.
6. Ejecutar **primer** `register` para sustituir placeholder del índice por entrada real (GUID).
7. Añadir watcher y pasos CI.
8. Añadir regla `.cursor` y actualizar `.github`.

---

## 10. Documentación de feature (opcional en el repo destino)

Si el otro entorno usa el mismo proceso **feature**, replicar la carpeta:

`docs/features/sddia-evolution-sync-norma/` con al menos `objectives.md`, `spec.md`, `plan.md`, `clarify.md`, `implementation.md`, y este fichero `replicacion-entorno-sddia.md`.

---

## 11. Verificación rápida post-replicación

- [ ] `paths.sddiaEvolutionPath` resuelve a una carpeta existente con `Evolution_log.md` y `evolution_contract.md`.
- [ ] Un registro de prueba genera `{uuid}.md` y una fila nueva en el índice.
- [ ] `sddia_evolution_validate` pasa en local con un PR simulado que solo toca SddIA con registro completo.
- [ ] CI falla si se simula diff en `SddIA/` sin trazabilidad (según reglas implementadas).
- [ ] Existe `.cursor/rules/sddia-evolution-sync.mdc` alineado con la norma; enlaces del `.mdc` resuelven bien en el repo destino.

---

*Fin de la guía. Para el texto completo del contrato y ejemplos YAML, copiar o alinear `SddIA/evolution/evolution_contract.md` del repositorio origen.*
