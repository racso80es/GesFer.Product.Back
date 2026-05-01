# Norma: Operaciones Git y suite de skills

**Fuente:** SddIA/norms. Complementa [git-via-skills-or-process.md](./git-via-skills-or-process.md) (canal obligatorio para invocar Git). **En materia de prohibición de primitivos y bypass, prevalece esta norma.**

## Alcance

Define reglas duras para **cómo** deben resolverse las operaciones Git en el ecosistema SddIA cuando intervienen agentes, sin sustituir el catálogo de skills ni el contrato en `paths.skillsDefinitionPath`.

## Ley de Hierro — Prohibición de comandos primitivos

Queda **estrictamente prohibido** el uso de `invoke-command`, `powershell -Command` o **cualquier ejecución de terminal nativa** para operaciones de Git. Si una skill falla, el agente debe **reportar la incidencia** y **solicitar la refactorización de la cápsula** correspondiente, prohibiéndose explícitamente el **bypass manual** para garantizar la trazabilidad.

### Implicaciones

- No se admite sustituir una skill Git defectuosa por un comando `git` ad hoc en shell.
- La remediación pasa por **arreglar o extender la skill** (p. ej. `git-sync-remote`, `git-branch-manager`) y el binario Rust asociado, manteniendo salida JSON contractual.
- Cualquier excepción temporal debe quedar **documentada y auditada** por proceso (p. ej. feature / hotfix), no por ejecución oculta.

## Coherencia

- **AGENTS.md:** leyes GIT y COMANDOS.
- **Skills ejecutables:** cápsulas bajo `paths.skillCapsules` con binarios `.exe` en `bin/`.
- **Trazabilidad:** telemetría y registro por skill; el bypass manual destruye la cadena de evidencia exigible en grado S+.
