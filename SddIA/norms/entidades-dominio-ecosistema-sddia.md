# Norma: Entidades de dominio (ecosistema SddIA)

**Fuente:** SddIA/norms. Aplicable a todo agente o IA que opere en el repositorio y a la gobernanza de artefactos SddIA.

## Definición

Se denominan **entidades de dominio** o **entidades del ecosistema SddIA** a aquellas que **integran el ítem o contrato de Token** (paths.tokensPath; Cúmulo). Es decir, son las entidades cuyos contratos o definiciones exigen operar bajo un contexto de Token (p. ej. Karma2Token) para trazabilidad y seguridad.

**Incluyen, según sus contratos:**

- **Skills** (paths.skillsDefinitionPath, paths.skillCapsules) — contrato: paths.skillsDefinitionPath/skills-contract.md (required_token: Karma2Token).
- **Tools** (paths.toolsDefinitionPath, paths.toolCapsules) — contrato: paths.toolsDefinitionPath/tools-contract.md (required_token: Karma2Token).
- **Actions** (paths.actionsPath) — contrato: actions-contract.md (required_token: Karma2Token).
- **Process** (paths.processPath) — contrato: paths.processPath/process-contract.md (required_token: Karma2Token).
- **Patterns** (paths.patternsPath) — contrato: patterns-contract.md (required_token: Karma2Token).
- **Principles** (paths.principlesPath) — contrato: principles-contract.md (required_token: Karma2Token).
- **Templates** (paths.templatesPath) — contrato: templates-contract.md (required_token: Karma2Token).
- **Tokens** (paths.tokensPath) — contrato: paths.tokensPath/tokens-contract.json (definición de los propios tokens).

## Obligaciones de estructura y sincronidad

Todas las **entidades de dominio** han de:

1. **Respetar la estructura canónica:** archivo `.md` con frontmatter YAML (metadatos) + cuerpo Markdown. Excepción: tokens (paths.tokensPath) pueden ser JSON-only por tokens-contract.
2. **Validación:** esquema YAML nativo en acción validate. El check `sddia_frontmatter_valid` valida frontmatter YAML en `.md` de entidades.

La validación puede realizarse mediante el check opcional `sddia_frontmatter_valid` (acción validate) cuando el diff toque paths.skillsDefinitionPath, paths.processPath, paths.featurePath, paths.fixPath u otras rutas de entidades de dominio. La acción sddia-difusion incluye entre sus criterios la validez del frontmatter en las definiciones que se difunden.

**Documentación de tareas (paths.featurePath, paths.fixPath):** Sigue el mismo patrón: un `.md` por acción con frontmatter YAML + Markdown; sin `.json` separados. Norma: SddIA/norms/features-documentation-pattern.md.

## Referencias

- **Token (Karma2Token):** paths.tokensPath; SddIA/tokens/karma2-token/spec.json.
- **Contrato de tokens:** paths.tokensPath/tokens-contract.json (Cúmulo).
- **Frontmatter válido:** SddIA/actions/validate (optional_checks.sddia_frontmatter_valid); SddIA/actions/sddia-difusion (criterios de aceptación).

---
*Definición canónica de entidades de dominio para gobernanza SddIA. Ref: refactorization-sincronidad-md-json.*
