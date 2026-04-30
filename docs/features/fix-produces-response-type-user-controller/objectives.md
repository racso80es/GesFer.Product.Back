---
type: feature
status: active
---
# Objectives
Remove the explicit `typeof` specification from `[ProducesResponseType]` attributes in `UserController.cs` to use the modern `[ProducesResponseType<T>]` generic attribute or inferred types, matching standard conventions.