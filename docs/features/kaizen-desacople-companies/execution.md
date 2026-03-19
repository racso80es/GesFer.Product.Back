---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
items_applied:
  - Eliminada entidad Company y CompanyConfiguration
  - Eliminado DbSet Companies de ApplicationDbContext
  - Migración RemoveCompaniesTable generada
  - UserController, SupplierController, CustomerController: CompanyId desde claim
  - Tests de integración actualizados con SetAuthTokenAsync
---

# Ejecución: Kaizen desacople Companies

## Items aplicados

| Fase | Item | Estado |
|------|------|--------|
| 1 | Eliminar src/domain/Entities/Company.cs | ✅ |
| 1 | Eliminar CompanyConfiguration.cs | ✅ |
| 1 | Eliminar DbSet Companies de ApplicationDbContext | ✅ |
| 2 | Migración RemoveCompaniesTable | ✅ |
| 3 | UserController: GetCompanyId(), [Authorize], GetAll/Create sin companyId del cliente | ✅ |
| 3 | SupplierController: idem | ✅ |
| 3 | CustomerController: idem | ✅ |
| 5 | UserControllerTests: SetAuthTokenAsync, tests actualizados | ✅ |
| 5 | SupplierControllerTests: idem | ✅ |
| 5 | CustomerControllerTests: idem | ✅ |

## Pendiente

- Aplicar migración en BD (dotnet ef database update) — requiere MySQL en ejecución
- Validación final (build, test)
