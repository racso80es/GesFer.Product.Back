---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-branch-manager
owner: tekton-developer
parameters:
  branchName:
    required: true
    type: string
  force:
    default: false
    required: false
    type: boolean
  operation:
    enum:
    - create
    - checkout
    - delete
    required: true
    type: string
  startPoint:
    description: Solo operation=create; default HEAD
    required: false
    type: string
  workingDirectory:
    required: false
    type: string
skill_id: git-branch-manager
spec_version: 1.0.0
status: Active
---

# Skill: git-branch-manager

Gestión básica de ramas: crear (`checkout -b`), cambiar de rama o borrar rama local.
