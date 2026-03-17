---
contract_version: 1.0.0
description: Contrato que cada item de seguridad debe cumplir en SddIA/security.
domain_entities: 'Los items de seguridad son entidades de dominio (ecosistema SddIA); integran el contrato Token y deben respetar estructura frontmatter YAML. Norma: SddIA/norms/entidades-dominio-ecosistema-sddia.md.'
folder_structure: 'Cada item debe residir en una carpeta nombrada con su UUID: SddIA/security/<uuid>/'
json_schema:
  properties:
    category:
      type: string
    id:
      description: UUID v4 del item de seguridad
      type: string
    interested_agents:
      description: Lista de agentes de SddIA que deben conocer este item.
      items:
        type: string
      type: array
    metadata:
      properties:
        difficulty:
          enum:
          - Beginner
          - Intermediate
          - Advanced
          type: string
        status:
          enum:
          - Draft
          - Published
          - Deprecated
          type: string
      required:
      - difficulty
      - status
      type: object
    tags:
      items:
        type: string
      type: array
    title:
      type: string
  required:
  - id
  - title
  - category
  - tags
  - metadata
  - interested_agents
  type: object
required_files:
- description: Archivo .md con frontmatter YAML (metadatos) + cuerpo Markdown.
  format: markdown_frontmatter_yaml
  language: es-ES
  name: spec.md
  required: true
scope: SddIA/security/
security_model:
  description: La consulta o aplicación de un item de seguridad requiere un contexto de Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/Tokens/karma2-token/spec.json
---

# Contrato de Seguridad (SddIA/security/)

**Alcance:** Todas las entidades bajo `SddIA/security/`.

**Objetivo:** Centralizar el conocimiento, políticas y prácticas de seguridad para ser consumidos y aplicados por los agentes de SddIA (especialmente `security-engineer`, `auditor`, `architect` y `tekton-developer`).

---

## 1. Estructura por Item de Seguridad

Cada item de seguridad debe residir en una carpeta nombrada con su **UUID**: `SddIA/security/<uuid>/`.

### Archivos Obligatorios

1.  **`spec.md`**
    *   **Contenido:** Descripción detallada del concepto de seguridad, vulnerabilidad, riesgo o política, junto con sus estrategias de mitigación.
    *   **Idioma:** Español (es-ES).
    *   **Formato:** Markdown estándar con encabezados claros (e.g., Descripción, Riesgo, Mitigación, Referencias).

2.  **`spec.json`**
    *   **Contenido:** Metadatos estructurados para consumo por agentes.
    *   **Esquema:** Definido en `security-contract.json`.
    *   **Campos Clave:**
        *   `id`: UUID del item.
        *   `title`: Título del item.
        *   `category`: Categoría (e.g., "Ciberseguridad", "DevSecOps", "Seguridad de Aplicaciones").
        *   `tags`: Etiquetas relevantes.
        *   `metadata`: Objeto con `difficulty` (Beginner, Intermediate, Advanced) y `status`.
        *   `interested_agents`: Lista de agentes de SddIA interesados en este item.

---

## 2. Agentes Interesados

La lista de `interested_agents` en `spec.json` debe basarse en la categoría del item y su impacto:

*   **Ciberseguridad (Ofensiva/Defensiva):** `security-engineer`, `auditor`, `architect`.
*   **DevSecOps / Infraestructura:** `tekton-developer`, `infrastructure-architect`, `security-engineer`.
*   **Seguridad de Aplicaciones:** `architect`, `tekton-developer`, `security-engineer`.
*   **Educación / Formación:** `security-engineer`, `auditor`.

---

## 3. Modelo de Seguridad (Karma2Token)

Todos los items de seguridad operan bajo el contexto de `Karma2Token`. Cualquier acción de creación, lectura o aplicación de estos items debe estar firmada y validada según `SddIA/tokens/karma2-token/spec.json`.

Los items de seguridad forman parte de las **entidades de dominio (ecosistema SddIA)**: son las que integran el ítem o contrato de Token. Han de respetar **estructura** (spec.md + spec.json en la carpeta del item) y **sincronidad** (paridad MD ↔ JSON). Norma: SddIA/norms/entidades-dominio-ecosistema-sddia.md.

---

## 4. Referencias

*   **Esquema JSON:** `SddIA/security/security-contract.json`
