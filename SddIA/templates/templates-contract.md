---
constraints:
- template_id en kebab-case.
- Rutas en input_sources deben poder resolverse vía Cúmulo cuando sean canónicas.
- process_ref debe existir en paths.processPath.
consumers:
- paths.actionsPath
- SddIA/agents/*.json
- paths.processPath
- .cursor/rules
contract_version: 1.0.0
description: 'Contrato que cada plantilla (template) en SddIA/templates debe cumplir: configuración predefinida de un proceso con fin concreto, orígenes de entrada y artefactos.'
folder_structure: Cada plantilla en SddIA/templates/<template-id>/ con spec.md (frontmatter YAML).
json_schema:
  properties:
    config:
      description: Parámetros opcionales por defecto (rama, carpeta de persistencia, etc.).
      type: object
    contract_ref:
      description: 'Referencia al contrato: SddIA/templates/templates-contract.json'
      type: string
    description:
      description: Descripción breve del fin concreto y del procedimiento.
      type: string
    input_sources:
      additionalProperties:
        type: string
      description: 'Orígenes de entrada: rutas Cúmulo, parciales, totales o ficheros.'
      type: object
    interested_agents:
      description: Agentes que orquestan o consumen esta plantilla (architect, tekton-developer, etc.).
      items:
        type: string
      type: array
    name:
      description: Nombre legible de la plantilla.
      type: string
    process_ref:
      description: 'Proceso que orquesta la tarea (feature, correccion-auditorias, bug-fix, etc.). Referencia: paths.processPath/<process-id>.'
      type: string
    related_actions:
      description: Acciones del ciclo que aplican (spec, clarify, planning, implementation, execution, validate, finalize).
      items:
        type: string
      type: array
    related_skills:
      description: Skills recomendadas (suite táctica Git: git-workspace-recon, git-branch-manager, git-save-snapshot, git-sync-remote, git-tactical-retreat, git-create-pr; además invoke-commit, documentation, etc.).
      items:
        type: string
      type: array
    template_id:
      description: Identificador de la plantilla en kebab-case.
      type: string
  required:
  - template_id
  - name
  - description
  - process_ref
  - related_actions
  - contract_ref
  type: object
required_files:
- description: Archivo .md con frontmatter YAML (metadatos) + cuerpo Markdown.
  format: markdown_frontmatter_yaml
  language: es-ES
  name: spec.md
  required: true
scope: SddIA/templates/
security_model:
  description: La ejecución de una plantilla requiere un contexto de Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato de Plantillas (SddIA/templates/)

**Alcance:** Todas las entidades bajo `SddIA/templates/`.

**Objetivo:** Disponer de configuraciones predefinidas (plantillas) que procedimenten el uso de procesos con un fin concreto, alineadas con los patrones de SddIA (skills, tools, actions, processes). Las plantillas facilitan arrancar una tarea con parámetros y orígenes de entrada ya definidos.

---

## 1. Propósito de las plantillas

- **Procedimentar:** Una plantilla asocia un **proceso** (feature, correccion-auditorias, etc.) con una **configuración encarada a un fin concreto**.
- **Entrada predefinida:** Origen de datos (rutas Cúmulo, rutas parciales o totales, ficheros) declarado en la plantilla.
- **Ciclo de vida:** La plantilla puede referenciar acciones y artefactos del proceso (objectives, spec, clarify, planning, etc.) para que el agente ejecute el ciclo completo.

Las plantillas son elementos de SddIA y cumplen la interfaz de proceso (al menos un `.md` y un `.json` por plantilla).

---

## 2. Estructura por plantilla

Cada plantilla reside en una carpeta nombrada con su **template_id** (kebab-case): `SddIA/templates/<template-id>/`.

### Archivos obligatorios

1. **`spec.md`**
   - **Contenido:** Descripción legible de la plantilla: propósito, proceso asociado, orígenes de entrada, uso, criterios de cierre.
   - **Idioma:** Español (es-ES).
   - **Formato:** Markdown estándar.

2. **`spec.json`**
   - **Contenido:** Metadatos estructurados para consumo por agentes.
   - **Esquema:** Definido en `templates-contract.json`.
   - **Campos clave:** template_id, name, description, process_ref, input_sources, config (opcional), related_actions, related_skills, contract_ref.

### Archivos opcionales

- **`config.json`**: Parámetros por defecto de la plantilla (valores que el usuario puede sobrescribir al instanciar la tarea).

---

## 3. Orígenes de entrada (input_sources)

Las plantillas declaran de dónde provienen los datos de la tarea:

- **Rutas Cúmulo:** Referencias canónicas (ej. `paths.auditsPath`, `paths.featurePath`). Resolver según Cúmulo.
- **Rutas parciales:** Relativas al repo (ej. `docs/audits/`, `docs/features/<nombre>/`).
- **Rutas totales:** Absolutas cuando se indique explícitamente (fichero concreto, URL de informe).
- **Fichero:** Nombre o patrón de fichero (ej. `AUDITORIA_*.md`, `validacion-*.json`).

El agente que ejecute la plantilla debe resolver estas referencias según la norma paths-via-cumulo cuando aplique.

---

## 4. Seguridad y trazabilidad

- **Karma2Token:** Toda ejecución de una plantilla debe operar bajo el contexto de un Karma2Token válido (paths.tokensPath; spec en SddIA/tokens/karma2-token/spec.json).
- **Consumidores:** paths.actionsPath, SddIA/agents, paths.processPath, .cursor/rules.

---

## 5. Referencias

- **Esquema JSON:** `SddIA/templates/templates-contract.json`
- **Proceso de creación:** `SddIA/process/create-template/` (spec.md, spec.json)
- **Cúmulo:** paths.templatesPath
