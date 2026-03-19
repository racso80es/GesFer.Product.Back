---
contract_ref: paths.processPath/process-contract.md
inputs:
  description: Descripción breve. Obligatorio.
  skill_id: kebab-case. Obligatorio.
paths:
  featurePath_ref: paths.featurePath (Cúmulo)
  skillCapsulesRef: paths.skillCapsules
  skillsDefinitionPath: ./SddIA/skills/
  skillsIndexPath_ref: paths.skillsIndexPath (Cúmulo)
  skillsPath_ref: paths.skillsPath (Cúmulo)
persist_ref: paths.featurePath/create-skill-<skill-id>
process_doc_ref: paths.processPath/create-skill/
process_id: create-skill
process_interface_compliance: 'Genera en carpeta de la tarea al menos un .md; entrega ejecutable: cápsula en paths.skillsPath/<skill-id>/.'
related_actions:
- spec
- implementation
- validate
- finalize
related_skills:
- iniciar-rama
spec_version: 1.0.0
skills_contract_ref: SddIA/skills/skills-contract.md
triggers:
- Crear nueva skill en paths.skillsPath
- Solicitud de creación de skill con skill-id
---

# Proceso: Creación de skills (create-skill)

Este documento define el **proceso de tarea** para crear una nueva skill en el proyecto. Está ubicado en paths.processPath/create-skill/ (Cúmulo). Las rutas de skills se obtienen de **Cúmulo** (paths.skillsPath, paths.skillCapsules, paths.skillsIndexPath).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): la tarea de creación genera en la carpeta de la tarea (Cúmulo) al menos un **`.md`** (objectives.md, spec.md, implementation.md). El **resultado ejecutable** es la cápsula en **paths.skillCapsules[<skill-id>]** con todos los artefactos requeridos por el contrato de skills.

## Propósito

El proceso **create-skill** define el procedimiento para incorporar una nueva skill al ecosistema de paths.skillsPath (Cúmulo): desde la definición del objetivo hasta la cápsula lista, el índice actualizado y Cúmulo sincronizado. Garantiza que cada skill cumpla SddIA/skills/skills-contract.md y que las rutas queden registradas en Cúmulo y en scripts/skills/index.json.

## Alcance del procedimiento

- **Documentación de la tarea:** Cúmulo (paths.featurePath/create-skill-<skill-id>/).
- **Definición (SddIA):** paths.skillsDefinitionPath/<skill-id>/ con spec.md (implementation_path_ref obligatorio).
- **Cápsula (implementación):** paths.skillCapsules[<skill-id>].

Fases: 0 Preparar entorno | 1 Objetivos y especificación | 1b Definición en SddIA | 2–6 Cápsula, manifest, launcher, índice, Cúmulo | 7 Validación | 8 Cierre.

## Restricciones

- skill_id en kebab-case. Rama feat/create-skill-<skill-id>. Windows 11, PowerShell 7+. Contrato skills (JSON entrada/salida, solo .exe) obligatorio.

## Implementación de la skill

La skill debe implementarse **únicamente como ejecutable Rust** (`.exe`).

**Estructura esperada de la cápsula:**

```
scripts/skills/<skill-id>/
├── bin/
│   └── <skill-name>.exe          # Ejecutable Rust compilado (OBLIGATORIO)
├── manifest.json                   # Metadatos de la skill (OBLIGATORIO)
├── <skill-name>.md                # Documentación de uso (OBLIGATORIO)
└── <skill-name>.bat               # Launcher que invoca solo .exe (OBLIGATORIO)
```

**Fuente Rust:**

El código fuente Rust debe ubicarse en:
```
scripts/skills-rs/src/bin/<skill-name>.rs
```

**Proceso de compilación:**

1. Desarrollar en `scripts/skills-rs/src/bin/<skill-name>.rs`
2. Añadir entrada `[[bin]]` en `scripts/skills-rs/Cargo.toml`
3. Ejecutar `scripts/skills-rs/install.ps1` para compilar y copiar el .exe a la cápsula
4. Actualizar el índice: `scripts/skills/index.json`
5. Actualizar Cúmulo: `SddIA/agents/cumulo.paths.json` (campo `skillCapsules`)

**Prohibiciones:**

❌ **NO se deben crear:**
- Archivos `.ps1` (PowerShell scripts)
- Archivos `.bat` como implementación principal (solo como launcher que invoca .exe)
- Scripts shell (`.sh`)
- Cualquier otro formato de script

✅ **Solo se debe generar:** Ejecutable `.exe` compilado desde Rust.

**Migración desde .ps1:**

Si estás migrando una skill existente desde `.ps1` a `.exe`:
1. Implementar en Rust
2. Validar funcionamiento del `.exe`
3. Eliminar el `.ps1`
4. Actualizar la spec de la skill con sección "Implementación"

## Referencias

- Contrato: SddIA/skills/skills-contract.md.
- Cúmulo: paths.skillsDefinitionPath, paths.skillsPath, paths.skillCapsules, paths.skillsIndexPath.
- Proceso machine-readable: paths.processPath/create-skill/spec.json.
