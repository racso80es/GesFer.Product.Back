---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.git-workspace-recon
owner: tekton-developer
parameters:
  includeRawPorcelain:
    default: true
    required: false
    type: boolean
  workingDirectory:
    required: false
    type: string
phases:
- description: rev-parse, branch, status --porcelain, remote -v, ahead/behind si @{u} existe.
  id: recon
  name: Reconocimiento workspace
related_agents:
- tekton-developer
skill_id: git-workspace-recon
spec_version: 1.0.0
status: Active
---

# Skill: git-workspace-recon

Arsenal Git: inspección de estado del repositorio sin mutar el árbol de trabajo (salvo lectura Git).

Ver cápsula `paths.skillCapsules.git-workspace-recon` para ejemplos JSON y launcher.
