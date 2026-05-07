---
type: "spec"
task: "Kaizen_2026_05_03_refactor_produces_response_type_profile_controller.md"
---
# Especificaciones

Se busca usar la sintaxis moderna genérica introducida en ASP.NET Core:
`[ProducesResponseType<T>(StatusCodes.Status...)]` en vez de `typeof(T)`.
También revisaremos los `[ProducesResponseType(StatusCodes.Status404NotFound)]` y similares que no devuelven body o devuelven objeto anónimo. De hecho, si devuelven un objeto anónimo `new { message = ... }`, podemos usar `[ProducesResponseType<object>(StatusCodes.Status404NotFound)]` u omitir el tipo si es vacío, pero mantener consistencia.
