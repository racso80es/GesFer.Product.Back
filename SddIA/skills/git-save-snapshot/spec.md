---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-save-snapshot
owner: tekton-developer
parameters:
  allowEmpty:
    default: false
    required: false
    type: boolean
  commitMessage:
    required: true
    type: string
  paths:
    description: Si stageAll=false, lista de rutas para git add
    required: false
    type: array
  stageAll:
    default: true
    required: false
    type: boolean
  workingDirectory:
    required: false
    type: string
skill_id: git-save-snapshot
spec_version: 1.0.0
status: Active
---

# Skill: git-save-snapshot

Equivalente operativo a un commit rápido: añade cambios y crea commit con mensaje explícito.
