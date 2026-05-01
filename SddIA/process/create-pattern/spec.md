---
contract_ref: paths.processPath/process-contract.md
persist_ref: paths.patternsPath/<uuid>
phases:
- description: Ejecutar git-workspace-recon para validar entorno limpio. Tras confirmar, aislar el cambio con git-branch-manager (p. ej. rama de trabajo para el nuevo patrón).
  id: '0'
  name: Preparar entorno
- description: Generar UUID, carpeta bajo paths.patternsPath, spec.md y spec.json (ver §1 Definición).
  id: '1'
  name: Definición
- description: Durante la definición, consolidar hitos con git-save-snapshot. Ante fallo estructural, git-tactical-retreat.
  id: '2'
  name: Versionado atómico
- description: Verificar cumplimiento de patterns-contract (ver §2 Validación).
  id: '3'
  name: Validación
- description: git-sync-remote; git-create-pr enlazando el nuevo patrón (ruta bajo paths.patternsPath) en el cuerpo del Pull Request.
  id: '4'
  name: Finalizar
principles_ref: paths.principlesPath
process_id: create-pattern
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
spec_version: 2.0.0
---

# Proceso: Creación de Patrones (create-pattern)

Este documento define el **proceso** para añadir nuevos patrones de diseño y arquitectura a `SddIA/patterns/` (paths.patternsPath, Cúmulo).

## Objetivo

Estandarizar la incorporación de conocimiento sobre patrones, asegurando que cada entrada sea completa, legible y utilizable por los agentes (especialmente Arquitectos y Tekton).

## Alcance

- **Ubicación:** `SddIA/patterns/`
- **Estructura:** Carpeta nombrada con UUID.
- **Contenido:** `spec.md` (legible) y `spec.json` (metadatos).

## Fases del Proceso

**Fase 0 (Git):** Obligatorio **git-workspace-recon** antes de **git-branch-manager** para aislar el contexto. Cierre con **git-sync-remote** y **git-create-pr** cuando el flujo requiera integración vía PR.

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
