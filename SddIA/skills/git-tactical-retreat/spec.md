---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-tactical-retreat
owner: tekton-developer
parameters:
  cleanUntracked:
    default: false
    required: false
    type: boolean
  confirmDestructive:
    description: Obligatorio true si hardReset o cleanUntracked (Visión Zero)
    required: false
    type: boolean
  hardReset:
    default: false
    required: false
    type: boolean
  resetTarget:
    default: HEAD
    required: false
    type: string
  stash:
    default: false
    required: false
    type: boolean
  stashMessage:
    default: WIP
    required: false
    type: string
  workingDirectory:
    required: false
    type: string
rules:
- hardReset o cleanUntracked sin confirmDestructive true → error (código 3)
skill_id: git-tactical-retreat
spec_version: 1.0.0
status: Active
---

# Skill: git-tactical-retreat

Operaciones de “retirada”: stash opcional, `reset --hard` y/o `clean -fd`, con confirmación explícita para acciones destructivas.
