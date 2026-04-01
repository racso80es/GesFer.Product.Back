# Proceso: Creación de Principio (create-principle)

Este documento define el **proceso** para añadir nuevos principios técnicos a `paths.principlesPath` (SddIA/principles/).

## Objetivo

Estandarizar la incorporación de principios (guías técnicas, normas de nomenclatura, etc.), asegurando que cada entrada cumpla el contrato de principios y sea utilizable por Arquitecto, Tekton, Cúmulo y, si aplica, por la validación pre-PR.

## Alcance

- **Ubicación:** paths.principlesPath (Cúmulo) = SddIA/principles/
- **Estructura:** Carpeta `<principle-id>` en kebab-case.
- **Contenido:** spec.md (legible) y spec.json (metadatos). Ambos obligatorios según principles-contract.

## Fases del Proceso

### 1. Definición

1. **Elegir principle_id:** Identificador en kebab-case (ej. `nomenclatura`, `norma-nomenclatura`).
2. **Crear carpeta:** paths.principlesPath/\<principle-id\>/.
3. **Redactar spec.md:**
   - Título, categoría, resumen y objetivo.
   - Contenido del principio (reglas, criterios).
   - Aplicación para Arquitecto y Tekton (y, si aplica, para Cúmulo o QA Judge).
   - Referencias.
   - Idioma: español (es-ES).
   - Pie: *Definición en paths.principlesPath/\<principle-id\>/ (contrato paths.principlesPath/principles-contract.md).*
4. **Crear spec.json:**
   - **id:** UUID v4 del principio.
   - **principle_id:** mismo que el nombre de la carpeta (kebab-case).
   - **title:** Título legible.
   - **category:** Categoría (Clean Code, Principios SOLID, Normas SddIA, etc.).
   - **tags:** Lista de etiquetas.
   - **metadata:** difficulty (Beginner | Intermediate | Advanced), status (Draft | Published | Deprecated).
   - **contract_ref:** paths.principlesPath/principles-contract.json.
   - **blocking_for_pr:** (opcional) true si el PR no debe aprobarse si no se cumple este principio. La acción validate debe incluir un check para este principio.
   - **defined_by_agent:** (opcional) Agente que define/custodia el principio (ej. cumulo).

### 2. Validación

1. Verificar que spec.json cumple SddIA/principles/principles-contract.json.
2. Asegurar que spec.md tiene formato claro y contenido útil.
3. Si **blocking_for_pr** es true: confirmar que la acción validate (paths.actionsPath/validate/) incluye la comprobación de este principio y que el resultado es bloqueante (global fail / blocking).

## Principio bloqueante para PR

Si el principio debe impedir que un PR pase cuando no se cumple (ej. norma de nomenclatura):

- En spec.json: `"blocking_for_pr": true`.
- En spec.md: indicar explícitamente que *Un PR no debe aprobarse si no se cumple esta norma*.
- La acción **validate** debe tener un check (ej. `nomenclatura` o `principle_<principle-id>`) que falle cuando se incumpla y que figure como obligatorio/blocking en validacion.json.

## Artefactos

- paths.principlesPath/\<principle-id\>/spec.md
- paths.principlesPath/\<principle-id\>/spec.json

## Referencia

- Contrato: SddIA/principles/principles-contract.json y principles-contract.md.
- Cúmulo: paths.principlesPath.
