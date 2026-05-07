---
type: "objectives"
task: "Kaizen_2026_05_03_refactor_produces_response_type_profile_controller.md"
---
# Objetivos

- Refactorizar todos los controladores de la API para usar la sintaxis genérica `[ProducesResponseType<T>(statusCode)]` en lugar de `[ProducesResponseType(typeof(T), statusCode)]` o `[ProducesResponseType(statusCode)]` cuando retornan datos u objetos anónimos (si es posible tiparlos), o mantenerlos con la sintaxis genérica tipada.
- Aplicar esto a todos los StatusCode definidos en los controladores que apliquen (400, 401, 404, 204, etc.) donde la versión genérica de .NET 8 es `[ProducesResponseType<TResponse>(statusCode)]` o `[ProducesResponseType(statusCode)]` sin el typeof. Para los de error, si no hay un tipo de dato esperado, se mantiene el atributo no genérico si solo especifica el StatusCode. O si devuelven ProblemDetails, tiparlo. Vamos a actualizar los `[ProducesResponseType(StatusCodes...)]` a genéricos si es posible, pero el requerimiento es principalmente evitar la sintaxis antigua.
- Mover tarea de `docs/TASKS/KAIZEN/` a `docs/TASKS/ACTIVE/` y luego a `docs/TASKS/DONE/`.
