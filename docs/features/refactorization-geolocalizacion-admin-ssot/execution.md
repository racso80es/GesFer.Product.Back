---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
updated: '2026-03-21'
process: refactorization
branch: feat/refactorization-geolocalizacion-admin-ssot
implementation_ref: docs/features/refactorization-geolocalizacion-admin-ssot/implementation.md
items_applied:
  - Fase 1: DTOs Geo, IAdminApiClient extendido, AdminApiClient, mocks Api + IntegrationTests, AdminGeolocationValidationService + DI
  - Fase 2-4: Eliminación entidades Country/State/City/PostalCode, Language sin Countries, User/Customer/Supplier sin navegaciones geo, DbContext, configuraciones EF
  - Fase 5: Validación jerárquica vía IAdminGeolocationValidationService en create/update User, Customer, Supplier
  - Fase 6: Eliminación controladores y CRUD aplicación (Country/State/City/PostalCode)
  - Fase 7: AuthService sin Include Country; LoginCommandHandler D3/D4 + TODOs
  - Fase 8: JsonDataSeeder sin Countries/Cities; MasterDataSeeder sin seed geo local; SetupService sin métodos muertos geo
  - Fase 9: Tests unitarios CreateUserCommandHandler con mock geo; eliminados tests PostalCode commands y tests integración Country/State/City
  - Migración EF: 20260321152937_RemoveLocalGeoCatalog
  - Fase 10: docs/architecture/geolocalizacion-contract.md (este cierre)
notes:
  - Integración: parte de tests DatabaseStep reportó 401 en login en una ejecución local; revisar entorno Docker/seed si persiste (56/79 pasaron en la misma corrida).
---

# Execution — refactorización geolocalización Admin SSOT

Registro de cambios aplicados al código según [`implementation.md`](./implementation.md).

## Resumen

- **Product** consume catálogo geo solo vía **Admin API** (`IAdminApiClient` + `IAdminGeolocationValidationService`).
- Eliminadas tablas y entidades locales de geo; columnas `Guid?` en User/Customer/Supplier **sin FK** local.
- Migración **`RemoveLocalGeoCatalog`** elimina FKs y tablas `Countries`, `States`, `Cities`, `PostalCodes`.

## Artefactos

| Tipo | Ruta |
|------|------|
| Migración | `src/Infrastructure/Migrations/20260321152937_RemoveLocalGeoCatalog.cs` |
| Contrato arquitectura | [`docs/architecture/geolocalizacion-contract.md`](../../architecture/geolocalizacion-contract.md) |

---

*Última actualización: acción execution (marzo 2026).*
