---
contract_ref: paths.processPath/process-contract.md
inputs:
  description: Descripción breve del fin concreto. Obligatorio.
  process_ref: Proceso que orquesta (feature, correccion-auditorias, etc.). Obligatorio.
  template_id: kebab-case. Obligatorio.
paths:
  featurePath_ref: paths.featurePath (Cúmulo)
  processPath_ref: paths.processPath (Cúmulo)
  templatesPath_ref: paths.templatesPath (Cúmulo)
persist_ref: paths.featurePath/create-template-<template-id>
phases:
- description: Ejecutar git-workspace-recon para validar entorno limpio. Tras confirmar, crear rama feat/create-template-<template-id> con git-branch-manager.
  id: '0'
  name: Preparar entorno
- description: Objetivos, spec de plantilla y entregable en paths.templatesPath (ver cuerpo).
  id: '1'
  name: Especificación y plantilla
- description: Consolidar hitos con git-save-snapshot durante redacción e integración. Ante fallo estructural, git-tactical-retreat.
  id: '2'
  name: Implementación documental y commits
- description: Acción validate según templates-contract.
  id: '3'
  name: Validar
- description: Cierre. git-sync-remote; git-create-pr con objectives/spec/validacion en el cuerpo del Pull Request. Acción finalize-process.
  id: '4'
  name: Finalizar
process_doc_ref: paths.processPath/create-template/
process_id: create-template
process_interface_compliance: 'Genera en carpeta de la tarea al menos un .md y un .json; entregable: carpeta en paths.templatesPath/<template-id>/ con spec.md y spec.json.'
related_actions:
- spec
- validate
- finalize-process
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
- documentation
spec_version: 2.0.0
templates_contract_ref: SddIA/templates/templates-contract.md
triggers:
- Crear nueva plantilla en paths.templatesPath
- Solicitud de creación de plantilla con template-id
---

# Proceso: Creación de plantillas (create-template)

Este documento define el **proceso de tarea** para crear una nueva plantilla (template) en el proyecto. Está ubicado en paths.processPath/create-template/ (Cúmulo). Las rutas de plantillas se obtienen de **Cúmulo** (paths.templatesPath).

**Interfaz de proceso:** Cumple la interfaz en Cúmulo (`process_interface`): la tarea de creación genera en la carpeta de la tarea (Cúmulo) al menos un **`.md`** (objectives.md, spec.md) y al menos un **`.json`** (spec.json). El **resultado** es la carpeta en **paths.templatesPath/<template-id>/** con spec.md y spec.json, cumpliendo SddIA/templates/templates-contract.md.

## Propósito

El proceso **create-template** define el procedimiento para incorporar una nueva plantilla al ecosistema de paths.templatesPath (Cúmulo): desde la definición del objetivo hasta la plantilla lista con spec.md y spec.json. Garantiza que cada plantilla cumpla SddIA/templates/templates-contract.md y que quede registrada en Cúmulo (paths.templatesPath). Las plantillas procedimentan el uso de procesos mediante configuraciones predefinidas con un fin concreto.

## Alcance del procedimiento

- **Documentación de la tarea:** Cúmulo (paths.featurePath/create-template-<template-id>/).
- **Entregable:** paths.templatesPath/<template-id>/ con spec.md y spec.json (y opcionalmente config.json).

Fases: 0 **git-workspace-recon** + **git-branch-manager** | 1 Objetivos y especificación de la plantilla | 2 Redactar spec.md y spec.json según templates-contract (**git-save-snapshot**; **git-tactical-retreat** si aplica) | 3 Validación | 4 Cierre (**git-sync-remote**, **git-create-pr**).

## Restricciones

- template_id en kebab-case. Rama feat/create-template-<template-id>. Windows 11, PowerShell 7+. Contrato templates (SddIA/templates/templates-contract.md) obligatorio. process_ref debe existir en paths.processPath.

## Referencias

- Contrato: SddIA/templates/templates-contract.md, templates-contract.md.
- Cúmulo: paths.templatesPath, paths.featurePath, paths.processPath.
- Proceso machine-readable: paths.processPath/create-template/spec.json.
