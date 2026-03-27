---
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
purpose: Cerrar ambigüedades del SPEC antes de planning; incorporar decisiones del usuario (watcher, tipos de cambio, Rust vs PS, .github/Jules).
decisions:
  - id: CL-001
    topic: Watcher automático
    decision: Incluido en el alcance actual (v1). Debe observar cambios bajo ./SddIA/ y disparar el flujo de registro (o alerta) según norma.
  - id: CL-002
    topic: Tipología de cambio
    decision: Toda intervención se clasifica en contexto amplio — alta (nuevo), baja (eliminación), modificación — y el contrato YAML v1.0 debe contemplarlo explícitamente.
  - id: CL-003
    topic: Implementación del registrador / watcher
    decision: No usar fichero .ps1. Implementación mediante binario Rust en cápsula de tool o skill, con contrato específico (stdin/stdout JSON según SddIA/norms/capsule-json-io.md) y manifest en paths.toolCapsules o paths.skillCapsules según diseño en planning.
  - id: CL-004
    topic: Cumplimiento e ID Jules
    decision: Los cambios necesarios en ./github (workflows, README, plantillas) forman parte del entregable para alinear CI y difusión con el protocolo SddIA y el cumplimiento cuando el actor es Jules (u otro agente); sin contradicción con AGENTS/SddIA.
spec_ref: docs/features/sddia-evolution-sync-norma/spec.md
status: cerrada_planning_2026-03-27
---

# Clarificación — Norma de Sincronismo SddIA (evolution)

Documento de **clarify** tras revisión del SPEC. Las decisiones **CL-001…CL-004** están reflejadas en `spec.md` (versión actualizada).

---

## 1. Decisiones cerradas (usuario)

### 1.1 Watcher automático (CL-001)

- **Antes en SPEC:** Watcher explícitamente fuera de v1.
- **Ahora:** El **watcher automático** pasa a **alcance actual**. Implica diseño técnico en planning: proceso de fondo o integración con el flujo de trabajo que detecte cambios en `./SddIA/` y exija o genere el registro de evolución sin depender solo de memoria humana.

### 1.2 Tipos de cambio — contexto amplio (CL-002)

No basta con “hubo modificación”. El contrato y la norma deben distinguir:

| Tipo | Significado mínimo |
| :--- | :--- |
| **Alta** | Creación de artefacto normativo nuevo (fichero o entidad). |
| **Baja** | Eliminación o retirada documentada (ruta deja de existir o se depreca explícitamente). |
| **Modificación** | Cambio de contenido sin alta/baja estructural clara. |

Esta tipología alimenta `cambios_realizados` y/o un campo dedicado (`tipo_operacion` o equivalente) definido en planning y en `evolution_contract.md`.

### 1.3 Sin PowerShell como entrega; Rust + contrato (CL-003)

- **Rechazado como solución principal:** `*.ps1` para registro automático.
- **Aprobado:** Ejecutable **Rust** con **contrato de cápsula** (meta/request y success/result según capsule-json-io), ubicación coherente con `paths.skillsRustPath` / `paths.toolsRustPath` y registro en Cúmulo (`skillCapsules` o `toolCapsules` según se clasifique la pieza: watcher+registro puede ser **tool** o **skill**; se decide en planning).

### 1.4 `.github` y cumplimiento ID Jules (CL-004)

- **Incluido:** Revisión y actualización de **`.github/`** para que el pipeline de PR y la documentación de difusión refuercen el cumplimiento del registro SddIA evolution cuando el diff afecte a `./SddIA/`, en línea con:
  - `SddIA/norms/touchpoints-ia.md` (Jules como gestor),
  - `SddIA/actions/sddia-difusion/` (coherencia .github ↔ SddIA),
  - y el protocolo de PR existente (`pr-validation.yml`, `verify_pr_protocol`).

Detalle concreto (pasos de workflow, nombres de job) queda para **plan.md**; el criterio de aceptación es: *un PR que toque `./SddIA/` sin trazabilidad requerida falla en CI o queda bloqueado según diseño acordado*.

---

## 2. Respuestas usuario → incorporadas en `plan.md`

| # | Pregunta | Respuesta |
| :---: | :--- | :--- |
| 1 | Watcher local vs CI | **Ambos** (watcher/validación local + validación en CI). |
| 2 | Bajas | **OK:** path eliminado + referencia a commit previo en el detalle. |
| 3 | Colisión de IDs | **`id_cambio` en YAML = GUID (UUID v4)**; nombre fichero `{id_cambio}.md`. Sin colisión por minuto. |
| 4 | Bootstrap | **Regenerar** con el primer binario: no conservar `SSDD-LOG-20260327-1430` como oficial. |

---

## 3. Relación con el SPEC

- Tras planning: actualizar `spec.md` a **1.2-plan** (ID = GUID, índice, bootstrap).
- Ejecución técnica: **`plan.md`**.

---

## 4. Validación con el usuario (histórico)

Decisiones **CL-001 … CL-004** y respuestas §2 cerradas; planning en curso en `plan.md`.
