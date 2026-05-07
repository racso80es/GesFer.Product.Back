---
type: "clarify"
---
# Clarificaciones
- .NET 8 permite `ProducesResponseType<T>(int statusCode)`.
- Reemplazar `[ProducesResponseType(StatusCodes.Status401Unauthorized)]` por `[ProducesResponseType(StatusCodes.Status401Unauthorized)]` ya es correcto si no hay tipo. Pero si devuelven un error con JSON `{ message = "..." }`, pueden ser `[ProducesResponseType<object>(StatusCodes.Status401Unauthorized)]` o un modelo de error específico.
