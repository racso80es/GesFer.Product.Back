---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
phases:
  - id: pre
    name: Preparar entorno (rama feat)
  - id: '1'
    name: Eliminar entidad Company y configuración EF
  - id: '2'
    name: Generar migración (eliminar tabla y FKs)
  - id: '3'
    name: Ajustar controllers (CompanyId solo desde claim)
  - id: '4'
    name: Seeds y DbInitializer
  - id: '5'
    name: Tests
  - id: '6'
    name: Documentación del contrato
  - id: '7'
    name: Validación
  - id: '8'
    name: Finalizar (PR, merge)
spec_ref: docs/features/kaizen-desacople-companies/spec.md
clarify_ref: docs/features/kaizen-desacople-companies/clarify.md
---

# Plan: Kaizen desacople de Companies

## Fase pre: Preparar entorno

**Skill:** iniciar-rama (paths.skillCapsules.iniciar-rama).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| pre.1 | Crear rama feat desde master actualizado | Rama `feat/kaizen-desacople-companies` |
| pre.2 | Posicionar repositorio en la rama | Working tree listo para commits |

**Invocación:** `Iniciar-Rama.bat feat kaizen-desacople-companies` (o skill iniciar-rama: BranchType feat, BranchName kaizen-desacople-companies).

**Criterio:** No trabajar en master; toda la implementación en la rama feat.

---

## Fase 1: Eliminar entidad Company y configuración EF

| Tarea | Descripción | Archivos |
|-------|-------------|----------|
| 1.1 | Eliminar entidad Company del dominio | `src/domain/Entities/Company.cs` — **eliminar** |
| 1.2 | Eliminar CompanyConfiguration | `src/Infrastructure/Data/Configurations/CompanyConfiguration.cs` — **eliminar** |
| 1.3 | Eliminar DbSet Companies del ApplicationDbContext | `src/Infrastructure/Data/ApplicationDbContext.cs` — quitar línea `DbSet<Company> Companies` |
| 1.4 | Verificar referencias a Company en dominio/Infra | Buscar `using` o referencias a `Company`; eliminar o ajustar |

**Orden:** 1.1 → 1.2 → 1.3 → 1.4. Tras eliminar Company, las relaciones EF inferidas (desde ICollection en Company) desaparecen automáticamente.

**Criterio:** Build sin errores de compilación; no hay referencias a Company en domain ni en configuraciones EF.

---

## Fase 2: Generar migración (eliminar tabla y FKs)

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 2.1 | Generar migración EF | `dotnet ef migrations add RemoveCompaniesTable` (o equivalente vía skill invoke-command) |
| 2.2 | Verificar migración generada | Debe: DropForeignKey para cada FK a Companies; DropTable Companies |
| 2.3 | Aplicar migración (local) | `dotnet ef database update` — verificar que aplica correctamente |

**Nota:** La migración debe eliminar primero las FKs y luego la tabla Companies. Las columnas CompanyId en User, Article, TaxType, etc. permanecen.

**Criterio:** Migración aplica sin errores; tabla Companies eliminada; columnas CompanyId intactas.

---

## Fase 3: Ajustar controllers (CompanyId solo desde claim)

**Regla:** CompanyId no ha de ser dado por el frontend. Siempre desde claim JWT.

### 3.1 Controllers con GetAll que aceptan companyId en query

| Controller | Cambio |
|------------|--------|
| UserController | Quitar `[FromQuery] Guid? companyId`; añadir `GetCompanyId()`; pasar `GetCompanyId()` al comando |
| SupplierController | Idem |
| CustomerController | Idem |

**Patrón:** Crear método privado `GetCompanyId()` (como en TaxTypesController/ArticleFamiliesController) y usar siempre ese valor.

### 3.2 Controllers con Create que reciben CompanyId en body

| Controller | Cambio |
|------------|--------|
| UserController.Create | Antes de crear comando: `dto.CompanyId = GetCompanyId()` (ignorar valor del body) |
| SupplierController.Create | Idem |
| CustomerController.Create | Idem |

**Patrón:** El controller obtiene CompanyId del claim y lo asigna al DTO antes de crear el comando. El handler no cambia.

### 3.3 Handlers/Commands que requieren CompanyId

| Command/Handler | Verificación |
|-----------------|--------------|
| GetAllUsersCommand | Aceptar CompanyId (obligatorio); controller pasa GetCompanyId() |
| GetAllSuppliersCommand | Idem |
| GetAllCustomersCommand | Idem |
| CreateUserCommand | Dto.CompanyId viene del controller (ya asignado) |
| CreateSupplierCommand | Idem |
| CreateCustomerCommand | Idem |

**Criterio:** Ningún endpoint acepta CompanyId del cliente; todos usan GetCompanyId() desde claims.

---

## Fase 4: Seeds y DbInitializer

| Tarea | Descripción | Archivos |
|-------|-------------|----------|
| 4.1 | JsonDataSeeder: eliminar referencias a CompanySeed/data.Companies | `src/Infrastructure/Services/JsonDataSeeder.cs` — quitar uso de Companies si existe |
| 4.2 | DbInitializer: verificar que no depende de tabla Companies | `src/Infrastructure/Data/DbInitializer.cs` — usa adminClient.GetCompanyAsync; no tabla local |
| 4.3 | Manejo de error: GetCompanyAsync devuelve null | Handlers que llaman GetCompanyAsync: asegurar manejo (InvalidOperationException, BadRequest, etc.) |

**Criterio:** Seeds y DbInitializer funcionan sin tabla Companies; errores por empresa inexistente manejados.

---

## Fase 5: Tests

| Tarea | Descripción | Archivos |
|-------|-------------|----------|
| 5.1 | CreateUserCommandHandlerTests | Verificar mock GetCompanyAsync; no usar DbSet Companies |
| 5.2 | UpdateUserCommandHandlerTests | Idem |
| 5.3 | UserControllerTests | GetAll: quitar companyId de query; usar token con CompanyId en claims |
| 5.4 | SupplierControllerTests | Idem |
| 5.5 | CustomerControllerTests | Idem (si existe) |
| 5.6 | JsonDataSeederTests | Seed:CompanyId config — verificar |
| 5.7 | DbInitializer / smoke tests | Verificar |
| 5.8 | IntegrationTests con MySQL | Si usan BD real, aplicar migración en BD de test |

**Criterio:** `dotnet test` pasa sin errores.

---

## Fase 6: Documentación del contrato

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 6.1 | Documentar contrato Companies | docs/architecture o docs/features/kaizen-desacople-companies: Admin SSOT; Product vía IAdminApiClient; CompanyId dato referencial; JWT con CompanyId |
| 6.2 | Actualizar clarify.md si hay decisiones adicionales | Registro de decisiones de implementación |

**Criterio:** Contrato documentado y accesible.

---

## Fase 7: Validación

| Tarea | Descripción | Criterio |
|-------|-------------|----------|
| 7.1 | Build | `dotnet build` sin errores |
| 7.2 | Tests | `dotnet test` pasa |
| 7.3 | Migración en BD limpia | Aplicar migración desde cero; verificar esquema |
| 7.4 | Checklist validacion.md | Crear validacion.md con checks ejecutados |

---

## Fase 8: Finalizar (PR, merge)

**Skill:** finalizar-git (paths.skillCapsules.finalizar-git).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 8.1 | Pre-PR: push a origin | Rama feat/kaizen-desacople-companies en origin |
| 8.2 | Crear Pull Request | PR hacia master/main |
| 8.3 | Post-PR: merge y limpieza | Merge a master; eliminar rama remota |
| 8.4 | Evolution Logs | paths.evolutionPath: entrada con fecha, rama, descripción |

---

## Orden de ejecución

1. **Pre:** Crear rama feat/kaizen-desacople-companies
2. **Fase 1:** Eliminar Company, CompanyConfiguration, DbSet (un solo commit)
3. **Fase 2:** Generar y aplicar migración
4. **Fase 3:** Ajustar UserController, SupplierController, CustomerController
5. **Fase 4:** Seeds y DbInitializer
6. **Fase 5:** Tests
7. **Fase 6:** Documentación
8. **Fase 7:** Validación
9. **Fase 8:** Finalizar

---

## Estrategia de commits

| Fase | Tipo | Ejemplo mensaje |
|------|------|-----------------|
| pre | chore | docs(kaizen): añadir objectives, spec, clarify, plan |
| 1 | refactor | refactor(domain): eliminar entidad Company y tabla Companies |
| 2 | chore | chore(ef): migración RemoveCompaniesTable |
| 3 | feat | feat(api): CompanyId solo desde claim en User, Supplier, Customer |
| 4 | fix | fix(seeds): ajustar JsonDataSeeder y DbInitializer sin Companies |
| 5 | test | test: actualizar tests tras desacople Companies |
| 6 | docs | docs: contrato Companies (Admin SSOT, Product vía API) |
| 7 | chore | chore(kaizen): validación pre-PR |

---

## Touchpoints (resumen)

| Área | Archivos |
|------|----------|
| Domain | Company.cs (eliminar) |
| Infrastructure | ApplicationDbContext.cs, CompanyConfiguration.cs (eliminar), JsonDataSeeder.cs, DbInitializer.cs |
| Migrations | Nueva migración RemoveCompaniesTable |
| Api | UserController.cs, SupplierController.cs, CustomerController.cs |
| Tests | CreateUserCommandHandlerTests, UpdateUserCommandHandlerTests, UserControllerTests, SupplierControllerTests, JsonDataSeederTests, IntegrationTests |
| Docs | docs/features/kaizen-desacople-companies/, docs/architecture (opcional) |
