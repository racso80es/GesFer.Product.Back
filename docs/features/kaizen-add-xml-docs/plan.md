---
name: Implementation Plan
---
# Plan
1. Add `/// <returns>Estado actual del servicio</returns>` to `HealthController.Get`.
2. Add `[ProducesResponseType(StatusCodes.Status200OK)]` to `HealthController.Get`.