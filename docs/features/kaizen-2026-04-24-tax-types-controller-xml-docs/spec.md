---
feature_name: kaizen-2026-04-24-tax-types-controller-xml-docs
created: 2026-04-24
base: main
---
# Especificación
Los controladores deben tener documentación XML completa (`<summary>`, `<returns>`) y declarar explícitamente los esquemas de respuesta HTTP validados usando `[ProducesResponseType]`. Específicamente, se requiere añadir:
- `StatusCodes.Status401Unauthorized` en todos los endpoints debido al atributo `[Authorize]`.
- Esquemas de retorno adecuados para las peticiones de éxito (`typeof(List<...>)`, `typeof(...)`, `typeof(Guid)` y sin contenido para Delete/Update).
- Códigos de error adecuados (`Status400BadRequest`, `Status404NotFound`).
