# Plantillas (SddIA/templates)

**Fuente de rutas:** Cúmulo (paths.templatesPath).

Las **plantillas** son configuraciones predefinidas que procedimentan el uso de un proceso con un fin concreto. Cada plantilla declara el proceso que orquesta, los orígenes de entrada (rutas Cúmulo, parciales o totales) y las acciones del ciclo de vida. Cumplen el contrato en `templates-contract.json` y `templates-contract.md`.

## Creación de plantillas

Proceso: **create-template** (paths.processPath/create-template/). Rama `feat/create-template-<template-id>`. Documentación de la tarea en paths.featurePath/create-template-<template-id>/ (Cúmulo). Entregable: carpeta en paths.templatesPath/<template-id>/ con spec.md y spec.json.

## Plantillas disponibles

| template_id | Descripción | Proceso |
|-------------|-------------|---------|
| correccion-auditorias-feature | Feature de corrección según auditorías: origen en paths.auditsPath (o ruta/fichero indicado); análisis con agentes y ciclo feature (objectives, spec, clarify, planning, etc.). | correccion-auditorias / feature |

Detalle de cada plantilla: paths.templatesPath/<template-id>/ (spec.md, spec.json).
