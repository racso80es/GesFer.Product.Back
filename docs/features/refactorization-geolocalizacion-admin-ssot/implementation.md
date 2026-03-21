---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
updated: '2026-03-21'
process: refactorization
branch: feat/refactorization-geolocalizacion-admin-ssot
plan_ref: docs/features/refactorization-geolocalizacion-admin-ssot/plan.md
spec_ref: docs/features/refactorization-geolocalizacion-admin-ssot/spec.md
clarify_ref: docs/features/refactorization-geolocalizacion-admin-ssot/clarify.md
contract_ref: docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json
implementation_status: ejecutado_en_codigo
items_phases:
  - pre
  - '1'
  - '2'
  - '3'
  - '4'
  - '5'
  - '6'
  - '7'
  - '8'
  - '9'
  - '10'
---

# Implementation — touchpoints (Admin SSOT geolocalización)

Documento único de **touchpoints** derivado de [`plan.md`](./plan.md), [`spec.md`](./spec.md) y [`clarify.md`](./clarify.md). **No sustituye** la ejecución en código (acción **execution**). Referencia de rama: `feat/refactorization-geolocalizacion-admin-ssot`.

---

## Algoritmo de validación jerárquica (cierra clarify §5.1)

Cuando un comando de **crear/actualizar** incluya uno o más de `CountryId`, `StateId`, `CityId`, `PostalCodeId` (no todos obligatorios; validar solo los presentes):

1. **CountryId:** `GET /api/geolocation/countries/{countryId}` → 404 ⇒ error de validación (mensaje claro). 200 ⇒ `country` válido.
2. **StateId:** `GET /api/geolocation/countries/{countryId}/states` y comprobar que exista un estado con `id == StateId` y `countryId` coherente con **D6**. Si no hay `CountryId` en el DTO pero sí `StateId`, obtener primero el país vía `GET /api/geolocation/states/{stateId}` no existe en contrato — **obligatorio** enviar `CountryId` cuando se envía `StateId`, o resolver país desde cadena previa; la spec solo expone estados por país. Regla práctica: si viene `StateId`, debe venir `CountryId` o inferirse desde validación previa del estado (si Admin añade `GET /api/geolocation/states/{stateId}` en el futuro, actualizar; **con el contrato actual**: validar `StateId` contra la lista de estados del país indicado por `CountryId`).
3. **CityId:** `GET /api/geolocation/states/{stateId}/cities` y comprobar `id == CityId` y coherencia con `StateId`.
4. **PostalCodeId:** `GET /api/geolocation/cities/{cityId}/postal-codes` y comprobar `id == PostalCodeId` y coherencia con `CityId`.

**Short-circuit:** ante 404/500 en cadena, no persistir; mensaje operable (sin filtrar secretos). **Orden de llamadas:** de arriba a abajo en la jerarquía; si un nivel falla, no validar niveles inferiores como válidos.

**Ubicación código:** servicio dedicado p. ej. `Infrastructure/Services/AdminGeolocationValidationService` (nombre orientativo) inyectado en handlers de User/Customer/Supplier; internamente usa el cliente HTTP geo (misma interfaz o la extendida en fase 1).

---

## Fase pre

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| pre.1 | — | — | Rama ya definida en plan; sin touchpoint de código. |
| pre.2 | — | — | Commits de documentación; sin touchpoint de código. |

---

## Fase 1: Cliente HTTP Admin — DTOs y GET geo

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 1.1 | Crear | `src/Infrastructure/DTOs/` (p. ej. `Geo/` o prefijo acordado) | DTOs de lectura alineados a [`admin-geolocation-contract.json`](./admin-geolocation-contract.json): `CountryGeoReadDto`, `StateGeoReadDto`, `CityGeoReadDto`, `PostalCodeGeoReadDto` con serialización compatible (PascalCase + `[JsonPropertyName]` si hace falta para camelCase). |
| 1.2 | Modificar | `src/Infrastructure/Services/IAdminApiClient.cs` | Añadir cinco métodos async que mapeen los GET del contrato (rutas relativas bajo base `AdminApi:BaseUrl`). Alternativa aceptada: interfaz `IAdminGeolocationClient` + registro DI si se prefiere separar responsabilidades. |
| 1.2 | Modificar | `src/Infrastructure/Services/AdminApiClient.cs` | Implementar los GET; reutilizar el mismo `HttpClient` (ya añade `X-Internal-Secret` en ctor — **D2**). |
| 1.3 | Modificar | `src/Api/Services/MockAdminApiClient.cs` | Implementar métodos geo devolviendo JSON acorde a `examples` del contrato para entorno `Testing`/Development. |
| 1.4 | Modificar | `src/IntegrationTests/Services/AdminApiClientTests.cs` (y/o tests nuevos) | Cobertura de deserialización y rutas; alinear con mock. |

---

## Fase 2: Eliminar entidades de dominio geo

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 2.1 | Eliminar | `src/domain/Entities/Country.cs` | Eliminar archivo. |
| 2.1 | Eliminar | `src/domain/Entities/State.cs` | Eliminar archivo. |
| 2.1 | Eliminar | `src/domain/Entities/City.cs` | Eliminar archivo. |
| 2.1 | Eliminar | `src/domain/Entities/PostalCode.cs` | Eliminar archivo. |
| 2.2 | Modificar | `src/domain/Entities/Language.cs` | Quitar `ICollection<Country> Countries` y cualquier referencia a `Country`. |

---

## Fase 3: Migración EF y persistencia

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 3.1 | Modificar | `src/Infrastructure/Data/ApplicationDbContext.cs` | Quitar `DbSet` de `Countries`, `States`, `Cities`, `PostalCodes`. |
| 3.2 | Eliminar | `src/Infrastructure/Data/Configurations/CountryConfiguration.cs` | Eliminar. |
| 3.2 | Eliminar | `src/Infrastructure/Data/Configurations/StateConfiguration.cs` | Eliminar. |
| 3.2 | Eliminar | `src/Infrastructure/Data/Configurations/CityConfiguration.cs` | Eliminar. |
| 3.2 | Eliminar | `src/Infrastructure/Data/Configurations/PostalCodeConfiguration.cs` | Eliminar. |
| 3.3 | Crear | `src/Infrastructure/Migrations/` | Nueva migración: eliminar FKs desde `Users`, `Customers`, `Suppliers` hacia tablas geo; eliminar tablas `Countries`, `States`, `Cities`, `PostalCodes` (orden respetando dependencias). Revisar migraciones existentes (`InitialCreate`, etc.) como referencia de nombres de FK. |

---

## Fase 4: Agregados User, Customer, Supplier

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 4.1 | Modificar | `src/domain/Entities/User.cs` | Quitar navegaciones `PostalCode`, `City`, `State`, `Country`; mantener `Guid?` de ids. |
| 4.1 | Modificar | `src/domain/Entities/Customer.cs` | Igual que User (misma estructura geo). |
| 4.1 | Modificar | `src/domain/Entities/Supplier.cs` | Igual que User. |
| 4.2 | Modificar | `src/Infrastructure/Data/Configurations/UserConfiguration.cs` | Eliminar `HasOne` hacia entidades geo; conservar índices opcionales en columnas Guid si aplica. |
| 4.2 | Modificar | `src/Infrastructure/Data/Configurations/CustomerConfiguration.cs` | Igual. |
| 4.2 | Modificar | `src/Infrastructure/Data/Configurations/SupplierConfiguration.cs` | Igual. |

---

## Fase 5: Validación en comandos create/update

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 5.1 | Crear | Servicio de validación (ruta sugerida arriba) | Implementar algoritmo §5.1 de este documento usando cliente Admin geo. |
| 5.2 | Modificar | `src/application/Handlers/User/CreateUserCommandHandler.cs` | Sustituir comprobaciones `_context.Countries` / `PostalCodes` / etc. por validación Admin; invocar si el DTO trae ids geo. |
| 5.2 | Modificar | `src/application/Handlers/User/UpdateUserCommandHandler.cs` | Igual. |
| 5.2 | Modificar | `src/application/Handlers/Customer/CreateCustomerCommandHandler.cs` | Igual (hoy valida contra `_context.PostalCodes`, `Cities`, `States`, `Countries`). |
| 5.2 | Modificar | `src/application/Handlers/Customer/UpdateCustomerCommandHandler.cs` | Revisar y alinear. |
| 5.2 | Modificar | `src/application/Handlers/Supplier/CreateSupplierCommandHandler.cs` | Igual. |
| 5.2 | Modificar | `src/application/Handlers/Supplier/UpdateSupplierCommandHandler.cs` | Revisar y alinear. |

---

## Fase 6: Retirar API y handlers CRUD geo

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 6.1 | Eliminar | `src/Api/Controllers/CountryController.cs` | Eliminar controlador. |
| 6.1 | Eliminar | `src/Api/Controllers/StateController.cs` | Eliminar. |
| 6.1 | Eliminar | `src/Api/Controllers/CityController.cs` | Eliminar. |
| 6.1 | Eliminar | `src/Api/Controllers/PostalCodeController.cs` | Eliminar. |
| 6.1 | Eliminar | Handlers y comandos bajo `src/application/Handlers/Country/`, `State/`, `City/`, `PostalCode/` (20 handlers en total — 5 por carpeta) | Eliminar archivos asociados. |
| 6.1 | Eliminar | Comandos en `src/application/Commands/Country/`, `State/`, `City/`, `PostalCode/` | Eliminar. |
| 6.1 | Eliminar | DTOs `src/application/DTOs/Country/`, y equivalentes State/City/PostalCode si ya no se usan | Tras ajustar referencias. |
| 6.2 | Modificar | `src/Api/DependencyInjection.cs` | No requiere quitar registro manual de handlers si se usa reflexión; verificar que no queden referencias rotas en controladores eliminados. |

---

## Fase 7: Login e idioma (D3/D4)

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 7.1 | Modificar | `src/application/Handlers/Auth/LoginCommandHandler.cs` | Eliminar uso de `user.Country?.LanguageId`; fijar idioma efectivo a **es** / `CountryLanguageId` según **D3/D4** y comentarios **TODO-i18n-context** / **TODO-default-lang-ES**. |
| 7.1 | Modificar | `src/application/DTOs/Auth/LoginResponseDto.cs` | Ajustar semántica de `CountryLanguageId` o documentar deprecación temporal en comentario. |
| 7.2 | Modificar | Handlers auth/login relacionados | Enlaces o comentarios a TODOs de clarify §4. |

---

## Fase 8: Seeds y SetupService

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 8.1 | Modificar | `src/Infrastructure/Services/JsonDataSeeder.cs` | Eliminar bloques de seed `Countries`/`Cities`, métodos `SeedCountriesAsync` / `SeedCitiesAsync`, propiedades `CountrySeed`/`CitySeed` en modelos embebidos del JSON; mantener **D5** (ids blandos u omitidos). |
| 8.1 | Modificar | Fuentes JSON de seed si existen bajo rutas referenciadas por `JsonDataSeeder` | Quitar arrays geo obsoletos. |
| 8.2 | Modificar | `src/Api/Services/SetupService.cs` | Sustituir resolución por `_context.States` / `Cities` / `PostalCodes` (líneas que buscan Barcelona, Madrid, etc.) por enfoque sin tablas locales: ids fijos documentados, Admin, o omisión de demo geo según clarify §2.6. |
| 8.3 | Modificar | `src/Infrastructure/Services/MasterDataSeeder.cs` | Eliminar o reemplazar `SeedSpainDataAsync` y lógica que inserta `Country`/`State`/`City`/`PostalCode` locales; mantener `SeedLanguagesAsync` si sigue siendo necesario para Product. |

---

## Fase 9: Tests

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 9.1 | Modificar / eliminar | `src/IntegrationTests/Controllers/CountryControllerTests.cs` | Eliminar o sustituir por pruebas que no dependan de CRUD geo en Product. |
| 9.1 | Modificar | `src/IntegrationTests/Controllers/StateControllerTests.cs` | Igual. |
| 9.1 | Modificar | `src/IntegrationTests/Controllers/CityControllerTests.cs` | Igual. |
| 9.1 | Modificar | Tests de integración que asertan sobre API geo local | Actualizar escenarios. |
| 9.2 | Crear / modificar | `src/tests/GesFer.Product.UnitTests/` | Tests unitarios del validador geo con mock del cliente Admin (§5.2 clarify — mock vs dataset fijo). |

---

## Fase 10: Documentación

| Id | Acción | Ruta | Ubicación / propuesta |
|----|--------|------|------------------------|
| 10.1 | Crear | `docs/architecture/geolocalizacion-contract.md` | SSOT comportamiento Product ↔ Admin (paralelo a `companies-contract.md`). |
| 10.2 | Crear / completar | `docs/features/refactorization-geolocalizacion-admin-ssot/validacion.md` | Según proceso de validación del repo. |

---

## Resumen por archivo (archivos más tocados)

| Archivo / área | Fases |
|----------------|-------|
| `IAdminApiClient.cs`, `AdminApiClient.cs`, `MockAdminApiClient.cs` | 1 |
| `ApplicationDbContext.cs`, `Migrations/*`, `*Configuration.cs` (geo + User/Customer/Supplier) | 3, 4 |
| Entidades dominio `Country|State|City|PostalCode`, `Language`, `User`, `Customer`, `Supplier` | 2, 4 |
| Handlers create/update User, Customer, Supplier | 5 |
| Controllers geo + 20 handlers CRUD geo + comandos/DTOs geo | 6 |
| `LoginCommandHandler.cs`, `LoginResponseDto.cs` | 7 |
| `JsonDataSeeder.cs`, `SetupService.cs`, `MasterDataSeeder.cs` | 8 |
| `IntegrationTests/Controllers/*Geo*` | 9 |

---

## Orden sugerido de aplicación en código

`1` (cliente) → `2` + `3` + `4` (modelo y BD coherentes) → `5` (validación) → `6` (eliminar superficie) → `7` → `8` → `9` → `10`.

Paralelizable: documentación `10.1` cuando el contrato de código esté estable.

---

*Generado en acción **implementation**: solo documentación; la ejecución corresponde a **execution**.*
