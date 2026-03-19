---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
type: plan-pendiente
parent: docs/features/fix-namespaces-remove-shared/plan.md
scope: .csproj, .sln, Dockerfile, scripts, namespaces IntegrationTests
---

# Plan pendiente: Refactor namespaces (proyectos, scripts, IntegrationTests)

## Contexto

Tras la Fase 1-4 ejecutada, quedan pendientes:
- **Proyectos (.csproj, .sln):** Nombres GesFer.Api, GesFer.Application, etc.
- **Dockerfile / Dockerfile.test:** Rutas y referencias a proyectos
- **Scripts:** setup-database.ps1, Migrations/README.md
- **Namespaces IntegrationTests:** GesFer.IntegrationTests → GesFer.Product.Back.IntegrationTests

---

## Fase 5: Namespaces IntegrationTests (código C#)

| Tarea | Archivo | Cambio |
|-------|---------|--------|
| 5.1 | DatabaseCollection.cs | `namespace GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |
| 5.2 | IntegrationTestWebAppFactory.cs | `namespace GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |
| 5.3 | DatabaseFixture.cs | `namespace GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |
| 5.4 | WebApplicationFactory.cs | `namespace GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |
| 5.5 | SharedTestCollection.cs | `namespace GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |
| 5.6 | Helpers (TestDataSeeder, MockSetupService, MockAdminApiClient, HttpRetryHelper) | `GesFer.IntegrationTests.Helpers` → `GesFer.Product.Back.IntegrationTests.Helpers` |
| 5.7 | Controllers tests (todos los *ControllerTests.cs) | `GesFer.IntegrationTests.Controllers` → `GesFer.Product.Back.IntegrationTests.Controllers` |
| 5.8 | Services tests (AdminApiClientTests, DbInitializerTests, JsonDataSeederTests, ValueObjectValidationTests) | `GesFer.IntegrationTests.Services` → `GesFer.Product.Back.IntegrationTests.Services` |
| 5.9 | IntegrationTestWebAppFactory.cs | `GesFer.IntegrationTests.Helpers` → `GesFer.Product.Back.IntegrationTests.Helpers` en usings |
| 5.10 | SetupControllerTests, MyCompanyControllerTests | `using GesFer.IntegrationTests.Helpers` → `GesFer.Product.Back.IntegrationTests.Helpers` |

---

## Fase 6: Scripts y documentación

| Tarea | Archivo | Cambio |
|-------|---------|--------|
| 6.1 | setup-database.ps1 | Ruta `../Infrastructure/GesFer.Infrastructure.csproj` — verificar si debe cambiar (nombre archivo sigue igual por ahora) |
| 6.2 | Migrations/README.md | Rutas `GesFer.Infrastructure.csproj` — documentar comando actual |

*Nota:* Los .csproj mantienen nombres GesFer.*.csproj; los scripts referencian esos archivos. Si en Fase 7 se renombran proyectos, actualizar aquí.

---

## Fase 7: Proyectos y solución (opcional, impacto alto)

| Tarea | Descripción | Riesgo |
|-------|-------------|--------|
| 7.1 | Renombrar GesFer.Api.csproj → GesFer.Product.Back.Api.csproj | Alto — CI/CD, Docker, referencias |
| 7.2 | Renombrar GesFer.Application.csproj → GesFer.Product.Back.Application.csproj | Alto |
| 7.3 | Renombrar GesFer.Domain.csproj → GesFer.Product.Back.Domain.csproj | Alto |
| 7.4 | Renombrar GesFer.Infrastructure.csproj → GesFer.Product.Back.Infrastructure.csproj | Alto |
| 7.5 | Renombrar GesFer.IntegrationTests.csproj → GesFer.Product.Back.IntegrationTests.csproj | Alto |
| 7.6 | Actualizar GesFer.Product.sln con nuevos nombres | Alto |
| 7.7 | Actualizar ProjectReference en todos los .csproj | Alto |
| 7.8 | Actualizar Dockerfile, Dockerfile.test | Alto |
| 7.9 | Actualizar scripts, README | Medio |

**Recomendación:** Ejecutar Fase 5 (namespaces) primero; Fase 7 requiere validación de CI/CD y Docker.

---

## Fase 8: Dockerfile (estructura actual)

| Tarea | Archivo | Observación |
|-------|---------|-------------|
| 8.1 | Dockerfile | Rutas `src/Product/Back/` — verificar si coincide con estructura real (repo tiene `src/Api/`, `src/application/`) |
| 8.2 | Dockerfile.test | Rutas `src/Api/`, `src/Application/` — mayúsculas en Application |

---

## Orden sugerido

1. **Fase 5** — Namespaces IntegrationTests (bajo riesgo, build + tests).
2. **Fase 6** — Scripts (revisión, sin cambios si proyectos no se renombran).
3. **Fase 8** — Verificar Dockerfile vs estructura real.
4. **Fase 7** — Solo si se decide renombrar proyectos (feature separada recomendada).

---

## Criterios de aceptación

- [x] Fase 5: Todos los namespaces IntegrationTests → GesFer.Product.Back.IntegrationTests
- [x] Fase 5: Build y tests en verde
- [x] Fase 6: Documentado / verificado (scripts OK)
- [ ] Fase 8: Dockerfile actualizado (pendiente)
