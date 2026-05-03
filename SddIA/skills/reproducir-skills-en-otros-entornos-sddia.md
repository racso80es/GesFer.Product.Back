---
document_id: reproducir-skills-en-otros-entornos-sddia
document_type: guia_portabilidad
norm_ref: SddIA/norms/paths-via-cumulo.md
process_ref: paths.processPath/create-skill/
contract_ref: SddIA/skills/skills-contract.md
capsule_io_ref: SddIA/norms/capsule-json-io.md
evolution_ref: SddIA/norms/sddia-evolution-sync.md
cumulo_ref: SddIA/agents/cumulo.json
paths_contract_ref: SddIA/agents/cumulo.paths.json
related_process_portability_ref: SddIA/skills/reproducir-create-skill-en-otros-entornos-sddia.md
idioma: es-ES
version: "1.0.0"
descripcion: >-
  Guía para portar una o más skills concretas (definición en paths.skillsDefinitionPath,
  cápsula paths.skillCapsules, índice y código Rust) a otro repositorio SddIA.
nota_entorno_gesfer: >-
  En GesFer.Product.Back las rutas físicas de definición e implementación siguen Cúmulo
  (paths.skillsDefinitionPath, paths.skillsPath, paths.skillCapsules). Esta guía usa solo claves paths.*;
  el destino debe resolver su cumulo.paths.json equivalente.
---

# Reproducir skills ejecutables en otros entornos SddIA

Esta guía describe **qué copiar y qué índices tocar** para que **skills ya existentes** funcionen en **otro repo** del ecosistema. Las rutas lógicas son siempre las del **Cúmulo** (`SddIA/agents/cumulo.json` → `pathsContract` → `SddIA/agents/cumulo.paths.json`).

**Alcance:** portar **definiciones y cápsulas** de skills. Para dar de alta en destino solo el **proceso de tarea** `create-skill` (carpeta bajo `paths.processPath`, README, `#Process`, difusión), usar `SddIA/skills/reproducir-create-skill-en-otros-entornos-sddia.md`.

## 1. Prerrequisitos del entorno destino

| Requisito | Uso |
|-----------|-----|
| **Cúmulo** | `cumulo.json` + `cumulo.paths.json` con `paths.skillsDefinitionPath`, `paths.skillsPath`, `paths.skillCapsules`, `paths.skillsIndexPath`, `paths.skillsRustPath`. |
| **Contrato de skills** | `SddIA/skills/skills-contract.md` — definición (`spec.md` / `spec.json`), cápsula con `manifest.json`, launcher `.bat`, binario en `<cápsula>/bin/*.exe`. |
| **Karma2Token** | Si el destino exige token en invocaciones, alinear con `paths.tokensPath` y spec del token. |
| **Evolution SddIA** | Cualquier alta bajo `./SddIA/` según `SddIA/norms/sddia-evolution-sync.md`. |

## 2. Checklist por cada `skill-id`

1. **Definición**  
   Copiar `paths.skillsDefinitionPath/<skill-id>/` completa (`spec.md`, `spec.json` si existe).  
   Comprobar que `implementation_path_ref` (u equivalente) apunte a una clave **`paths.skillCapsules.<skill-id>`** que existirá en el Cúmulo destino.

2. **Cápsula de implementación**  
   Copiar el árbol resuelto por **`paths.skillCapsules[<skill-id>]`** en origen (típicamente bajo `paths.skillsPath/<skill-id>/`): `manifest.json`, documentación `.md`, launcher `.bat`, `bin/<nombre>.exe`, y cualquier recurso adicional que el manifest liste.

3. **Cúmulo (`cumulo.paths.json`)**  
   Añadir la entrada **`skillCapsules.<skill-id>`** con el path relativo correcto en el repo destino (no copiar paths absolutos ni asumir el mismo slug de carpeta sin verificar).

4. **Índice de skills**  
   Actualizar **`paths.skillsIndexPath`** según el formato del destino (en GesFer: `scripts/skills/index.json`) para incluir el `skill_id`, rutas y metadatos que el proyecto espere.

5. **Fuente Rust (opcional pero recomendable)**  
   Si el destino compila desde monorepo: copiar o sincronizar el crate bajo **`paths.skillsRustPath`** y documentar cómo ejecutar `install.ps1` o el build que copie el `.exe` a la cápsula. Si solo se porta el binario, documentar versión y procedencia.

6. **Visibilidad para agentes / IDE**  
   Si la skill debe aparecer en disparadores: actualizar `SddIA/norms/interaction-triggers.md` (tabla `#Skill`), y si aplica **`.cursor/rules/skill-suggestions.mdc`** en destino.

## 3. Coherencia normativa

- **Ejecutable:** según `skills-contract.md`, el binario entregable está en **`<cápsula>/bin/<nombre>.exe`**; el **`.bat`** en la raíz de la cápsula invoca ese binario (no sustituir por invocar `.ps1` como entrega principal en nuevas incorporaciones).
- **E/S JSON:** envelope y esquemas según `SddIA/norms/capsule-json-io.md` si la skill participa en el flujo de cápsulas.
- **Comandos:** en entornos GesFer, la ley de comandos (skill / tool / proceso) sigue vigente; no documentar atajos que contradigan `SddIA/norms/commands-via-skills-or-tools.md`.

## 4. Evolution SddIA (obligatorio si alteras `./SddIA/`)

En la misma intervención o PR en destino:

1. Fichero `paths.sddiaEvolutionPath/{uuid}.md` con frontmatter según `paths.sddiaEvolutionContractFile`.
2. Entrada en `paths.sddiaEvolutionLogFile`.

**Tipología:** `alta` al incorporar nuevas definiciones/carpetas; `modificacion` si solo se ajustan índices o textos.

## 5. Verificación mínima en el destino

1. Existe `paths.skillsDefinitionPath/<skill-id>/spec.md` (y `spec.json` si el destino lo exige).  
2. Resuelve `paths.skillCapsules[<skill-id>]` y el launcher ejecuta el `.exe` esperado.  
3. `paths.skillsIndexPath` incluye la skill sin duplicados rotos.  
4. Búsqueda global: no hay referencias a `skill-id` o paths de origen que no existan en destino.  
5. Si aplica evolution: índice + detalle UUID presentes.

## 6. Referencias cruzadas

- Portar el **proceso** `create-skill`: `SddIA/skills/reproducir-create-skill-en-otros-entornos-sddia.md`  
- Contrato: `SddIA/skills/skills-contract.md`  
- Rutas: `SddIA/norms/paths-via-cumulo.md`  
- Proceso canónico de alta de una skill: `paths.processPath/create-skill/spec.md`

---

*Guía de portabilidad de skills. Mantener alineada con Cúmulo y skills-contract al evolucionar.*
