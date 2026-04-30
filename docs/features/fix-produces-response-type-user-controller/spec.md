---
type: feature
status: active
---
# Specification
Refactor `UserController.cs` to replace `[ProducesResponseType(typeof(T), status)]` with `[ProducesResponseType<T>(status)]`.