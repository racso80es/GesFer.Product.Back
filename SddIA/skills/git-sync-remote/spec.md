---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-sync-remote
owner: tekton-developer
parameters:
  branch:
    required: false
    type: string
  operation:
    enum:
    - fetch
    - pull
    - push
    required: true
    type: string
  remote:
    default: origin
    required: false
    type: string
  workingDirectory:
    required: false
    type: string
skill_id: git-sync-remote
spec_version: 1.0.0
status: Active
---

# Skill: git-sync-remote

Sincronización con remoto: `fetch`, `pull` o `push` (sin ramas forzadas adicionales más allá de las opciones expuestas).
