---
blocking_for_pr: true
category: Normas SddIA
contract_ref: paths.principlesPath/principles-contract.md
defined_by_agent: cumulo
id: d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f80
metadata:
  difficulty: Beginner
  status: Published
principle_id: nomenclatura
tags:
- Nomenclatura
- Convenciones
- Cúmulo
- PR
- Validación
---

# Norma de Nomenclatura

**principle_id:** `nomenclatura`  
**Definido por:** Cúmulo (agente responsable de SSOT y estructura en SddIA).

## Resumen

Este principio establece la nomenclatura obligatoria para ramas, commits, identificadores en SddIA (principle_id, action_id, process_id, token_id, etc.) y convenciones de nombres en código y documentación. **Un PR no debe aprobarse si no se cumple esta norma.** La acción validate debe incluir un check de nomenclatura y bloquear el PR cuando falle.

## Objetivo

Mantener coherencia y trazabilidad en todo el repositorio y en los artefactos de SddIA, de modo que nombres de ramas, commits, carpetas y referencias sigan un mismo criterio y sean validables de forma automática.

## Reglas de nomenclatura (Cúmulo)

### Ramas Git

- **Ramas de funcionalidad:** `feat/<nombre_feature>` — nombre en kebab-case, sin espacios.
- **Ramas de corrección:** `fix/<nombre_fix>` — nombre en kebab-case.
- **Ramas de refactorización:** `feat/refactorization-<nombre_refactor>`.
- **Prohibido:** commits directos en `master`/`main`; ramas sin prefijo `feat/` o `fix/` para trabajo de feature/fix.

### Commits

- **Formato convencional:** `<tipo>(<alcance>): <descripción>`.
- **Tipos habituales:** feat, fix, refactor, docs, chore, test.
- **Descripción:** en minúsculas (salvo nombres propios), sin punto final en el asunto.

### SddIA (paths.principlesPath, paths.actionsPath, paths.processPath, paths.tokensPath, etc.)

- **Identificadores de entidad:** siempre **kebab-case** (ej. `principle_id`, `action_id`, `process_id`, `token_id`, `skill_id`, `tool_id`).
- **Carpetas:** nombre = identificador en kebab-case (ej. `regla-del-boy-scout`, `nomenclatura`, `karma2-token`).
- **Rutas:** obtenidas solo desde Cúmulo (paths.\*); no usar rutas literales en la documentación de comportamiento.

### Código (C#, TypeScript)

- **Tipos, clases, interfaces:** PascalCase.
- **Métodos y propiedades públicas:** PascalCase.
- **Variables locales, parámetros:** camelCase.
- **Constantes:** PascalCase o UPPER_SNAKE_CASE según convención del proyecto.

### Documentación (docs/, SddIA/)

- **Archivos y carpetas:** kebab-case cuando sean identificadores o slugs (ej. `paths.featurePath/<nombre_feature>/`).
- **Nombres de documentos:** pueden usar MAYÚSCULAS para destacar (ej. EVOLUTION_LOG.md, SPEC-*.md) cuando así lo defina la convención del proyecto.

## Aplicación para Cúmulo

- Cúmulo es el agente que **define y custodia** esta norma.
- Mantener este principio actualizado cuando se añadan nuevas convenciones (paths, procesos, acciones).
- Asegurar que el contrato de principios y la acción validate referencian este principio como bloqueante para PR.

## Aplicación para Arquitecto y Tekton

- Al crear ramas, commits, carpetas en SddIA o docs: respetar kebab-case para identificadores y nombres de feature/fix.
- Al generar especificaciones o planes: usar únicamente identificadores que cumplan esta norma.

## Aplicación para QA Judge / Validate

- La acción **validate** debe incluir un check **nomenclatura** (o **principle_nomenclatura**) que compruebe, como mínimo:
  - Nombre de rama actual: prefijo `feat/` o `fix/` y resto en kebab-case.
  - Mensajes de commit en el diff: formato convencional cuando aplique.
- **Implementación ejecutable:** `scripts/validate-nomenclatura.ps1`. Invocar desde el flujo de validate (o desde CI). Parámetros: `-BaseBranch main`, `-CheckCommits` (opcional). Salida JSON con check, result (pass/fail), message, detail. Exit code 0 = pass, 1 = fail.
- Si el check falla: **global: fail**, **blocking: true** en validacion.json. El PR no debe aprobarse hasta que se cumpla la norma.

## Referencias

- AGENTS.md (ley GIT, CONSULTA DOCUMENTAL).
- SddIA/norms/paths-via-cumulo.md.
- Proceso create-principle: paths.processPath/create-principle/.

---
*Definición en paths.principlesPath/nomenclatura/ (contrato paths.principlesPath/principles-contract.md). Principio bloqueante para PR.*
