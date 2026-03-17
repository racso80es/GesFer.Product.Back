# Puntos de interacción con la IA (touchpoints)

**Objetivo:** Mantener los distintos gestores de IA (Cursor, Jules, etc.) y artefactos (.cursor/rules, .github) alineados con las normas definidas en **SddIA** (AGENTS.md, SddIA/norms/). SddIA es la única fuente de verdad; los touchpoints son **difusión** de esas normas.

Acción que orquesta la revisión y actualización: **paths.actionsPath/sddia-difusion/ (SddIA/actions/sddia-difusion/)**.

---

## Touchpoints actuales

| Touchpoint | Ubicación | Descripción | Cómo mantener |
|------------|-----------|-------------|----------------|
| **Cursor** | `.cursor/rules/*.mdc` | Reglas que Cursor aplica al asistente (disparadores #Skill, #Action, #Process, subir; SSOT). | Al cambiar procesos, acciones o listados en SddIA, ejecutar la acción sddia-difusion y actualizar los .mdc. La regla `sddia-ssot.mdc` declara que SddIA prevalece. |
| **AGENTS.md** | Raíz del repo | Protocolo maestro del sistema multiagente. Referenciado por Cursor y por documentación. | Modificar solo según acuerdo con SddIA; no duplicar lógica en .cursor que contradiga AGENTS.md. |
| **AGENTS.norms.md** | Raíz del repo | Tabla de disparadores; enlaza a SddIA/norms/interaction-triggers.md. | Mantener sincronizado con interaction-triggers.md cuando se añadan disparadores (ej. #Tool). |
| **.github** | `.github/` | PR template, README (difusión). Workflows e issue templates si se añaden. | No duplicar normas; referenciar AGENTS.md y SddIA. PR template pide rama feat/fix, proceso y doc de tarea. Revisar con acción sddia-difusion. |

## Touchpoints futuros o opcionales

| Touchpoint | Ubicación | Descripción | Cómo mantener |
|------------|-----------|-------------|----------------|
| **Jules** | (configuración específica del gestor) | Si el proyecto usa Jules u otro asistente: prompt de sistema, reglas, contexto. | Documentar aquí o en docs/ dónde se configura Jules y cómo se inyectan las normas (p. ej. copia de AGENTS.md, o referencia a SddIA). Objetivo: mismo comportamiento que en Cursor (leyes, procesos, Cúmulo). |
| **Otros IDEs/gestores** | Variable | Cualquier otro cliente que use el repo para asistencia IA. | Misma regla: SddIA es SSOT; documentar en esta tabla el touchpoint y el procedimiento de difusión. |

---

## Principios

1. **Una sola fuente de verdad:** Las normas de comportamiento están en SddIA (y AGENTS.md); los touchpoints las difunden, no las reemplazan.
2. **Rutas vía Cúmulo:** En reglas y documentación de comportamiento no usar rutas literales; referenciar paths.featurePath, paths.actionsPath, paths.skillCapsules, etc. (SddIA/norms/paths-via-cumulo.md).
3. **Revisión al cambiar SddIA:** Si se añade un proceso (p. ej. refactorization), una acción (p. ej. sddia-difusion) o un disparador, actualizar los touchpoints afectados (acción sddia-difusion).

---
*Referencia: paths.actionsPath/sddia-difusion/ (SddIA/actions/sddia-difusion/). Actualizar esta tabla al añadir o quitar gestores.*
