# Análisis: Restructuración de process y actions al patrón skills/tools (Kaizen)

**Objetivo (acción Kaizen):** Reestructurar `SddIA/process` y `SddIA/actions` para cumplir el mismo patrón que `SddIA/skills` y `SddIA/tools`: **cada ítem en su carpeta** y **contratos explícitos** (process-contract, actions-contract) implementados correctamente.

**Fuente:** Análisis de situación actual. Rutas según Cúmulo (paths.processPath, paths.actionsPath, paths.skillsDefinitionPath, paths.toolsDefinitionPath).

---

## 1. Patrón de referencia: skills y tools

### 1.1 Estructura skills

| Elemento | Ubicación | Contenido |
|----------|-----------|-----------|
| **Contrato** | `SddIA/skills/skills-contract.json` + `skills-contract.md` | Define artefactos obligatorios (spec.md, spec.json por skill), implementation_path_ref, consumers, constraints. |
| **Índice/README** | `SddIA/skills/README.md` | Listado y descripción de skills. |
| **Por skill** | `SddIA/skills/<skill-id>/` | **Carpeta por ítem.** Dentro: `spec.md`, `spec.json` (con skill_id, phases, parameters, contract_ref, implementation_path_ref). |

- **Cúmulo:** paths.skillsDefinitionPath = ./SddIA/skills/; paths.skillCapsules[skill-id] = ./scripts/skills/<skill-id>/.
- Cada skill tiene **identificador único** (skill-id), **definición** en SddIA y **implementación** en cápsula (referencia vía Cúmulo).

### 1.2 Estructura tools

| Elemento | Ubicación | Contenido |
|----------|-----------|-----------|
| **Contrato** | `SddIA/tools/tools-contract.json` + `tools-contract.md` | Salida JSON, feedback, artefactos por tool, constraints. |
| **Índice/README** | `SddIA/tools/README.md` | Listado de herramientas. |
| **Por tool** | `SddIA/tools/<tool-id>/` | **Carpeta por ítem.** Dentro: `spec.md`, `spec.json` (toolId, inputs, output, contract_ref, implementation_path_ref). |

- **Cúmulo:** paths.toolsDefinitionPath = ./SddIA/tools/; paths.toolCapsules[tool-id] = ./scripts/tools/<tool-id>/.

### 1.3 Contrato explícito (skills-contract.json resumido)

- `contract_version`, `scope`, `description`
- `definition_artefacts`: spec.md y spec.json en paths.skillsDefinitionPath/<skill-id>/
- `required_artefacts_capsule`, `implementation_requirements`, `constraints`
- `consumers`: paths.actionsPath, agents, paths.processPath

---

## 2. Situación actual: process

### 2.1 Estructura actual

| Ubicación | Contenido | ¿Carpeta por ítem? | ¿spec.md + spec.json por ítem? |
|-----------|-----------|--------------------|---------------------------------|
| `SddIA/process/README.md` | Índice y uso | — | — |
| `SddIA/process/feature.md` | Proceso feature (solo .md) | No | No (solo .md) |
| `SddIA/process/refactorization.md` | Proceso refactorization (solo .md) | No | No |
| `SddIA/process/create-tool.md` | Proceso create-tool (doc) | No | No (doc en .md) |
| `SddIA/process/create-tool.json` | Proceso create-tool (machine-readable) | No | Parcial (.json existe, .md aparte) |
| `SddIA/process/bug-fix-specialist.json` | Agente/proceso bug-fix (solo .json) | No | No (solo .json, sin .md en misma “unidad”) |

- **No existe** `process-contract.json` ni `process-contract.md` en la raíz de process.
- **Interfaz de proceso** está definida **solo en Cúmulo** (cumulo.json → `process_interface`: al menos un .md y un .json en carpeta de la tarea). No hay contrato en SddIA/process/ que unifique requisitos por proceso.
- **Inconsistencia:** create-tool tiene .md + .json en raíz; feature y refactorization solo .md; bug-fix solo .json. No hay patrón “carpeta <process-id>/ con spec.md y spec.json”.

### 2.2 Consumidores que referencian process

- paths.processPath (Cúmulo).
- interaction-triggers.md (#Process): listado desde paths.processPath (README + ficheros).
- feature.md, refactorization.md: “acciones en paths.actionsPath”.
- create-tool.json: process_doc_ref = paths.processPath/create-tool.md.
- Skills (iniciar-rama): related_artefacts = paths.processPath/feature.md, bug-fix-specialist.json.

### 2.3 Gap frente al patrón skills/tools

| Aspecto | Skills/Tools | Process (actual) |
|---------|--------------|-------------------|
| Contrato en SddIA | skills-contract.json, tools-contract.json en raíz | Solo process_interface en Cúmulo; no hay process-contract.json en SddIA/process/ |
| Unidad por ítem | Carpeta <id>/ con spec.md + spec.json | Ficheros sueltos en raíz (feature.md, create-tool.json, etc.) |
| Identificador | skill-id, tool-id (kebab-case) | process_id existe en create-tool.json y bug-fix-specialist.json; no hay carpeta que lo refleje |
| Referencia a definición | paths.skillsDefinitionPath/<skill-id>/, paths.toolsDefinitionPath/<tool-id>/ | paths.processPath/feature.md (ruta a fichero, no a carpeta) |

---

## 3. Situación actual: actions

### 3.1 Estructura actual

| Ubicación | Contenido | ¿Carpeta por ítem? | ¿spec.md + spec.json por ítem? |
|-----------|-----------|--------------------|---------------------------------|
| `SddIA/actions/spec.md` | Acción spec | No | No (solo .md) |
| `SddIA/actions/clarify.md` | Acción clarify | No | No |
| `SddIA/actions/planning.md` | Acción planning | No | No |
| `SddIA/actions/implementation.md` | Acción implementation | No | No |
| `SddIA/actions/execution.md` | Acción execution | No | No |
| `SddIA/actions/validate.md` | Acción validate | No | No |
| `SddIA/actions/finalize.md` | Acción finalize | No | No |
| `SddIA/actions/sddia-difusion.md` | Acción sddia-difusion | No | No |

- **No existe** `actions-contract.json` ni `actions-contract.md` en la raíz de actions.
- **No hay** spec.json por acción: solo ficheros .md sueltos en la raíz de paths.actionsPath.
- **No hay** carpeta por acción: no existe `SddIA/actions/spec/`, `SddIA/actions/finalize/`, etc.

### 3.2 Consumidores que referencian actions

- paths.actionsPath (Cúmulo).
- Process (feature.md, refactorization.md, create-tool): “acciones en paths.actionsPath”.
- interaction-triggers.md (#Action): listado desde paths.actionsPath (cada .md es una acción).
- AGENTS.norms.md, finalize.md (skill finalizar-git): referencias a paths.actionsPath/<action>.md.

### 3.3 Gap frente al patrón skills/tools

| Aspecto | Skills/Tools | Actions (actual) |
|---------|--------------|------------------|
| Contrato en SddIA | skills-contract.json, tools-contract.json | No existe actions-contract.json ni .md |
| Unidad por ítem | Carpeta <id>/ con spec.md + spec.json | Un solo .md por acción en raíz (spec.md, finalize.md, …) |
| Identificador | action_id = nombre fichero sin .md | Implícito por nombre de fichero; no hay action_id en JSON por acción |
| Machine-readable | spec.json por ítem (phases, parameters, contract_ref) | No hay JSON por acción; solo documentación .md |

---

## 4. Resumen de gaps

| Ámbito | Contrato explícito (raíz) | Carpeta por ítem | spec.md + spec.json por ítem | Referencia Cúmulo coherente |
|--------|---------------------------|------------------|------------------------------|-----------------------------|
| **process** | No (solo process_interface en Cúmulo) | No | Parcial (create-tool tiene ambos en raíz; resto no) | paths.processPath apunta a ficheros, no a carpetas |
| **actions** | No | No | No (solo .md) | paths.actionsPath apunta a ficheros .md en raíz |

---

## 5. Propuesta de contratos (para implementación futura)

### 5.1 process-contract.json (borrador)

- **scope:** paths.processPath (SddIA/process/) — definición de procesos de tarea.
- **description:** Todo proceso debe tener definición en paths.processPath/<process-id>/ con spec.md y spec.json; cumple process_interface (Cúmulo) para artefactos en carpeta de la tarea.
- **definition_artefacts:** spec.md y spec.json en paths.processPath/<process-id>/.
- **spec.json por proceso:** process_id, name, description, contract_ref (process-contract.json), process_interface_compliance, phases/steps, persist_ref (paths.featurePath | paths.fixPath), related_actions (paths.actionsPath/…), related_skills (paths.skillCapsules).
- **constraints:** process_id en kebab-case; rutas solo vía Cúmulo.
- **consumers:** paths.actionsPath, SddIA/agents, SddIA/norms (interaction-triggers).

### 5.2 actions-contract.json (borrador)

- **scope:** paths.actionsPath (SddIA/actions/) — definición de acciones del ciclo.
- **description:** Toda acción debe tener definición en paths.actionsPath/<action-id>/ con spec.md y spec.json; entrada/salida y flujo documentados.
- **definition_artefacts:** spec.md y spec.json en paths.actionsPath/<action-id>/.
- **spec.json por acción:** action_id, name, description, contract_ref (actions-contract.json), purpose, inputs, outputs, flow_steps, related_processes, related_skills.
- **constraints:** action_id en kebab-case; rutas solo vía Cúmulo.
- **consumers:** paths.processPath, SddIA/agents, SddIA/norms.

---

## 6. Estructura objetivo (resumen)

### 6.1 Process

```
SddIA/process/
  process-contract.json
  process-contract.md
  README.md
  feature/
    spec.md
    spec.json
  bug-fix/
    spec.md
    spec.json
  refactorization/
    spec.md
    spec.json
  create-tool/
    spec.md
    spec.json
```

- **Migración:** feature.md → feature/spec.md (+ feature/spec.json generado o redactado). Igual para refactorization, create-tool (repartir contenido .md y .json actual en spec.md y spec.json dentro de create-tool/). bug-fix-specialist.json → bug-fix/spec.json + bug-fix/spec.md (extraer o redactar spec.md desde el json).

### 6.2 Actions

```
SddIA/actions/
  actions-contract.json
  actions-contract.md
  README.md
  spec/
    spec.md
    spec.json
  clarify/
    spec.md
    spec.json
  planning/
    spec.md
    spec.json
  implementation/
    spec.md
    spec.json
  execution/
    spec.md
    spec.json
  validate/
    spec.md
    spec.json
  finalize/
    spec.md
    spec.json
  sddia-difusion/
    spec.md
    spec.json
```

- **Migración:** Cada <action>.md → <action>/spec.md; crear <action>/spec.json con action_id, purpose, inputs, outputs, flow (resumido).

---

## 7. Impacto en referencias (Cúmulo y normas)

- **paths.processPath** hoy = ./SddIA/process/. Si se mantiene, las referencias pasan de `paths.processPath/feature.md` a `paths.processPath/feature/spec.md` (o `paths.processPath/feature/` con convención de que el punto de entrada es spec).
- **paths.actionsPath** hoy = ./SddIA/actions/. Referencias de `paths.actionsPath/finalize.md` a `paths.actionsPath/finalize/spec.md` (o finalize/ con spec como estándar).
- **interaction-triggers.md:** actualizar list_source y ejemplos a rutas por carpeta (paths.processPath/<process-id>/, paths.actionsPath/<action-id>/).
- **AGENTS.md, process README, create-tool, bug-fix, feature.md, refactorization.md:** actualizar enlaces a definiciones por carpeta y a los nuevos contratos.

---

## 8. Próximos pasos sugeridos (Kaizen)

1. **Definir y aprobar** process-contract.json y actions-contract.json (y .md) en SddIA.
2. **Crear carpetas** por proceso y por acción; migrar contenido existente a <id>/spec.md y <id>/spec.json.
3. **Actualizar Cúmulo** si hace falta (process_interface puede seguir en cumulo.json; el contrato en SddIA/process/ sería la definición formal del dominio).
4. **Actualizar normas y referencias** (interaction-triggers, README process/actions, AGENTS.md, .cursor/rules si aplica).
5. **Validar** que procesos y acciones sigan siendo descubribles (listados #Process, #Action) y que las rutas vía Cúmulo sigan siendo la única fuente.

---
*Análisis para acción Kaizen: restructuración process/actions al patrón skills/tools. SddIA como SSOT.*
