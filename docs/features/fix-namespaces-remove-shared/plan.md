---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
phases:
  - id: '1'
    name: Merge con main
  - id: '2'
    name: Refactor referencias residuales
  - id: '3'
    name: Eliminar usings duplicados
  - id: '4'
    name: Validación
spec_ref: docs/features/fix-namespaces-remove-shared/spec.md
clarify_ref: docs/features/fix-namespaces-remove-shared/clarify.md
---

# Plan: fix-namespaces-remove-shared

## Fase 1: Merge con origin/main

| Tarea | Descripción | Criterio |
|-------|-------------|----------|
| 1.1 | Ejecutar `git merge origin/main` | Merge iniciado |
| 1.2 | Resolver 7 archivos en conflicto adoptando versión de main en usings | Sin conflictos pendientes |
| 1.3 | Completar merge (commit) | Rama actualizada con main |

**Archivos en conflicto:** SetupService.cs, CustomerConfiguration.cs, SupplierConfiguration.cs, UserConfiguration.cs, JsonDataSeeder.cs, MasterDataSeeder.cs, JsonDataSeederTests.cs.

---

## Fase 2: Refactor referencias residuales

| Tarea | Archivo | Cambio |
|-------|---------|--------|
| 2.1 | SetupService.cs | `GesFer.Infrastructure.Services.MasterDataSeeder` → `GesFer.Product.Back.Infrastructure.Services.MasterDataSeeder` |
| 2.2 | JsonDataSeederTests.cs | `using SeedResult = GesFer.Infrastructure.Services.SeedResult` → `GesFer.Product.Back.Infrastructure.Services.SeedResult` |
| 2.3 | TestDataSeeder.cs | `GesFer.Domain.Services.SensitiveDataSanitizer` → `GesFer.Product.Back.Domain.Services.SensitiveDataSanitizer` |
| 2.4 | DatabaseFixture.cs | `GesFer.Api.Program` → `GesFer.Product.Back.Api.Program` |
| 2.5 | Program.cs | `GesFer.Api.Swagger.DefaultValueSchemaFilter` → `GesFer.Product.Back.Api.Swagger.DefaultValueSchemaFilter` |
| 2.6 | Program.cs | `"Application", "GesFer.Api"` → `"Application", "GesFer.Product.Back.Api"` (enrich) |
| 2.7 | setup-database.ps1, Migrations/README.md | Rutas GesFer.Infrastructure.csproj (verificar coherencia) |
| 2.8 | Dockerfile, Dockerfile.test | Rutas de proyectos (verificar estructura actual) |

---

## Fase 3: Eliminar usings duplicados

| Tarea | Archivo | Cambio |
|-------|---------|--------|
| 3.1 | SetupService.cs | Eliminar `using GesFer.Product.Back.Domain.Entities;` duplicado |
| 3.2 | CustomerConfiguration.cs | Eliminar `using GesFer.Product.Back.Domain.Entities;` duplicado |

---

## Fase 4: Validación

| Tarea | Descripción | Criterio |
|-------|-------------|----------|
| 4.1 | `dotnet build` | Sin errores |
| 4.2 | Ejecutar tests existentes | Suite en verde |

---

## Orden de ejecución

1. Fase 1 (merge) — obligatorio primero.
2. Fases 2 y 3 — pueden ejecutarse en paralelo por archivo.
3. Fase 4 — al final.
