---
contract_version: 1.0.0
description: Contrato que cada patrón de diseño debe cumplir en SddIA/patterns.
folder_structure: 'Cada patrón debe residir en una carpeta nombrada con su UUID: SddIA/patterns/<uuid>/'
json_schema:
  properties:
    category:
      type: string
    id:
      description: UUID v4 del patrón
      type: string
    interested_agents:
      description: Lista de agentes de SddIA que deben conocer este patrón.
      items:
        type: string
      type: array
    metadata:
      properties:
        difficulty:
          enum:
          - Basic
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
scope: SddIA/patterns/
security_model:
  description: La aplicación o modificación de un patrón requiere un contexto de Karma2Token válido.
  required_token: Karma2Token
  token_ref: SddIA/tokens/karma2-token/spec.json
---

# Contrato de Patrones (SddIA/patterns/)

**Alcance:** Todas las entidades bajo `SddIA/patterns/`.

**Objetivo:** Centralizar el conocimiento de patrones de diseño y arquitectura para ser consumidos por los agentes de SddIA (especialmente `architect` y `tekton-developer`).

---

## 1. Estructura por Patrón

Cada patrón debe residir en una carpeta nombrada con su **UUID**: `SddIA/patterns/<uuid>/`.

### Archivos Obligatorios

1.  **`spec.md`**
    *   **Contenido:** Descripción legible del patrón, propósito, uso, pros/contras, ejemplos.
    *   **Idioma:** Español (es-ES).
    *   **Formato:** Markdown estándar.

2.  **`spec.json`**
    *   **Contenido:** Metadatos estructurados para consumo por agentes.
    *   **Esquema:** Definido en `patterns-contract.json`.
    *   **Campos Clave:**
        *   `id`: UUID del patrón.
        *   `title`: Título del patrón.
        *   `category`: Categoría (e.g., "Arquitectura de Software", "Patrones de Diseño").
        *   `tags`: Etiquetas relevantes.
        *   `metadata`: Objeto con `difficulty` y `status`.
        *   `interested_agents`: Lista de agentes de SddIA interesados en este patrón.

---

## 2. Agentes Interesados

La lista de `interested_agents` en `spec.json` debe basarse en la categoría del patrón:

*   **Arquitectura de Software:** `architect`, `infrastructure-architect`
*   **Patrones de Diseño:** `tekton-developer`, `architect`
*   **Sistemas Distribuidos:** `architect`, `infrastructure-architect`, `performance-engineer`
*   **Domain-Driven Design:** `architect`, `clarifier`
*   **General/Otros:** `architect`, `tekton-developer`

---

## 3. Referencias

*   **Esquema JSON:** `SddIA/patterns/patterns-contract.json`
*   **Proceso de Creación:** `SddIA/process/create-pattern.md`
