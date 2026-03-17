---
contract_ref: paths.processPath/process-contract.md
inputs:
  description: Descripción breve. Obligatorio.
  tool_id: kebab-case. Obligatorio.
paths:
  featurePath_ref: paths.featurePath (Cúmulo)
  toolCapsulesRef: paths.toolCapsules
  toolsDefinitionPath: ./SddIA/tools/
  toolsIndexPath_ref: paths.toolsIndexPath (Cúmulo)
  toolsPath_ref: paths.toolsPath (Cúmulo)
persist_ref: paths.featurePath/create-tool-<tool-id>
process_doc_ref: paths.processPath/create-tool/
process_id: create-tool
process_interface_compliance: 'Genera en carpeta de la tarea al menos un .md y un .json; entrega ejecutable: cápsula en paths.toolsPath/<tool-id>/.'
related_actions:
- spec
- implementation
- validate
- finalize
related_skills:
- iniciar-rama
spec_version: 1.0.0
tools_contract_ref: SddIA/tools/tools-contract.md
triggers:
- Crear nueva herramienta en paths.toolsPath
- Solicitud de creación de herramienta con tool-id
---

# Proceso: Creación de herramientas (create-tool)

Este documento define el **proceso de tarea** para crear una nueva herramienta (tool) en el proyecto. Está ubicado en paths.processPath/create-tool/ (Cúmulo). Las rutas de herramientas se obtienen de **Cúmulo** (paths.toolsPath, paths.toolCapsules, paths.toolsIndexPath).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): la tarea de creación genera en la carpeta de la tarea (Cúmulo) al menos un **`.md`** (objectives.md, spec.md, implementation.md) y al menos un **`.json`** (spec.json, implementation.json, validacion.json). El **resultado ejecutable** es la cápsula en **paths.toolCapsules[<tool-id>]** con todos los artefactos requeridos por el contrato de herramientas.

## Propósito

El proceso **create-tool** define el procedimiento para incorporar una nueva herramienta al ecosistema de paths.toolsPath (Cúmulo): desde la definición del objetivo hasta la cápsula lista, el índice actualizado y Cúmulo sincronizado. Garantiza que cada herramienta cumpla SddIA/tools/tools-contract.md y que las rutas queden registradas en Cúmulo y en scripts/tools/index.json.

## Alcance del procedimiento

- **Documentación de la tarea:** Cúmulo (paths.featurePath/create-tool-<tool-id>/).
- **Definición (SddIA):** paths.toolsDefinitionPath/<tool-id>/ con spec.md y spec.json (implementation_path_ref obligatorio).
- **Cápsula (implementación):** paths.toolCapsules[<tool-id>].

Fases: 0 Preparar entorno | 1 Objetivos y especificación | 1b Definición en SddIA | 2–6 Cápsula, manifest, scripts, índice, Cúmulo | 7 Opcional Rust | 8 Validación | 9 Cierre.

## Restricciones

- toolId en kebab-case. Rama feat/create-tool-<tool-id>. Windows 11, PowerShell 7+. Contrato tools (salida JSON, feedback) obligatorio.

## Implementación de la Herramienta

La herramienta debe implementarse **únicamente como ejecutable Rust** (`.exe`).

**Estructura esperada de la cápsula:**

```
scripts/tools/<tool-id>/
├── bin/
│   └── <tool-name>.exe        # Ejecutable Rust compilado (OBLIGATORIO)
├── manifest.json               # Metadatos de la herramienta (OBLIGATORIO)
├── <tool-name>-config.json     # Configuración (si aplica)
└── <tool-name>.md              # Documentación de uso (OBLIGATORIO)
```

**Fuente Rust:**

El código fuente Rust debe ubicarse en:
```
scripts/tools-rs/src/<tool-name>.rs
```

O, si es complejo:
```
scripts/tools-rs/src/<tool-name>/
├── main.rs
├── lib.rs
└── ...
```

**Proceso de Compilación:**

1. Desarrollar en `scripts/tools-rs/src/<tool-name>.rs`
2. Compilar:
   ```powershell
   cargo build --release --manifest-path scripts/tools-rs/Cargo.toml
   ```
3. Copiar el `.exe` generado a `scripts/tools/<tool-id>/bin/<nombre>.exe`
4. Actualizar el índice: `scripts/tools/index.json`
5. Actualizar Cúmulo: `SddIA/agents/cumulo.paths.json` (campo `toolCapsules`)

**Prohibiciones:**

❌ **NO se deben crear:**
- Archivos `.ps1` (PowerShell scripts)
- Archivos `.bat` (Batch files) como implementación principal
- Scripts shell (`.sh`)
- Cualquier otro formato de script

✅ **Solo se debe generar:** Ejecutable `.exe` compilado desde Rust.

**Migración desde .ps1:**

Si estás migrando una herramienta existente desde `.ps1` a `.exe`:
1. Implementar en Rust
2. Validar funcionamiento del `.exe`
3. Eliminar el `.ps1`
4. Actualizar la spec de la herramienta con sección "Implementación"

## Referencias

- Contrato: SddIA/tools/tools-contract.md, tools-contract.md.
- Cúmulo: paths.toolsDefinitionPath, paths.toolsPath, paths.toolCapsules, paths.toolsIndexPath.
- Proceso machine-readable: paths.processPath/create-tool/spec.json.
