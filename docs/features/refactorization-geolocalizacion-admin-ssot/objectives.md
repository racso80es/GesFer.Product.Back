---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
updated: '2026-03-21'
process: refactorization
branch: feat/refactorization-geolocalizacion-admin-ssot
contract_ref: SddIA/norms/features-documentation-pattern.md
admin_geo_contract: docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json
---

# Objetivos

## Objetivo

Adecuar el contexto **Product** para que las entidades de **geolocalización** (Country, State, City, PostalCode) **no** se persistan ni gestionen en Product. **Admin** es la única fuente de verdad (SSOT). Product solo **almacena identificadores** (Guid) en entidades de negocio que necesiten dirección y **consume la API de lectura de Admin** bajo `/api/geolocation` — alineado con el contrato exportado en [`admin-geolocation-contract.json`](./admin-geolocation-contract.json) y con el mismo criterio arquitectónico que el desacople de **Company** (`docs/architecture/companies-contract.md`, feature `kaizen-desacople-companies`).

## Alcance

- **Dominio Product:** Eliminar entidades de dominio Country, State, City, PostalCode; tablas y configuraciones EF asociadas; `DbSet` en `ApplicationDbContext`; migraciones que eliminen esas tablas y las FK desde **toda** entidad que hoy referencie geo (`User`, `Customer`, `Supplier` y las que existan al ejecutar la migración).
- **API Product:** Eliminar controladores y handlers CRUD locales de país/provincia/ciudad/código postal que lean/escriban BD. El backend Product **debe** consumir Admin para geo ([`clarify.md`](./clarify.md) **D1**). Los catálogos jerárquicos están en **Admin** (`GET /api/geolocation/...`). El front puede llamar también a Admin para UI; proxy/BFF en Product solo si se decide más adelante por CORS/origen único.
- **Cliente HTTP:** Implementar llamadas a los cinco GET documentados (mismos DTOs que Admin) usando `AdminApi:BaseUrl`. **Comportamiento por defecto y persistente:** cabecera **`X-Internal-Secret`** con valor de `InternalSecret` en configuración (igual que `IAdminApiClient`) — [`clarify.md`](./clarify.md) **D2**.
- **Seeds:** Eliminar de `JsonDataSeeder` y de `SetupService` la siembra de Countries, States, Cities, PostalCodes en Product. Los ids geo que queden en seeds de Product son **sin integridad referencial**; si el seed va **vacío** en geo, no debe fallar el arranque — [`clarify.md`](./clarify.md) **D5**, §3.2.
- **Comandos de creación/actualización:** Para **cada** entidad con campos geo (`User`, `Customer`, `Supplier` y futuras), validación **obligatoria** frente a Admin cuando el DTO incluya esos campos — [`clarify.md`](./clarify.md) **D6**, **D7**.
- **Referencias en entidades:** En **todas** las entidades que usen geo, mantener `CountryId`, `StateId`, `CityId`, `PostalCodeId` como **columnas sin FK**; valores = **Guid de Admin** — [`clarify.md`](./clarify.md) **D6**, **D7**, §3 y §3.0.
- **Login / idioma:** Simplificación acordada: contexto efectivo **español (`es`)** por ahora; sin `CountryLanguageId` desde país local. Evolución a i18n completo en **TODO-i18n-context** / **TODO-default-lang-ES** — [`clarify.md`](./clarify.md) **D3**, **D4**, §4.

## Fuente de contrato Admin (congelada para esta feature)

| Artefacto | Rol |
|-----------|-----|
| [`admin-geolocation-contract.json`](./admin-geolocation-contract.json) | SSOT de rutas, códigos HTTP, DTOs y ejemplos JSON (origen: `GeoDtos.cs` + `GeolocationController.cs` en Admin). |
| Swagger `http://localhost:5010/index.html` | Verificación visual; debe coincidir con el JSON; si difiere, actualizar el JSON y el spec. |

**Lectura:** solo registros **activos**; jerarquía **país → estado → ciudad → código postal** vía encadenamiento de endpoints GET.

## Leyes aplicables

- **SOBERANÍA:** `docs/` y `SddIA/` como SSOT documental; contrato de integración en la carpeta de esta feature + arquitectura al cierre.
- **COMPILACIÓN:** Build y tests verdes tras cada fase acotada.
- **GIT:** Rama `feat/refactorization-geolocalizacion-admin-ssot`; sin commits en `master` (ley AGENTS).

## Referencias en código actual (inventario orientativo)

| Área | Indicación |
|------|------------|
| Entidades | `Country`, `State`, `City`, `PostalCode`; `Language.Countries` navegación inversa |
| EF | `ApplicationDbContext` DbSets; `*Configuration.cs` para geo |
| API | `CountryController`, `StateController`, `CityController`, `PostalCodeController` |
| Handlers | `application/Handlers/Country`, `State`, `City`, `PostalCode` |
| Agregados | `User`, `Customer`, `Supplier` (y cualquier otra con columnas geo) con FKs/navegación a geo hoy |
| Seeds | `JsonDataSeeder` (`SeedCountriesAsync`, `SeedCitiesAsync`, JSON de countries/cities) |
| Setup | `SetupService` (resolución Barcelona/Madrid/etc. vía tablas locales) |
| Tests | `IntegrationTests` que crean países vía API Product |
| Init | `InitDatabase.cs` orden de tablas incluye geo |

---

# Análisis preliminar

## Situación actual

Product posee modelo relacional completo de geolocalización (tablas, navegaciones, CRUD) y lo usa para validar existencia y semillas.

## Estado deseado

- Sin tablas de geo en la BD de Product.
- Validación / enriquecimiento mediante **GET** a Admin según [`admin-geolocation-contract.json`](./admin-geolocation-contract.json).
- Catálogo UI: consumo desde **Admin**; Product backend siempre integra Admin (**D1**).

## Riesgos / decisiones abiertas

Ver **[`clarify.md`](./clarify.md) §5** (algoritmo de validación jerárquica; dataset de ids en CI). Decisiones **D1–D7** cerradas en §2 salvo matices de §5.

## Próximos artefactos del proceso

1. Opcional: cerrar **§5.1** y **§5.2** en `clarify.md` cuando se fije criterio de validación y seeds.
2. `plan.md` — Orden de migración y tareas.
3. `implementation.md` / `execution.md` / `validacion.md` — Ciclo de acciones.
