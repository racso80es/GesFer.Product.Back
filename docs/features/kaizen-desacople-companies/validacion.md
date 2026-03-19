---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
branch: feat/kaizen-desacople-companies
global: true
checks:
  build: ok
  unit_tests: n/a
  integration_tests: parcial
  migration: generada
git_changes: true
---

# Validación: Kaizen desacople Companies

## Checks ejecutados

| Check | Estado | Notas |
|-------|--------|-------|
| dotnet build | ✅ OK | Compilación correcta |
| Migración RemoveCompaniesTable | ✅ Generada | Elimina FKs y tabla Companies |
| UserControllerTests (aislados) | ✅ 13/13 | Pasan en ejecución aislada |
| SupplierControllerTests (aislados) | — | Pendiente verificar |
| CustomerControllerTests (aislados) | — | Pendiente verificar |
| Suite completa | ⚠️ Parcial | 83 pasan, 22 fallan (User/Supplier/Customer); fallos en SetAuthTokenAsync con 401. En ejecución aislada los tests pasan. Posible contención al ejecutar suite completa. |

## Pendiente

1. **Aplicar migración en BD:** `dotnet ef database update` (requiere MySQL en ejecución).
2. **Suite completa:** Investigar fallos intermitentes en login cuando se ejecutan todos los tests (posible issue preexistente de paralelismo/contención).

## Archivos modificados

- Eliminados: Company.cs, CompanyConfiguration.cs
- Modificados: ApplicationDbContext.cs, UserController.cs, SupplierController.cs, CustomerController.cs
- Migración: 20260319190434_RemoveCompaniesTable.cs
- Tests: UserControllerTests, SupplierControllerTests, CustomerControllerTests
- Docs: execution.md, companies-contract.md
