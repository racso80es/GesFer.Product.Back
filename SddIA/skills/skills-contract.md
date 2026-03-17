---
constraints:
- Cumplimiento obligatorio de Karma2Token para trazabilidad y seguridad.
- skill-id en kebab-case.
- Rutas canónicas solo desde Cúmulo (paths.skillsDefinitionPath, paths.skillCapsules, paths.skillsIndexPath).
- Un skill sin spec.md (con frontmatter YAML) en su carpeta no se considera completo.
- Las acciones y agentes resuelven la definición en SddIA y la implementación vía Cúmulo.
consumers:
- paths.actionsPath
- SddIA/agents/*.json
- paths.processPath
contract_version: 1.1.0
default_implementation:
  delivery:
  - Cada skill con ejecutable reside en una cápsula paths.skillCapsules[skill-id]. Los ejecutables se construyen en paths.skillsRustPath (Cúmulo) y se copian a <cápsula>/bin/.
  - Launcher .bat en la cápsula invoca el .exe en bin/.
  - 'manifest.json, documentación .md obligatorios en la cápsula. Rutas canónicas: Cúmulo paths.skillCapsules.'
  language: rust
  rationale: Rendimiento, seguridad de memoria, portabilidad y distribución como binario. Las implementaciones por defecto de scripts de skills (y de tools) han de ser en Rust.
definition_artefacts:
- ext: .md
  format: frontmatter_yaml
  naming: spec.md
  path: paths.skillsDefinitionPath/<skill-id>/
  purpose: 'Especificación: frontmatter YAML (metadatos) + cuerpo Markdown. Campos: skill_id, name, description, implementation_path_ref. es-ES.'
description: 'Contrato que todo skill debe cumplir: definición en paths.skillsDefinitionPath/<skill-id>/, implementación en paths.skillCapsules[skill-id]. Implementación por defecto en Rust.'
implementation:
  format: Rust executable (.exe)
  location_pattern: scripts/skills/{skill-id}/bin/{nombre}.exe
  migration_note: Las skills existentes con .ps1 deben migrar a .exe. El fallback a .ps1 ha sido eliminado.
  prohibited_formats:
  - .ps1
  - .bat
  - .sh
  source_location: scripts/skills-rs/src/{nombre}.rs
  standard: Solo se deben generar ejecutables .exe compilados desde Rust. Los scripts PowerShell (.ps1) están prohibidos en nuevas implementaciones.
implementation_requirements:
- El ejecutable o script sugerido para la implementación de la skill debe ser desarrollado en Rust (localizado en paths.toolsRustPath (Cúmulo)).
required_artefacts_capsule:
- ext: .rs
  path: paths.skillsRustPath (Cúmulo)/src/bin
  purpose: Implementación por defecto en Rust.
- ext: binary
  path: <cápsula>/bin/<skill_bin>.exe
  purpose: Ejecutable Rust en la cápsula (copiado desde skills-rs/install.ps1). OBLIGATORIO.
- ext: .bat
  purpose: 'Launcher en cápsula: invoca binario en bin/.'
- ext: .json
  naming: manifest
  purpose: 'Manifest de la cápsula: skillId, components, contract_ref.'
- ext: .md
  purpose: Documentación en la cápsula. es-ES.
scope: SddIA/skills/
security_model:
  description: Todo skill debe ser invocado bajo el contexto de un Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato de skills (Cúmulo: paths.skillsPath / paths.skillCapsules)

**Alcance:** Todas las entidades en **paths.skillsPath** y en cada **paths.skillCapsules[&lt;skill-id&gt;]** (Cúmulo (SddIA/agents/cumulo.json) que actúen como skills con implementación ejecutable. Listado: **paths.skillsIndexPath** (índice en raíz de skills).

**Desacoplamiento definición / implementación:** La **definición** (qué hace el skill, contrato, entradas/salidas) está en **paths.skillsDefinitionPath**/&lt;skill-id&gt;/ (paths.skillsDefinitionPath/&lt;skill-id&gt;/) en formato spec.md y spec.json. La **implementación** (scripts, config, ejecutables) está en **paths.skillCapsules[&lt;skill-id&gt;]** (scripts). La raíz del path de implementación la indica Cúmulo; en la definición (spec.json) debe indicarse **implementation_path_ref** (ej. `paths.skillCapsules.&lt;skill-id&gt;`) para resolver la ruta desde Cúmulo sin duplicar rutas literales.

**Objetivo:** Unificar la estructura de skills con encapsulamiento por carpeta (igual que tools) y garantizar que el **ejecutable sugerido** sea en Rust cuando el skill tenga implementación invocable.

---

## 1. Definición por skill (paths.skillsDefinitionPath/&lt;skill-id&gt;/)

Cada skill debe tener en **paths.skillsDefinitionPath**/&lt;skill-id&gt;/:

- **spec.md** — Especificación legible: objetivo, entradas, salidas, flujo, reglas. Idioma: es-ES.
- **spec.json** — Metadatos machine-readable. Si el skill tiene implementación ejecutable, debe incluir **implementation_path_ref**: referencia a la ruta de implementación en Cúmulo (ej. `paths.skillCapsules.<skill-id>`).

---

## 2. Implementación por defecto: Rust

**Las implementaciones por defecto de los scripts de skills (y de las herramientas tools) han de ser en Rust.**

- **Motivo:** rendimiento, seguridad de memoria, portabilidad y distribución como binario único.
- **Entrega:** cada skill con ejecutable reside en una **cápsula** **paths.skillCapsules[&lt;skill-id&gt;]** (Cúmulo). Los ejecutables se construyen en paths.skillsRustPath (Cúmulo) y se copian a `&lt;cápsula&gt;/bin/`. Opcional: launcher `.bat` en la cápsula que delegue al binario en `bin/` o al script `.ps1` como fallback.
- **Launcher:** dentro de la cápsula, el `.bat` invoca el `.exe` en `bin/` si existe; en caso contrario, fallback al script `.ps1` de la cápsula.
- **manifest.json** (skillId, components, contract_ref), **documentación** (`.md`) son obligatorios en la cápsula. **Rutas canónicas:** Cúmulo `Cúmulo (SddIA/agents/cumulo.json)` → **paths.skillsPath**, **paths.skillCapsules**. En documentación .md no usar rutas literales; referenciar vía Cúmulo.

Referencia: mismo patrón que `paths.toolsDefinitionPath/tools-contract.md` (Implementación por defecto: Rust).

## 3. Artefactos por skill (con ejecutable)

Cada skill con implementación invocable reside en una **cápsula** **paths.skillCapsules[&lt;skill-id&gt;]** (Cúmulo) y debe contar con:

- **Implementación Rust:** código en `scripts/skills-rs/src/bin/&lt;skill_bin&gt;.rs`; binario final en `&lt;cápsula&gt;/bin/` (copiado tras `scripts/skills-rs/install.ps1`).
- **Fallback:** script `.ps1` en la cápsula cuando no exista o no se compile el binario Rust.
- **Launcher:** `.bat` en la cápsula que invoque el binario en `bin/` si existe, si no el `.ps1`.
- **manifest.json:** skillId, version, description, contract_ref, components (launcher_bat, launcher_ps1, doc, bin).
- **Documentación:** un `.md` en la cápsula que describa uso y parámetros. Idioma: es-ES.

---

## 4. Restricciones

- skill-id en kebab-case.
- Rutas canónicas solo desde Cúmulo (paths.skillsDefinitionPath, paths.skillCapsules, paths.skillsIndexPath).
- Skills sin implementación ejecutable: solo definición en paths.skillsDefinitionPath/&lt;skill-id&gt;/ (spec.md, spec.json).

---

## 5. Consumidores

El contrato permite que acciones, agentes y procesos (feature, bug-fix) consuman skills mediante rutas resueltas desde Cúmulo y launcher en la cápsula.

**Referencia machine-readable:** `paths.skillsDefinitionPath/skills-contract.json`.
