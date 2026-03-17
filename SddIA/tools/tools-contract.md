---
constraints:
- Cumplimiento obligatorio de Karma2Token para trazabilidad y seguridad.
- toolId en kebab-case.
- El JSON de salida debe ser válido y codificado en UTF-8.
- No escribir feedback sensible (contraseñas, tokens) en message ni en data.
- exitCode 0 únicamente cuando success es true; en caso de fallo, success false y exitCode != 0.
consumers:
- paths.actionsPath
- SddIA/agents/*.json
- paths.toolsPath
- CI/CD
contract_version: 1.1.0
default_implementation:
  delivery:
  - Cada herramienta reside en una capsula paths.toolsPath (Cúmulo) /<tool-id>/ con manifest.json. Los ejecutables se construyen en paths.toolsRustPath (Cúmulo) y se copian a paths.toolsPath (Cúmulo) /<tool-id>/bin/.
  - Launcher .bat en la capsula invoca el .exe.
  - 'Config (.json), documentacion (.md) y manifest.json obligatorios en la capsula. Rutas canonicas: Cúmulo paths.toolCapsules.'
  language: rust
  rationale: Rendimiento, seguridad de memoria, portabilidad y distribución como binario. Las implementaciones por defecto de tools y de scripts de skills han de ser en Rust formato exe.
definition_artefacts:
- ext: .md
  format: frontmatter_yaml
  naming: spec.md
  path: paths.toolsDefinitionPath/<tool-id>/
  purpose: 'Especificación: frontmatter YAML (metadatos) + cuerpo Markdown. Campos: toolId, version, description, implementation_path_ref. es-ES.'
description: 'Contrato que toda herramienta (tool) debe cumplir: definición en paths.toolsDefinitionPath/<tool-id>/ con archivo .md con frontmatter YAML; implementación en paths.toolCapsules; salida JSON y feedback estructurado.'
feedback:
  description: 'Las herramientas deben mantener un feedback adecuado: trazabilidad de fases, niveles y mensajes.'
  entry_schema:
    level: info | warning | error.
    message: Texto breve y legible del evento.
    phase: Identificador de la fase o paso (ej. docker, mysql, api, clients).
    timestamp: ISO 8601 del momento del evento.
  optional_per_entry:
    detail: Texto adicional o código de error.
    duration_ms: Duración del paso en ms.
  rules:
  - Cada fase o paso significativo debe generar al menos una entrada en feedback.
  - En caso de error, debe existir una entrada con level 'error' que describa el fallo.
  - Las advertencias (recursos no disponibles, timeouts parciales) deben registrarse con level 'warning'.
implementation:
  format: Rust executable (.exe)
  location_pattern: scripts/tools/{tool-id}/bin/{nombre}.exe
  migration_note: Las herramientas existentes con .ps1 deben migrar a .exe. Mientras no exista .exe, se mantiene temporalmente el .ps1 documentado como 'pendiente de migración'.
  prohibited_formats:
  - .ps1
  - .bat
  - .sh
  source_location: scripts/tools-rs/src/{nombre}.rs
  standard: Solo se deben generar ejecutables .exe compilados desde Rust. Los scripts PowerShell (.ps1) están prohibidos en nuevas implementaciones.
output:
  description: Toda herramienta debe producir al finalizar un resultado en formato JSON, adecuado a su propósito.
  optional_fields:
    data: Objeto libre con datos específicos del fin de la herramienta (servicios levantados, URLs, errores por fase, etc.).
    duration_ms: Duración total de la ejecución en milisegundos.
  output_modes:
  - Escribir el JSON en un fichero si se indica -OutputPath (o equivalente).
  - Emitir el JSON por stdout si se indica -OutputJson o variable de entorno TOOLS_OUTPUT_JSON=1.
  - En cualquier caso, el resultado interno debe poder serializarse según este esquema.
  required_fields:
    exitCode: Código de salida numérico (0 = éxito, distinto de 0 = fallo).
    feedback: Array de entradas de feedback ordenadas cronológicamente.
    message: Mensaje breve de resumen (humano y máquina).
    success: 'Booleano: true si la ejecución fue correcta.'
    timestamp: ISO 8601 de finalización (UTC recomendado).
    toolId: Identificador de la herramienta (kebab-case, ej. prepare-full-env).
required_artefacts_per_tool:
- ext: .rs
  path: paths.toolsRustPath (Cúmulo)/src/bin
  purpose: Implementación por defecto en Rust.
- ext: binary
  path: paths.toolsPath (Cúmulo) /<tool-id>/bin/<tool_bin>.exe
  purpose: Ejecutable Rust en la capsula (copiado desde tools-rs/install.ps1). OBLIGATORIO para nuevas herramientas.
- ext: .bat
  purpose: 'Launcher en capsula: invoca binario en bin/.'
- ext: .json
  naming: manifest
  purpose: 'Manifest de la capsula: toolId, components, contract_ref (obligatorio en cada capsula).'
- ext: .json
  naming: <tool-id>-config
  purpose: Configuración machine-readable.
- ext: .md
  naming: <tool-id>.md
  purpose: 'Documentación. Idioma: es-ES.'
scope: paths.toolsPath (Cúmulo), paths.toolsDefinitionPath (SddIA/tools/)
security_model:
  description: Toda tool debe ejecutarse bajo el contexto de un Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato de herramientas (Cúmulo: paths.toolsPath / paths.toolCapsules)

**Alcance:** Todas las entidades en **paths.toolsPath** y en cada **paths.toolCapsules[&lt;tool-id&gt;]** (Cúmulo, `SddIA/agents/cumulo.json`) que actúen como herramientas ejecutables. Listado de herramientas: **paths.toolsIndexPath** (índice en raíz de tools).

**Desacoplamiento definición / implementación:** La **definición** (qué hace la herramienta, contrato, entradas/salidas) está en **paths.toolsDefinitionPath**/&lt;tool-id&gt;/ (SddIA/tools/&lt;tool-id&gt;/) en formato .md y .json. La **implementación** (scripts, config, ejecutables) está en **paths.toolCapsules[&lt;tool-id&gt;]** (scripts). La raíz del path de implementación la indica Cúmulo; en la definición (spec.json) debe indicarse **implementation_path_ref** (ej. `paths.toolCapsules.&lt;tool-id&gt;`) para resolver la ruta desde Cúmulo sin duplicar rutas literales.

**Objetivo:** Unificar la salida en JSON adecuada al fin de cada herramienta y garantizar un **feedback adecuado** (trazable, por fases y niveles).

---

## 1. Salida JSON

Toda herramienta debe producir un **resultado final en JSON** que cumpla al menos:

| Campo      | Tipo     | Obligatorio | Descripción |
|------------|----------|-------------|-------------|
| `toolId`   | string   | Sí          | Identificador de la herramienta (kebab-case). |
| `exitCode` | number   | Sí          | Código de salida (0 = éxito). |
| `success`  | boolean  | Sí          | `true` si la ejecución fue correcta. |
| `timestamp`| string   | Sí          | ISO 8601 de finalización. |
| `message`  | string   | Sí          | Resumen breve del resultado. |
| `feedback` | array    | Sí          | Lista ordenada de eventos de feedback. |
| `data`     | object   | No          | Datos específicos del fin de la herramienta. |
| `duration_ms` | number | No       | Duración total en milisegundos. |

**Formas de entrega del JSON:**

- **Fichero:** si la herramienta recibe un parámetro de salida (p. ej. `-OutputPath`), escribe el JSON en esa ruta.
- **Stdout:** si se indica `-OutputJson` o `TOOLS_OUTPUT_JSON=1`, emitir el JSON por stdout al final (para piping o integración).

---

## 2. Feedback adecuado

El array `feedback` es el registro de lo que fue ocurriendo durante la ejecución. Cada entrada debe tener:

| Campo       | Tipo   | Descripción |
|-------------|--------|-------------|
| `phase`     | string | Fase o paso (ej. `docker`, `mysql`, `api`, `clients`). |
| `level`     | string | `info` \| `warning` \| `error`. |
| `message`   | string | Mensaje breve y legible. |
| `timestamp` | string | ISO 8601 del evento. |
| `detail`    | string | (Opcional) Detalle o código de error. |
| `duration_ms` | number | (Opcional) Duración del paso en ms. |

**Reglas:**

1. **Trazabilidad:** Cada fase o paso significativo debe generar al menos una entrada en `feedback`.
2. **Errores:** Si algo falla, debe existir una entrada con `level: "error"` que describa el fallo.
3. **Advertencias:** Situaciones recuperables (timeouts parciales, recursos no encontrados pero opcionales) deben registrarse con `level: "warning"`.
4. **Orden:** Las entradas deben ir en orden cronológico (primera ocurrencia primero).

Así se mantiene un **feedback adecuado** tanto para humanos (mensajes claros) como para máquinas (fases, niveles, tiempos).

---

## 3. Implementación por defecto: Rust

**Las implementaciones por defecto de las herramientas (y de los scripts de skills) han de ser en Rust.**

- **Motivo:** rendimiento, seguridad de memoria, portabilidad y distribución como binario único.
- **Entrega:** cada herramienta reside en una **cápsula** **paths.toolCapsules[&lt;tool-id&gt;]** (Cúmulo). Los ejecutables se construyen en paths.toolsRustPath (Cúmulo) y se copian a `&lt;cápsula&gt;/bin/`. Opcional: wrapper `.bat` en **paths.toolsPath** que delegue a la cápsula.
- **Launcher:** dentro de la cápsula, el `.bat` invoca el `.exe` en `bin/` si existe; en caso contrario, fallback al script `.ps1` de la cápsula.
- **Config** (`.json`), **documentación** (`.md`) y **manifest.json** (toolId, components, contract_ref) son obligatorios en la cápsula. **Rutas canónicas:** Cúmulo `SddIA/agents/cumulo.json` → **paths.toolsPath**, **paths.toolCapsules**. En documentación .md no usar rutas literales; referenciar vía Cúmulo.

Referencia: agente Security Engineer (paths (Cúmulo) o agente Security Engineer).

## 4. Artefactos por herramienta

Cada herramienta reside en una **cápsula** **paths.toolCapsules[&lt;tool-id&gt;]** (Cúmulo) y debe contar con:

- **Implementación Rust:** código en `scripts/tools-rs/src/bin/&lt;tool_bin&gt;.rs`; binario final en `&lt;cápsula&gt;/bin/` (copiado tras `scripts/tools-rs/install.ps1`).
- **Fallback:** script `.ps1` en la cápsula cuando no exista o no se compile el binario Rust.
- **Launcher:** `.bat` en la cápsula que invoque el binario en `bin/` si existe, si no el `.ps1`. Opcional: wrapper `.bat` en **paths.toolsPath** que delegue a la cápsula.
- **manifest.json:** toolId, components (launcher_bat, launcher_ps1, config, doc, bin), contract_ref.
- **Configuración:** cuando sea parametrizable, un `.json` de configuración en la cápsula.
- **Documentación:** un `.md` en la cápsula que describa uso, parámetros y formato de la salida JSON. Idioma: es-ES.

---

## 5. Restricciones

- `toolId` en kebab-case.
- JSON de salida válido y UTF-8.
- No incluir datos sensibles (contraseñas, tokens) en `message`, `feedback` ni `data`.
- Coherencia: `exitCode === 0` solo cuando `success === true`; en fallo, `success === false` y `exitCode !== 0`.

---

## 6. Consumidores

El contrato permite que acciones, agentes, otros scripts y pipelines (CI/CD) consuman un resultado uniforme y un feedback estructurado de todas las herramientas en **paths.toolsPath** y en cada cápsula **paths.toolCapsules[&lt;tool-id&gt;]** (Cúmulo).

**Referencia machine-readable:** `SddIA/tools/tools-contract.json`.
