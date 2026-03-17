---
action_id: execution
contract_ref: actions-contract.md
flow_steps:
- Validación entrada
- Carga ítems
- Por cada ítem aplicar
- Persistencia
- Auditoría
inputs:
- implementation.md (carpeta tarea, Cúmulo)
outputs:
- execution.md con frontmatter YAML + cuerpo Markdown en carpeta de tarea (Cúmulo). Sin execution.json. Patrón: SddIA/norms/features-documentation-pattern.md.
patterns_ref: paths.patternsPath
principles_ref: paths.principlesPath
---

# Action: Execution

## Propósito

La acción **execution** (ejecución) aplica al código los cambios definidos en el documento de implementación. Transforma los ítems del `implementation.md` en modificaciones reales en el repositorio: crear, modificar o eliminar archivos según la propuesta de cada ítem. No define qué cambiar —eso ya está en la fase de implementation— sino que **ejecuta** esos cambios de forma ordenada, realizando tareas agrupadas en commits y registra el resultado para trazabilidad.

## Principio

- **Entrada única:** El documento de implementación (o su representación JSON) es la única fuente de lo que se debe hacer.
- **Orden y dependencias:** Se respetan el orden sugerido y las dependencias entre ítems (p. ej. crear entidad antes de usarla en un handler).
- **Trazabilidad:** Toda aplicación de cambio se registra en un artefacto de salida para validación y auditoría.

## Entradas

- **Documento de implementación** (obligatorio): ruta a implementation.md en la carpeta de la tarea (Cúmulo). Contiene los ítems con Id, Acción, Ruta, Ubicación, Propuesta y Dependencias (frontmatter YAML + cuerpo Markdown).

## Salidas

- **Registro de ejecución:** execution.md en la carpeta de la tarea (Cúmulo) (frontmatter YAML + cuerpo Markdown).
  - Debe incluir: por cada ítem aplicado, id del ítem, ruta del archivo, acción realizada (Crear | Modificar | Eliminar), estado (OK | Error), mensaje opcional y timestamp.
  - En caso de error en un ítem: registro del fallo y decisión (detener o continuar según criterio del agente).

## Flujo de ejecución (propuesto)

1. **Validación de entrada:** Comprobar que `implementation.md` exista y sea válido.
2. **Carga del plan de ítems:** Ordenar ítems según dependencias y orden sugerido.
3. **Por cada ítem:**
   - Resolver la ruta del archivo (existente para Modificar/Eliminar; no existente para Crear).
   - Aplicar la propuesta (crear archivo, editar bloque, eliminar bloque/archivo).
   - Registrar el resultado en la estructura de salida.
4. **Persistencia:** Escribir execution.md en la carpeta de la tarea (Cúmulo) con el registro completo (frontmatter YAML + cuerpo Markdown).
5. **Auditoría:** Opcionalmente registrar en paths.auditsPath + paths.accessLogFile que se ejecutó la acción execution para la feature.

## Implementación técnica (opcional)

Puede ejecutarse mediante scripts o el agente Tekton que consuma el documento de implementación. Parámetros típicos:

- `--implementation`: ruta a `implementation.md`.
- `--persist`: ruta base de la feature (ej. paths.featurePath/<nombre_feature>/); donde se escribirá `execution.md`.
- `--token`: (opcional) token de auditoría.

## Integración con agentes

- **Tekton Developer (agente ejecutor):** Es el responsable de ejecutar esta acción. Aplica cada ítem del documento de implementación al código, respeta el contrato de comandos (invoke-command) y las restricciones (no master, commits atómicos, Kaizen). Consume `implementation.md` y produce `execution.md`.
- **Arquitecto / QA Judge:** Pueden revisar `execution.md` para comprobar que todos los ítems se aplicaron y que no hubo desviaciones respecto al documento de implementación.

## Agente responsable (referencia para definición de agente)

| Concepto | Descripción |
| :--- | :--- |
| **Id sugerido** | `tekton-developer` (ya existente). La acción execution es responsabilidad del mismo agente que implementa el plan. |
| **Rol** | Ejecutor: traducir documento de implementación en cambios de código y registro en `execution.md`. |
| **Skills necesarios** | `dotnet-development`, `filesystem-ops`, `git-operations`, `invoke-command`. |
| **Restricciones** | No trabajar en `master`; commits atómicos; todo comando de sistema vía invoke-command. |

No se requiere un agente nuevo: **Tekton Developer** asume la fase de ejecución. Si en el futuro se desea separar "planificador de cambios" de "aplicador", podría definirse un agente **Execution Runner** que solo aplique implementation.md y genere `execution.md`, invocado por Tekton.

## Estándares de calidad

- **Exhaustividad:** Todo ítem del documento de implementación debe aparecer en `execution.md` con estado OK o Error.
- **Determinismo:** Mismo IMPL en mismo contexto debe producir el mismo conjunto de cambios (salvo fallos de entorno).
- **Trazabilidad:** El PR y la validación posterior pueden apoyarse en `execution.md` para saber qué se tocó.

## Dependencias con otras acciones

- **implementation** (acción previa): Produce la entrada (`implementation.md`).
- **validate** (acción siguiente): Puede usar `execution.md` para saber qué archivos/cambios comprobar.

---
*Documento de definición de la acción Execution. Corresponde a la fase 6 del procedimiento feature (aplicar el plan al código).*
