---
id: Kaizen_2024_03_29_add_typeof_object_health_controller
title: Add typeof(object) to HealthController ProducesResponseType
priority: Low
date: 2024-03-29
status: DONE
---

# Add typeof(object) to HealthController ProducesResponseType

## Objective
Enforce standard formatting for `HealthController` response types by explicitly adding the type definition.

## Description
Currently, `HealthController.cs` has `[ProducesResponseType(StatusCodes.Status200OK)]`. It should explicitly specify the type being returned, which is an anonymous object, so we'll use `typeof(object)` for clarity and standard alignment: `[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]`.

## Scope
- Modify `src/Api/Controllers/HealthController.cs` to include `typeof(object)` in the `[ProducesResponseType]` attribute for the `Get` endpoint.