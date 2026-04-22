---
title: "Documentar Contexto User - Specifications"
---

# Specs
This audit defines the search perimeter in the legacy code. We will inspect:
- `src/domain/Entities/User.cs` and related entities.
- EF Core configurations like `UserConfiguration.cs`.
- CQRS handlers in `src/application/Handlers/User/` to extract business rules.
- `UserController.cs` for endpoint mappings.
- MasterDataSeeders for seeds.
- DTOs in `src/application/DTOs/User/`.

The deliverables are the 6 obligatory vectors defined in the objective.
