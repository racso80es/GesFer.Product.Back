---
commands:
  delete: Remove-Item (rm)
  list: Get-ChildItem (ls)
  read: Get-Content (cat)
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
rules:
- 'Shell: Use PowerShell (pwsh) exclusively. Do not use bash commands like ''ls -la'', ''rm -rf''.'
- 'Path Validation: Verify ''Get-Location'' (pwd) before creating files to ensure correct placement.'
- 'Destructive Actions: Deleting files requires explicit plan approval.'
- 'Build Artifacts: Do not edit artifacts (dist/, bin/, obj/) directly; edit source.'
skill_id: filesystem-ops
---

# Skill: File System Operations

**skill_id:** `filesystem-ops`

## Objetivo

Reglas seguras de manipulación de archivos en entorno Windows/PowerShell.

## Reglas

- **Shell:** Use PowerShell (pwsh) exclusively. Do not use bash commands like 'ls -la', 'rm -rf'.
- **Path Validation:** Verify 'Get-Location' (pwd) before creating files to ensure correct placement.
- **Destructive Actions:** Deleting files requires explicit plan approval.
- **Build Artifacts:** Do not edit artifacts (dist/, bin/, obj/) directly; edit source.

## Comandos equivalentes

- list: Get-ChildItem (ls)
- read: Get-Content (cat)
- delete: Remove-Item (rm)

## Alcance

Skill de definición únicamente. Consumido por agentes para operaciones de archivo en Windows.

---
*Definición en paths.skillsDefinitionPath/filesystem-ops/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
