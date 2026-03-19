---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
process: feature
branch: feat/kaizen-skills-tools-rust-json-contract
contract_ref: SddIA/norms/features-documentation-pattern.md
---

# Objetivos

## Objetivo

Realizar un **Kaizen** (mejora continua) sobre scripts, skills y tools para cumplir con los contratos SddIA, que establecen:

1. **Implementación en Rust:** Los ejecutables invocables por IA no han de ser `.ps1`, sino binarios `.exe` compilados desde Rust.
2. **JSON entrada/salida:** Cumplimiento de contrato JSON de entrada y salida para interoperabilidad machine-readable.

## Alcance

- **Skills:** paths.skillCapsules (iniciar-rama, finalizar-git, invoke-command, invoke-commit).
- **Tools:** paths.toolCapsules (invoke-mysql-seeds, prepare-full-env, run-tests-local, postman-mcp-validation, start-api).
- **Contratos:** SddIA/tools/tools-contract.md, SddIA/skills/skills-contract.md, SddIA/constitution.json (skills_tools_implementation: rust).
- **Rutas:** paths.skillsRustPath (scripts/skills-rs/), paths.toolsRustPath (scripts/tools-rs/).

## Ley aplicada

- **Ley COMANDOS:** No ejecutar comandos directamente; toda ejecución vía skill, herramienta, acción o proceso (SddIA/norms/commands-via-skills-or-tools.md).
- **Ley GIT:** No commits en master; trabajo en rama feat/ con documentación en paths.featurePath.
- **Ley COMPILACIÓN:** Código roto inaceptable; verificar localmente tras cambios.

## Referencias canónicas

- SddIA/constitution.json → configuration.skills_tools_implementation: "rust"
- SddIA/tools/tools-contract.md → prohibited_formats: .ps1, .bat, .sh; output JSON obligatorio
- SddIA/skills/skills-contract.md → prohibited_formats: .ps1, .bat, .sh; implementación Rust obligatoria
