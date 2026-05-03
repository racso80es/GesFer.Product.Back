---
document_id: reproducir-create-skill-en-otros-entornos-sddia
document_type: guia_portabilidad
norm_ref: SddIA/norms/paths-via-cumulo.md
process_ref: paths.processPath/create-skill/
analog_process_ref: paths.processPath/create-tool/
contract_ref: paths.processPath/process-contract.md
skills_contract_ref: SddIA/skills/skills-contract.md
evolution_ref: SddIA/norms/sddia-evolution-sync.md
cumulo_ref: SddIA/agents/cumulo.json
paths_contract_ref: SddIA/agents/cumulo.paths.json
skills_entities_portability_ref: SddIA/skills/reproducir-skills-en-otros-entornos-sddia.md
idioma: es-ES
version: "1.0.0"
descripcion: >-
  Guía para reproducir el proceso de tarea create-skill (definición en paths.processPath,
  listados y difusión) en otro repositorio o entorno SddIA.
nota_entorno_gesfer: >-
  En GesFer.Product.Back el proceso create-skill, su carpeta bajo paths.processPath,
  el README de procesos, interaction-triggers (#Process), AGENTS.md (tabla de procesos y rol CREATE-SKILL),
  AGENTS.norms.md (#Process) y la difusión Cursor ya están alineados.
  Este documento sirve como SSOT de portabilidad hacia otros repos; no implica duplicar aquí.
---

# Reproducir el proceso `create-skill` en otros entornos SddIA

Esta guía describe **qué artefactos copiar, qué índices actualizar y cómo validar** para que el proceso **`create-skill`** exista y sea descubrible en **otro repo** que siga el mismo ecosistema SddIA. Las rutas lógicas son siempre las del **Cúmulo** (`SddIA/agents/cumulo.json` → `pathsContract` → `SddIA/agents/cumulo.paths.json`); no documentar rutas físicas como SSOT sin anclarlas a claves `paths.*`.

**Alcance de este documento:** portar la **definición del proceso** (`paths.processPath/create-skill/`) y su **visibilidad** (README de procesos, disparador `#Process`, difusión Cursor si aplica). **No** incluye portar skills concretas; para eso usar `SddIA/skills/reproducir-skills-en-otros-entornos-sddia.md`.

## 1. Prerrequisitos del entorno destino

| Requisito | Uso |
|-----------|-----|
| **Cúmulo** presente | `cumulo.json` + `cumulo.paths.json` con `paths.processPath`, `paths.featurePath`, `paths.skillsDefinitionPath`, `paths.skillCapsules`, `paths.skillsIndexPath`, `paths.skillsRustPath` (y, según contrato de skills, `paths.skillsPath` si el destino separa definición e implementación como aquí). |
| **Contrato de procesos** | `paths.processPath/process-contract.md` (y `spec.json` por proceso si el destino lo exige). |
| **Windows 11 + PowerShell 7+** | Convención del proyecto GesFer; alinear si el destino es distinto. |
| **Evolution SddIA** (si aplica) | Cualquier alta/modificación bajo `./SddIA/` según `SddIA/norms/sddia-evolution-sync.md`. |

## 2. Analogía con `create-tool`

El proceso **`create-skill`** está modelado como **espejo** de **`create-tool`**, sustituyendo:

| Concepto | create-tool | create-skill |
|----------|-------------|--------------|
| Rama de tarea | `feat/create-tool-<tool-id>` | `feat/create-skill-<skill-id>` |
| Persistencia tarea | `paths.featurePath/create-tool-<tool-id>` | `paths.featurePath/create-skill-<skill-id>` |
| Definición entidad | `paths.toolsDefinitionPath` | `paths.skillsDefinitionPath` |
| Cápsula | `paths.toolCapsules` | `paths.skillCapsules` |
| Índice | `paths.toolsIndexPath` | `paths.skillsIndexPath` |
| Código Rust | `paths.toolsRustPath` | `paths.skillsRustPath` |

Referencia de plantilla: `paths.processPath/create-tool/spec.md`.

## 3. Checklist: definición del proceso en `paths.processPath`

1. **Carpeta del proceso**  
   Crear `paths.processPath/create-skill/` con al menos:
   - **`spec.md`** — frontmatter YAML + cuerpo Markdown (`process_id: create-skill`, entradas, rutas vía refs `paths.*`, fases, restricciones).
   - **`spec.json`** — metadatos machine-readable (`process_id`, `persist_ref`, `phases`, `outputs`, etc.) alineado con el cuerpo de `spec.md`.

2. **Coherencia con contrato de skills**  
   En `spec.md`, la cápsula ejecutable debe describirse según **`SddIA/skills/skills-contract.md`**: ejecutable **`<cápsula>/bin/<nombre>.exe`**, launcher **`.bat`** en la raíz de la cápsula que invoque el binario en `bin/`, más `manifest.json` y documentación en la cápsula. El envelope JSON de agentes sigue `SddIA/norms/capsule-json-io.md`.  
   **No** alinear con variantes que coloquen el `.exe` solo en la raíz de la cápsula sin `bin/` si el contrato del destino exige la estructura estándar anterior.

## 4. Checklist: visibilidad y consumo por agentes / IDE

| Artefacto | Acción |
|-----------|--------|
| `paths.processPath/README.md` | Añadir fila **create-skill** en la tabla de procesos y entrada numerada en la sección **Uso** (incluir rama, `paths.featurePath/create-skill-<skill-id>` y entregable vía `paths.skillCapsules` / `paths.skillsPath` según el README local). |
| `SddIA/norms/interaction-triggers.md` | En el listado del disparador **`#Process`**, añadir fila `create-skill` con descripción y `paths.processPath/create-skill/`. |
| `.cursor/rules/process-suggestions.mdc` | Si el destino usa Cursor, añadir la misma fila en la tabla del disparador `#Process` (difusión; no sustituye la norma). |
| `AGENTS.md` (opcional) | Si el destino lista procesos en el protocolo maestro, añadir **`create-skill`** en la tabla de procesos para no contradecir `paths.processPath`. |

## 5. Evolution SddIA (obligatorio si alteras `./SddIA/`)

En la misma intervención o PR:

1. Fichero de detalle `paths.sddiaEvolutionPath/{uuid}.md` con frontmatter según `paths.sddiaEvolutionContractFile`.
2. Entrada en `paths.sddiaEvolutionLogFile` (índice maestro).

**Tipología:** normalmente `alta` al introducir el proceso por primera vez; `modificacion` si solo se ajusta texto o checklist.

## 6. Documentación de la feature de portado (recomendado)

Si el portado se hace como tarea trazable, usar el proceso **`feature`** y persistir en `paths.featurePath/<nombre_feature>/` con el patrón **un `.md` por acción** y frontmatter (`objectives.md`, `clarify.md`, `plan.md`, …) según `SddIA/norms/features-documentation-pattern.md` y `docs/features/features-contract.md`.

Nombre sugerido de feature: p. ej. `create-process-create-skill` o `port-process-create-skill-<repo-slug>`.

## 7. Verificación mínima en el destino

1. Existe `paths.processPath/create-skill/spec.md` y `spec.json` (si el destino exige JSON por proceso).  
2. `paths.processPath/README.md` menciona **create-skill** y el enlace relativo `./create-skill/` resuelve.  
3. `SddIA/norms/interaction-triggers.md` lista **create-skill** bajo `#Process`.  
4. Búsqueda global: no quedan referencias rotas a `create-skill` sin carpeta destino.  
5. Si aplica evolution: índice + detalle UUID presentes y coherentes.

## 8. Referencias cruzadas

- Proceso canónico: `paths.processPath/create-skill/spec.md`  
- Proceso análogo: `paths.processPath/create-tool/spec.md`  
- Portar skills ejecutables: `SddIA/skills/reproducir-skills-en-otros-entornos-sddia.md`  
- Rutas: `SddIA/norms/paths-via-cumulo.md`  
- E/S JSON agentes: `SddIA/norms/capsule-json-io.md`

---

*Guía de portabilidad del proceso. Mantener alineada con Cúmulo, process-contract y skills-contract al evolucionar.*
