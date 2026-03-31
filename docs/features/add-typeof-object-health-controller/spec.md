---
title: Spec
status: DONE
---
# Spec
Modify `src/Api/Controllers/HealthController.cs` to explicitly specify the type being returned, which is an anonymous object, so we'll use `typeof(object)` for clarity and standard alignment: `[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]`.