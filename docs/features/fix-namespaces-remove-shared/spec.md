---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
base: origin/main
scope: src/, tests/, IntegrationTests/
contract_ref: SddIA/norms/features-documentation-pattern.md
---

# SPEC: Resolución de conflictos y mejora de refactor namespaces

## Contexto

La rama `feature/fix-namespaces-remove-shared-*` tiene conflictos con `origin/main` y referencias residuales a namespaces antiguos. El refactor pretende eliminar dependencias de Shared y estandarizar todos los namespaces a `GesFer.Product.Back.*`.

## Arquitectura

- **Capas afectadas:** Api, Application, Domain, Infrastructure, IntegrationTests
- **Patrón de namespace objetivo:** `GesFer.Product.Back.{Capa}.{Subcapa}`
- **Cambios semánticos ya aplicados:** SharedSecret→InternalSecret, ConfigureSharedEntities→ConfigureCommonEntities, UpdateSharedAuditFields→UpdateCommonAuditFields

## Especificación técnica

### Punto 1: Resolución de conflictos de merge

**Descripción:** Resolver los 7 archivos en conflicto al integrar `origin/main` en la rama actual.

**Archivos afectados:**
- `src/Api/Services/SetupService.cs`
- `src/Infrastructure/Data/Configurations/CustomerConfiguration.cs`
- `src/Infrastructure/Data/Configurations/SupplierConfiguration.cs`
- `src/Infrastructure/Data/Configurations/UserConfiguration.cs`
- `src/Infrastructure/Services/JsonDataSeeder.cs`
- `src/Infrastructure/Services/MasterDataSeeder.cs`
- `src/IntegrationTests/Services/JsonDataSeederTests.cs`

**Criterio de aceptación:** Merge con main sin conflictos; en bloques de usings, adoptar la versión de main (`GesFer.Product.Back.*`).

---

### Punto 2: Completar refactor de referencias residuales

**Descripción:** Sustituir todas las referencias a namespaces antiguos (`GesFer.Infrastructure.*`, `GesFer.Domain.*`, `GesFer.Api.*`) por `GesFer.Product.Back.*`.

**Alcance (clarificado):** Todo incluido — código C# (src, tests, IntegrationTests), .csproj, Dockerfile, scripts.

**Touchpoints identificados:**
- `SetupService.cs`: `GesFer.Infrastructure.Services.MasterDataSeeder` (líneas 154-156)
- `JsonDataSeederTests.cs`: `using SeedResult = GesFer.Infrastructure.Services.SeedResult`
- `TestDataSeeder.cs`: `new GesFer.Domain.Services.SensitiveDataSanitizer()`
- `DatabaseFixture.cs`: `GesFer.Api.Program`
- `Program.cs`: `GesFer.Api.Swagger.DefaultValueSchemaFilter`
- `.csproj`, `Dockerfile`, scripts: rutas y nombres de proyecto

**Criterio de aceptación:** No queden referencias literales a `GesFer.Application`, `GesFer.Domain`, `GesFer.Infrastructure` o `GesFer.Api` en código, proyectos ni scripts.

---

### Punto 3: Eliminar usings duplicados

**Descripción:** Eliminar declaraciones `using` duplicadas que no aportan valor.

**Archivos afectados:**
- `SetupService.cs`: `using GesFer.Product.Back.Domain.Entities;` (duplicado)
- `CustomerConfiguration.cs`: `using GesFer.Product.Back.Domain.Entities;` (duplicado)

**Criterio de aceptación:** Una sola declaración por namespace en cada archivo.

---

### Punto 4: Validación post-cambios

**Descripción:** Garantizar que el código compila y los tests pasan tras aplicar los cambios. Por ahora: build + tests existentes (mismo nivel que el resto de puntos del checklist).

**Checks:**
- `dotnet build` sin errores
- Tests unitarios e integración ejecutados y pasando
- Sin regresiones en funcionalidad existente

**Criterio de aceptación:** Build exitoso y suite de tests en verde.

---

## Seguridad

- No se introducen cambios en lógica de negocio ni en flujos de autenticación.
- Cambios limitados a namespaces, usings y referencias; sin modificación de algoritmos.

## Criterios de aceptación globales (checklist)

| # | Criterio | Estado |
|---|----------|--------|
| 1 | Merge con `origin/main` completado sin conflictos | ✓ |
| 2 | Todas las referencias a namespaces antiguos sustituidas (código C#) | ✓ |
| 3 | Usings duplicados eliminados | ✓ |
| 4 | Build y tests existentes en verde | ✓ |
