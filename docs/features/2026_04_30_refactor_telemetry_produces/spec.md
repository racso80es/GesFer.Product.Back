---
type: feature
status: open
priority: medium
---
# Specification
The `[ProducesResponseType(StatusCodes.Status200OK)]` and `[ProducesResponseType(StatusCodes.Status400BadRequest)]` attributes in `src/Api/Controllers/TelemetryController.cs` must be updated to `[ProducesResponseType<object>(StatusCodes.Status200OK)]` and `[ProducesResponseType<object>(StatusCodes.Status400BadRequest)]`.
