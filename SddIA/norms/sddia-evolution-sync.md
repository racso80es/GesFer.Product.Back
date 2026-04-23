# Norma — Sincronismo y trazabilidad SddIA (evolution)

**Ámbito:** cambios en artefactos bajo `./SddIA/` (normas, procesos, acciones, agentes, skills definición, tokens, etc.), salvo que la herramienta de validación excluya rutas puntuales (p. ej. detalles en `paths.sddiaEvolutionPath` según versión del binario).

**Fuente de rutas:** únicamente claves `paths.*` del contrato Cúmulo (`SddIA/agents/cumulo.paths.json`). No usar rutas literales en documentación normativa.

## 1. Obligación de registro

Toda **alta**, **baja** o **modificación** material bajo `./SddIA/` debe quedar reflejada en el protocolo evolution **en la misma intervención o en el mismo PR**:

1. Fichero de detalle `{id_cambio}.md` con frontmatter conforme a `paths.sddiaEvolutionContractFile` (contrato v1.1).
2. Entrada correspondiente en el índice `paths.sddiaEvolutionLogFile`.

**Identificador:** `id_cambio` = **UUID v4**; nombre de fichero = `{id_cambio}.md`.

**Tipología (`tipo_operacion`):**

| Valor | Uso mínimo |
| :--- | :--- |
| `alta` | Nuevo artefacto o entidad normativa. |
| `baja` | Eliminación o retirada documentada; incluir `rutas_eliminadas` y `commit_referencia_previo` cuando aplique. |
| `modificacion` | Cambio de contenido sin alta/baja estructural clara. |

## 2. Separación respecto a evolución de producto

- **`paths.sddiaEvolutionPath`** — protocolo de trazabilidad del **ecosistema SddIA** en este repositorio. (Uso EXCLUSIVO para modificaciones dentro de la carpeta `./SddIA/` que modifiquen normas, agentes o reglas SddIA). **NO USE ESTA RUTA BAJO NINGUNA CIRCUNSTANCIA PARA TAREAS O BUGS REGULARES DE PRODUCTO.**
- **`paths.evolutionPath`** / `docs/evolution/` — evolución de **producto** (features, cierres, bugs, kaizen). No son intercambiables. TODO registro de log general, tareas y correcciones (incluso aquellas sobre el log de bugs o tareas de gestión) debe registrarse AQUÍ, en `docs/evolution/EVOLUTION_LOG.md`. NUNCA modifique `SddIA/evolution/Evolution_log.md` salvo que esté modificando las propias reglas de la IA en la carpeta `SddIA`.

### Mecanismo de Validación Pre-Escritura (Principio de Independencia)
Antes de escribir en cualquier log de evolución, el agente **debe** hacerse la siguiente pregunta:
> *"¿Este cambio afecta mi capacidad de razonamiento/protocolos (SddIA) o afecta a una funcionalidad del usuario/sistema (Producto)?"*

Cualquier violación de esta frontera (es decir, registrar cambios de producto en la crónica de SddIA o viceversa) será tratada como **Ruido Degenerativo** (Filtro A), comprometiendo la integridad ontológica del sistema.

## 3. Implementación

- **Registro y validación:** solo mediante **binarios Rust** publicados en `paths.skillsRustPath`, copiados a la cápsula `paths.skillCapsules.sddia-evolution-register`, con contrato JSON de entrada/salida alineado a `SddIA/skills/skills-contract.md` (sin `.ps1` como entrega principal del registro).
- **Watcher local:** aviso ante cambios bajo `./SddIA/`; no sustituye el registro con metadatos completos.
- **CI:** validación en PRs que alteren `./SddIA/` (ver workflow de repositorio).

## 4. Cumplimiento en agentes e IDE

Los agentes y editores (Cursor, Jules, etc.) deben asumir esta norma como **innegociable**. La difusión en `.cursor/rules` es refuerzo; la **fuente canónica** es esta norma + contrato + CI.

## 5. Referencias

- Contrato: `SddIA/evolution/evolution_contract.md` (vista vía `paths.sddiaEvolutionContractFile`).
- Cúmulo: `SddIA/agents/cumulo.json` → `pathsContract`.
- Touchpoints IA: `SddIA/norms/touchpoints-ia.md`.
