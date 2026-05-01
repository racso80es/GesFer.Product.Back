---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-create-pr
owner: tekton-developer
parameters:
  base:
    required: false
    type: string
  body:
    required: false
    type: string
  draft:
    default: false
    required: false
    type: boolean
  head:
    required: false
    type: string
  title:
    required: true
    type: string
  workingDirectory:
    required: false
    type: string
rules:
- Requiere GitHub CLI (gh) en PATH y sesión autenticada
skill_id: git-create-pr
spec_version: 1.0.0
status: Active
---

# Skill: git-create-pr

Crea una pull request invocando `gh pr create` con los parámetros expuestos en JSON.
