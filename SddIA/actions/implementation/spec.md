---
action_id: implementation
contract_ref: actions-contract.md
flow_steps:
- Validación
- Análisis plan
- Resolución rutas
- Unificación
- Persistencia
- Auditoría
inputs:
- Plan (obligatorio)
- SPEC (opcional)
- Clarificaciones (opcional)
outputs:
- implementation.md con frontmatter YAML + cuerpo Markdown en carpeta de tarea (Cúmulo). Sin implementation.json. Patrón: SddIA/norms/features-documentation-pattern.md.
patterns_ref: paths.patternsPath
principles_ref: paths.principlesPath
---

# Action: Implementation

## Propósito
La acción **implementation** (implementación) no realiza cambios en el código. Su objetivo es **indicar todos los touchpoints en el código** derivados de un plan (y/o SPEC) y **unificar esa información en un único documento de implementación**. Ese documento sirve como guía ejecutable: cada ítem indica dónde actuar (ruta de archivo, ubicación dentro del archivo) y qué proponer (cambio, creación o eliminación). El implementador (humano o agente) usa ese documento como fuente única para aplicar los cambios.

## Principio
- **No modifica el código:** la acción solo produce documentación.
- **Indica todo:** cada cambio derivado del plan se traduce en al menos un ítem con ruta + ubicación + propuesta.
- **Unifica:** un solo documento de implementación (p. ej. `IMPL-{FeatureId}.md`) agrupa todos los ítems, ordenados por fase/tarea o por archivo.

## Entradas
- **Plan de implementación** (obligatorio): ruta al documento PLAN (p. ej. `.docs/feature/article-family/PLAN-ARTICLE-FAMILY-CRUD.md`).
- **SPEC** (opcional): ruta a la SPEC asociada para enriquecer contexto o criterios.
- **Clarificaciones** (opcional): ruta al documento de clarificaciones si existe.

## Salida
- **Documento de implementación** (único): nuevo archivo que consolida todos los touchpoints.
  - **Nombre sugerido:** `IMPL-{FeatureId}.md` o `IMPL-{SpecName}.md` (ej. `IMPL-ARTICLE-FAMILY-CRUD.md`).
  - **Ubicación sugerida:** misma carpeta que el PLAN (ej. `.docs/feature/article-family/`) o `openspecs/implementation/` según convención del proyecto.
  - **Estructura mínima del documento generado:**
    1. **Cabecera:** referencia al PLAN, SPEC, rama.
    2. **Ítems de implementación:** por cada cambio a realizar:
       - **Id** (ej. 1.1, 2.4): referencia a la tarea del plan.
       - **Acción:** Crear | Modificar | Eliminar.
       - **Ruta:** archivo afectado (ruta desde la raíz del repo).
       - **Ubicación:** descripción de dónde dentro del archivo (clase, método, línea aproximada o bloque lógico).
       - **Propuesta:** texto o pseudocódigo que describe el cambio (qué añadir, qué quitar, qué sustituir).
       - **Dependencias:** ítems que deben estar hechos antes (opcional).
    3. **Resumen por archivo** (opcional): lista de archivos tocados con los ids de ítem que les afectan, para facilitar trabajo por archivo.
    4. **Orden sugerido:** si el orden de aplicación importa (p. ej. migraciones, seeds), indicarlo brevemente.

## Flujo de ejecución (propuesto)
1. **Validación:** Comprobar que el plan (y opcionalmente SPEC) existan y sean legibles.
2. **Análisis del plan:** Recorrer cada tarea del PLAN y derivar uno o más touchpoints (archivo + ubicación + propuesta).
3. **Resolución de rutas:** Usar rutas canónicas del repositorio (p. ej. `src/Product/Back/...`) y verificar que los archivos existan cuando la acción sea "Modificar" o "Eliminar".
4. **Unificación:** Generar el documento de implementación con todos los ítems, agrupados por fase/tarea o por archivo (según plantilla).
5. **Persistencia:** Guardar el documento en la ruta acordada.
6. **Auditoría:** Registrar la generación en paths.auditsPath + paths.accessLogFile (opcional pero recomendado).

## Implementación técnica (opcional)

Puede implementarse mediante scripts o el agente que genere el documento IMPL a partir del plan. Parámetros típicos:

- `--plan`: ruta al documento PLAN (.md).
- `--spec`: (opcional) ruta a la SPEC.
- `--output`: (opcional) ruta del documento de implementación generado; si no se indica, derivar del nombre del plan (ej. mismo directorio, nombre `IMPL-ARTICLE-FAMILY-CRUD.md`).
- `--token`: (opcional) token de auditoría para registrar la acción.

## Integración con agentes
- **Tekton Developer:** Consume el documento de implementación generado para aplicar los cambios en el código en el orden indicado.
- **Arquitecto / QA Judge:** Pueden validar que los cambios aplicados coincidan con los ítems del documento.

## Estándares de calidad
- **Exhaustividad:** Toda tarea del plan que implique tocar código debe tener al menos un ítem en el documento de implementación.
- **Unicidad:** Una sola fuente de verdad para "qué cambiar y dónde" en esta feature (el documento IMPL).
- **Trazabilidad:** Cada ítem del IMPL referencia la tarea del PLAN (y opcionalmente la SPEC).

## Ejemplo de ítem en el documento generado
```markdown
### 2.11.1 – Modificar: albarán de venta
- **Id:** 2.11
- **Acción:** Modificar
- **Ruta:** `src/Product/Back/application/Handlers/SalesDeliveryNote/CreateSalesDeliveryNoteCommandHandler.cs`
- **Ubicación:** Consulta de artículos (Include) y cálculo de IVA (uso de Family).
- **Propuesta:**
  - Sustituir `.Include(a => a.Family)` por `.Include(a => a.ArticleFamily).ThenInclude(af => af.TaxType)`.
  - Sustituir `article.Family.IvaPercentage` por `article.ArticleFamily.TaxType.Value`.
- **Dependencias:** 2.4 (Article con ArticleFamilyId, sin Family).
```

---
*Documento de definición de la acción Implementation. La funcionalidad básica es indicar touchpoints en el código y unificarlos en un único documento para implementación; no aplicar cambios al código.*
