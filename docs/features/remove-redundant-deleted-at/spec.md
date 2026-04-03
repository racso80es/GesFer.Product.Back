---
type: spec
status: active
---
# Especificacion Técnica
Revisar los archivos `AuthService.cs` y `GetAllPostalCodesCommandHandler.cs` para remover las comprobaciones explicitas de `DeletedAt == null` del query expression tree (LINQ). Mantener el chain asegurando tipos validos y compilables.
