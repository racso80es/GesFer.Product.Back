# Proceso: Creación de Patrones (create-pattern)

Este documento define el **proceso** para añadir nuevos patrones de diseño y arquitectura a `SddIA/patterns/`.

## Objetivo

Estandarizar la incorporación de conocimiento sobre patrones, asegurando que cada entrada sea completa, legible y utilizable por los agentes (especialmente Arquitectos y Tekton).

## Alcance

- **Ubicación:** `SddIA/patterns/`
- **Estructura:** Carpeta nombrada con UUID.
- **Contenido:** `spec.md` (legible) y `spec.json` (metadatos).

## Fases del Proceso

### 1. Definición

1.  **Generar UUID:** Cada patrón debe tener un identificador único (UUID v4).
2.  **Crear Carpeta:** Crear `SddIA/patterns/<uuid>/`.
3.  **Redactar `spec.md`:**
    *   Título y descripción detallada en Markdown.
    *   Idioma: Español (es-ES).
    *   Debe incluir referencias y contexto de uso.
4.  **Definir `spec.json`:**
    *   `id`: UUID del patrón.
    *   `title`: Título del patrón.
    *   `category`: Categoría (e.g., Arquitectura, Diseño).
    *   `tags`: Lista de etiquetas.
    *   `metadata`: Dificultad, estado, etc.
    *   `interested_agents`: Lista de agentes que deben conocer este patrón (mapeado desde la categoría).

### 2. Validación

1.  Verificar que `spec.json` cumple con `SddIA/patterns/patterns-contract.json`.
2.  Asegurar que `spec.md` tiene un formato claro y contenido útil.

## Mapeo de Agentes Interesados

| Categoría | Agentes Sugeridos |
| :--- | :--- |
| Arquitectura de Software | `architect`, `infrastructure-architect` |
| Patrones de Diseño | `tekton-developer`, `architect` |
| Sistemas Distribuidos | `architect`, `infrastructure-architect`, `performance-engineer` |
| Domain-Driven Design | `architect`, `clarifier` |
| *Default* | `architect`, `tekton-developer` |

## Artefactos

*   `SddIA/patterns/<uuid>/spec.md`
*   `SddIA/patterns/<uuid>/spec.json`
