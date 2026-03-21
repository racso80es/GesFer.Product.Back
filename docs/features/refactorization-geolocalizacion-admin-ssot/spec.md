---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
updated: '2026-03-21'
base: origin/main
scope: src/domain, src/Infrastructure, src/Api, src/application, src/IntegrationTests, docs/architecture
contract_ref: SddIA/norms/features-documentation-pattern.md
admin_geo_contract_file: docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json
related_architecture: docs/architecture/companies-contract.md
---

# SPEC: Geolocalización — Admin SSOT (Product consumidor)

## Contexto

La responsabilidad de **Country**, **State**, **City** y **PostalCode** está en el contexto **Admin**. Product **no** crea tablas ni semillas de catálogo geográfico; conserva **Guid** en entidades de negocio y obtiene datos de catálogo mediante la API de lectura documentada.

**Contrato canónico (esta feature):** [`admin-geolocation-contract.json`](./admin-geolocation-contract.json) (`exportVersion` 1.0.0). Origen en código Admin: `GesFer.Admin.Back.Application/DTOs/Geo/GeoDtos.cs`, `GeolocationController.cs`.

**Principio:** Mismo patrón que **Company** — sin tabla local de catálogo; columnas referenciales sin FK a Admin.

---

## Punto 1: Contrato HTTP con Admin (cerrado)

**Descripción:** Product debe integrarse exclusivamente con estos recursos (todos **GET**, `Content-Type: application/json; charset=utf-8`):

| Ruta | Respuesta exitosa | Notas |
|------|-------------------|--------|
| `/api/geolocation/countries` | `CountryGeoReadDto[]` | 200, 500 |
| `/api/geolocation/countries/{countryId}` | `CountryGeoReadDto` | 200, 404 (`{ "message": "string" }`), 500 |
| `/api/geolocation/countries/{countryId}/states` | `StateGeoReadDto[]` | 200, 500 |
| `/api/geolocation/states/{stateId}/cities` | `CityGeoReadDto[]` | 200, 500 |
| `/api/geolocation/cities/{cityId}/postal-codes` | `PostalCodeGeoReadDto[]` | 200, 500 |

**Autorización (Admin):** `AuthorizeSystemOrAdmin`. En Product, llamadas **Product → Admin** usan por defecto **`X-Internal-Secret`** (`InternalSecret` en appsettings), mismo criterio que company/logs — [`clarify.md`](./clarify.md) **D2**. Excepciones por entorno se documentan en despliegue.

**DTOs (lectura):**

- **CountryGeoReadDto:** `id`, `name`, `code`
- **StateGeoReadDto:** `id`, `countryId`, `name`, `code` (opcional)
- **CityGeoReadDto:** `id`, `stateId`, `name`
- **PostalCodeGeoReadDto:** `id`, `cityId`, `code`

Esquema JSON detallado y ejemplos: ver `jsonSchema` y `examples` en [`admin-geolocation-contract.json`](./admin-geolocation-contract.json).

**Criterio de aceptación:** Cliente en Product implementa llamadas a estas rutas; DTOs C# compatibles con serialización JSON del contrato; tests con respuestas alineadas a `examples`.

---

## Punto 2: Eliminar entidades de dominio de geolocalización en Product

**Descripción:** Eliminar archivos de entidad:

- `Country.cs`, `State.cs`, `City.cs`, `PostalCode.cs`

**Ajustes relacionados:**

- `Language.cs`: eliminar `ICollection<Country> Countries` (y referencias obsoletas).

**Criterio de aceptación:** No existen tipos de dominio `Country|State|City|PostalCode` en `src/domain/Entities/`.

---

## Punto 3: Eliminar persistencia EF y tablas

**Descripción:**

- Quitar `DbSet` de Countries, States, Cities, PostalCodes de `ApplicationDbContext`.
- Eliminar configuraciones `CountryConfiguration`, `StateConfiguration`, `CityConfiguration`, `PostalCodeConfiguration`.
- Generar **migración** que elimine FKs desde entidades de negocio hacia tablas de geo y luego elimine tablas `Countries`, `States`, `Cities`, `PostalCodes` (orden según dependencias).

**Criterio de aceptación:** Snapshot de modelo EF sin entidades de geo; BD aplicada sin esas tablas.

---

## Punto 4: Columnas sin FK en **todas** las entidades que guardan dirección

**Descripción:** Aplica a **cualquier** entidad de dominio que persista datos de geolocalización (hoy `User`, `Customer`, `Supplier`; mismas reglas para entidades nuevas — [`clarify.md`](./clarify.md) **D6**, **D7**, §3.0):

- Mantener `PostalCodeId`, `CityId`, `StateId`, `CountryId` como **nullable Guid** (salvo regla de negocio explícita) sin navegación a entidades eliminadas.
- Quitar propiedades de navegación `PostalCode`, `City`, `State`, `Country`.
- Actualizar configuraciones EF: sin relaciones hacia geo; índices opcionales si aportan valor.

**Criterio de aceptación:** Migración y configuraciones coherentes; ninguna FK a tablas de geo en ninguna entidad que use estos campos.

---

## Punto 5: Cliente Admin para geolocalización

**Descripción:**

- Añadir métodos que cubran los cinco GET del Punto 1 (relativos a base `AdminApi:BaseUrl`), reutilizando el mismo `HttpClient` / registro DI que `IAdminApiClient` cuando sea viable.
- **Cabeceras:** Enviar **`X-Internal-Secret`** desde configuración (`InternalSecret`) como comportamiento estándar — [`clarify.md`](./clarify.md) **D2**.
- Crear DTOs en `Infrastructure/DTOs` (p. ej. prefijo `Admin` o `Geo`) con propiedades equivalentes a `CountryGeoReadDto`, etc.

**Criterio de aceptación:** Mock del cliente en entorno `Testing`; pruebas que validen deserialización según `jsonSchema` del contrato.

---

## Punto 6: Sustituir lógica que usaba DbContext de geo

**Descripción:**

- Handlers de **crear/actualizar** de **toda** entidad con campos geo (`User`, `Customer`, `Supplier`, etc.): validación **obligatoria** de ids frente a Admin cuando el DTO incluya esos campos — [`clarify.md`](./clarify.md) **D6**. Detalle algorítmico: [`clarify.md`](./clarify.md) §5.1.
- Seeds: ids geo **sin FK**; seed vacío en geo permitido — **D5**, §3.2 de clarify.
- Handlers que exponían nombres de ciudad/país desde navegación: obtener textos vía Admin o contrato solo con ids según API pública de Product.
- **`LoginCommandHandler`:** sin navegación `Country`; idioma efectivo simplificado a **es** según [`clarify.md`](./clarify.md) **D3** / **D4** y TODOs §4.
- Eliminar comandos/handlers/controladores CRUD de geo en Product (salvo decisión futura de proxy/BFF documentada aparte).

**Criterio de aceptación:** Sin `_context.Countries`, `States`, `Cities`, `PostalCodes` en aplicación.

---

## Punto 7: Seeds y SetupService

**Descripción:**

- `JsonDataSeeder`: eliminar seeds de countries/cities y JSON asociado.
- `SetupService`: eliminar resolución por tablas locales; dependencia de datos Admin documentada ([`clarify.md`](./clarify.md) §2.6).

**Criterio de aceptación:** Seeds no insertan geo local; entornos documentados.

---

## Punto 8: Tests y documentación

**Descripción:**

- Actualizar integración que asumía API Product de geo o datos locales.
- Añadir `docs/architecture/geolocalizacion-contract.md` (paralelo a `companies-contract.md`) al cerrar diseño.

**Criterio de aceptación:** Tests verdes; doc de arquitectura actualizada.

---

## Dependencias externas

- API Admin desplegada con rutas y auth acordes a [`admin-geolocation-contract.json`](./admin-geolocation-contract.json) y aceptación de **`X-Internal-Secret`** en rutas geo (D2).
- [`clarify.md`](./clarify.md) §5 para algoritmo de validación jerárquica y dataset de ids en CI.

---

## Fuera de alcance (salvo nueva spec)

- Exponer en Admin endpoints distintos a los del contrato (cambio en Admin, no en esta spec).
- Mover **Language** a Admin — ver [`clarify.md`](./clarify.md) §2.5.
