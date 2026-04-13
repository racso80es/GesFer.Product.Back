---
created: 2026-04-12
status: pending
---
# Add ProducesResponseType to DashboardController

The `DashboardController.cs` misses the `[ProducesResponseType]` attribute for its `GetStats` endpoint. This attribute should be added to improve API discoverability in Swagger.
