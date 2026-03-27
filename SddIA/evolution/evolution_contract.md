---
document_type: evolution_contract
contrato_version: "1.1"
norma_ref: SddIA/norms/sddia-evolution-sync.md
paths_ref: SddIA/agents/cumulo.paths.json
---

# Contrato de cambio — protocolo SddIA (evolution)

Versión **1.1**. Toda intervención que altere artefactos bajo `./SddIA/` (salvo exclusiones acordadas en la norma) debe registrarse con **doble persistencia**: índice maestro (`paths.sddiaEvolutionLogFile`) y fichero de detalle `{id_cambio}.md` donde `id_cambio` es un **UUID v4** en forma canónica (minúsculas, con guiones).

## Identificador

- **`id_cambio`:** UUID v4 (string).
- **Nombre de fichero:** `{id_cambio}.md` en `paths.sddiaEvolutionPath`.
- El formato legible tipo `SSDD-LOG-YYYYMMDD-HHMM` no sustituye al GUID como id machine-readable principal.

## Frontmatter obligatorio (detalle `{id_cambio}.md`)

| Campo | Tipo | Notas |
| :--- | :--- | :--- |
| `contrato_version` | string | Debe ser `1.1`. |
| `id_cambio` | string | UUID v4. |
| `fecha` | string | ISO 8601 (fecha u offset UTC). |
| `autor` | string | Identidad responsable del registro. |
| `proyecto_origen_cambio` | string | Repositorio o contexto. |
| `contexto` | string | Motivación breve. |
| `descripcion_breve` | string | Una línea para índice. |
| `tipo_operacion` | string | `alta` \| `baja` \| `modificacion`. |
| `cambios_realizados` | lista | Objetos `{ anterior, nuevo }` (en **baja**, documentar path retirado y sustituto o histórico). |
| `impacto` | string | `Bajo` \| `Medio` \| `Alto`. |
| `replicacion.instrucciones` | string | Pasos para otro entorno. |
| `replicacion.hash_integrity` | string | SHA-256 en hex minúsculas del YAML canónico del frontmatter, o `SHA-256-PENDIENTE`. |

## Frontmatter condicional (baja)

- `rutas_eliminadas`: lista de strings (paths relativos a la raíz del repo).
- `commit_referencia_previo`: SHA corto, URL de commit o referencia de trazabilidad donde el artefacto aún existía.

## Hash de integridad

El campo `replicacion.hash_integrity` debe ser el **SHA-256** (hex minúsculas, 64 caracteres) del bloque YAML **canónico** del frontmatter, serializado con claves en orden estable (p. ej. orden lexicográfico de claves de primer nivel) y sin línea final extra; o el literal `SHA-256-PENDIENTE` hasta calcularlo (solo transitorio).

## Índice `Evolution_log.md`

Tabla recomendada: **ID (GUID)** | **Fecha** | **Descripción breve**. Tras el primer registro oficial, eliminar filas placeholder «pendiente».
