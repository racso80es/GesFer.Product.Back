---
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
rules:
- 'Format: All documentation must be in Markdown (.md).'
- 'Hierarchy: No docs in root (except AGENTS.md). Use categories: [GOB], [EVO], [AUD], [TEC], [OPS], [SPEC].'
- 'Language: Spanish (es-ES) exclusively.'
- 'Evolution Log: Update paths.evolutionPath + paths.evolutionLogFile (Cúmulo) for architectural changes.'
- 'Legacy: Move obsolete docs to paths (Cúmulo) para legacy si existe; si no, convención del proyecto instead of deleting them immediately if unsure.'
skill_id: documentation
---

# Skill: Documentation & Knowledge Management

**skill_id:** `documentation`

## Objetivo

Estándares para mantener la Single Source of Truth (SSOT) en documentación y conocimiento del proyecto.

## Reglas

- **Format:** All documentation must be in Markdown (.md).
- **Hierarchy:** No docs in root (except AGENTS.md). Use categories: [GOB], [EVO], [AUD], [TEC], [OPS], [SPEC].
- **Language:** Spanish (es-ES) exclusively.
- **Evolution Log:** Update paths.evolutionPath + paths.evolutionLogFile (Cúmulo) for architectural changes.
- **Legacy:** Move obsolete docs to ruta legacy según Cúmulo o convención del proyecto instead of deleting them immediately if unsure.

## Alcance

Skill de definición únicamente (sin implementación ejecutable en cápsula). Consumido por agentes y acciones para aplicar estándares de documentación.

---
*Definición en paths.skillsDefinitionPath/documentation/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
