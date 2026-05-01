---
contract_ref: paths.processPath/process-contract.md
inputs:
  description: Descripción breve. Obligatorio.
  skill_id: kebab-case. Obligatorio.
paths:
  featurePath_ref: paths.featurePath (Cúmulo)
  skillCapsules_ref: paths.skillCapsules (Cúmulo)
  skillsDefinitionPath_ref: paths.skillsDefinitionPath (Cúmulo)
  skillsIndexPath_ref: paths.skillsIndexPath (Cúmulo)
  skillsPath_ref: paths.skillsPath (Cúmulo)
  skillsRustPath_ref: paths.skillsRustPath (Cúmulo)
capsule_io_ref: SddIA/norms/capsule-json-io.md
persist_ref: paths.featurePath/create-skill-<skill-id>
portability_ref: SddIA/skills/reproducir-create-skill-en-otros-entornos-sddia.md
process_doc_ref: paths.processPath/create-skill/
process_id: create-skill
process_interface_compliance: 'Genera en carpeta de la tarea al menos un .md; entregable ejecutable: cápsula paths.skillCapsules[<skill-id>] con bin/<nombre>.exe, manifest.json, launcher .bat y doc .md según skills-contract.'
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

El código fuente Rust vive en **paths.skillsRustPath** (Cúmulo), típicamente `src/bin/<skill-name>.rs` del crate de skills-rs.

**Proceso de compilación e integración:**

1. Implementar el binario bajo paths.skillsRustPath (p. ej. `src/bin/<skill-name>.rs`).
2. Registrar el binario en el `Cargo.toml` del crate (entrada `[[bin]]`).
3. Compilar y copiar el `.exe` a **paths.skillCapsules[&lt;skill-id&gt;]/bin/** según el flujo del proyecto (instalación del crate en paths.skillsRustPath). La **invocación de comandos** debe cumplir `SddIA/norms/commands-via-skills-or-tools.md` (p. ej. skill **invoke-command** o cápsula/skill de instalación trazada), no ejecutar scripts sueltos desde el agente.
4. Actualizar **paths.skillsIndexPath** (Cúmulo).
5. Actualizar **paths** en `SddIA/agents/cumulo.paths.json` (mapeo **skillCapsules** para &lt;skill-id&gt;).

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

- Contrato skills: SddIA/skills/skills-contract.md.
- E/S JSON cápsulas: SddIA/norms/capsule-json-io.md.
- Cúmulo: paths.skillsDefinitionPath, paths.skillsPath, paths.skillCapsules, paths.skillsIndexPath, paths.skillsRustPath.
- Portabilidad del proceso a otros repos: SddIA/skills/reproducir-create-skill-en-otros-entornos-sddia.md.
- Proceso machine-readable: paths.processPath/create-skill/spec.json.
