---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
plan_ref: docs/features/fix-namespaces-remove-shared/plan.md
spec_ref: docs/features/fix-namespaces-remove-shared/spec.md
branch: feature/fix-namespaces-remove-shared-14188559524536972040
items:
  - id: 1.1
    phase: Merge
  - id: 2.1
    phase: Refactor
  - id: 3.1
    phase: Usings
  - id: 4.1
    phase: Validación
---

# Implementation: fix-namespaces-remove-shared

## Ítems de implementación

### 1.1 – Merge con origin/main
- **Acción:** Ejecutar
- **Ruta:** Repo
- **Propuesta:** `git merge origin/main`; resolver conflictos adoptando versión de main en los 7 archivos.
- **Dependencias:** Ninguna

### 2.1 – Modificar: SetupService.cs (referencias MasterDataSeeder)
- **Id:** 2.1
- **Acción:** Modificar
- **Ruta:** `src/Api/Services/SetupService.cs`
- **Ubicación:** Líneas ~154-156
- **Propuesta:** Sustituir `GesFer.Infrastructure.Services.MasterDataSeeder` por `GesFer.Product.Back.Infrastructure.Services.MasterDataSeeder` (2 ocurrencias).

### 2.2 – Modificar: JsonDataSeederTests.cs (SeedResult)
- **Id:** 2.2
- **Acción:** Modificar
- **Ruta:** `src/IntegrationTests/Services/JsonDataSeederTests.cs`
- **Ubicación:** Línea ~13
- **Propuesta:** `using SeedResult = GesFer.Infrastructure.Services.SeedResult` → `using SeedResult = GesFer.Product.Back.Infrastructure.Services.SeedResult`

### 2.3 – Modificar: TestDataSeeder.cs (SensitiveDataSanitizer)
- **Id:** 2.3
- **Acción:** Modificar
- **Ruta:** `src/IntegrationTests/Helpers/TestDataSeeder.cs`
- **Ubicación:** Línea ~42
- **Propuesta:** `new GesFer.Domain.Services.SensitiveDataSanitizer()` → `new GesFer.Product.Back.Domain.Services.SensitiveDataSanitizer()`

### 2.4 – Modificar: DatabaseFixture.cs (Program)
- **Id:** 2.4
- **Acción:** Modificar
- **Ruta:** `src/IntegrationTests/DatabaseFixture.cs`
- **Ubicación:** Líneas ~15, ~29
- **Propuesta:** `GesFer.Api.Program` → `GesFer.Product.Back.Api.Program` (2 ocurrencias)

### 2.5 – Modificar: Program.cs (DefaultValueSchemaFilter)
- **Id:** 2.5
- **Acción:** Modificar
- **Ruta:** `src/Api/Program.cs`
- **Ubicación:** Línea ~82
- **Propuesta:** `GesFer.Api.Swagger.DefaultValueSchemaFilter` → `GesFer.Product.Back.Api.Swagger.DefaultValueSchemaFilter`

### 2.6 – Modificar: Program.cs (enrich Application)
- **Id:** 2.6
- **Acción:** Modificar
- **Ruta:** `src/Api/Program.cs`
- **Ubicación:** Líneas ~43, ~175
- **Propuesta:** `"Application", "GesFer.Api"` → `"Application", "GesFer.Product.Back.Api"`

### 3.1 – Modificar: SetupService.cs (using duplicado)
- **Id:** 3.1
- **Acción:** Modificar
- **Ruta:** `src/Api/Services/SetupService.cs`
- **Ubicación:** Líneas 1-2
- **Propuesta:** Eliminar una de las dos líneas `using GesFer.Product.Back.Domain.Entities;`

### 3.2 – Modificar: CustomerConfiguration.cs (using duplicado)
- **Id:** 3.2
- **Acción:** Modificar
- **Ruta:** `src/Infrastructure/Data/Configurations/CustomerConfiguration.cs`
- **Ubicación:** Líneas 1-2
- **Propuesta:** Eliminar una de las dos líneas `using GesFer.Product.Back.Domain.Entities;`

### 4.1 – Validación
- **Id:** 4.1
- **Acción:** Ejecutar
- **Propuesta:** `dotnet build` y tests unitarios/integración.

---

## Resumen por archivo

| Archivo | Ítems |
|---------|-------|
| src/Api/Services/SetupService.cs | 2.1, 3.1 |
| src/Api/Program.cs | 2.5, 2.6 |
| src/Infrastructure/Data/Configurations/CustomerConfiguration.cs | 3.2 |
| src/IntegrationTests/Services/JsonDataSeederTests.cs | 2.2 |
| src/IntegrationTests/Helpers/TestDataSeeder.cs | 2.3 |
| src/IntegrationTests/DatabaseFixture.cs | 2.4 |

---

## Orden sugerido

1. Fase 1 (merge) — si hay conflictos, resolver antes de continuar.
2. Ítems 2.1–2.6, 3.1–3.2 (sin dependencias entre sí).
3. Ítem 4.1 (validación final).
