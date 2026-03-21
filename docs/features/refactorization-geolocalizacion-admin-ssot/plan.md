---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
process: refactorization
branch: feat/refactorization-geolocalizacion-admin-ssot
phases:
  - id: pre
    name: Rama feat y baseline documental
  - id: '1'
    name: Cliente HTTP Admin geo (DTOs + X-Internal-Secret)
  - id: '2'
    name: Eliminar entidades de dominio geo y navegaciones
  - id: '3'
    name: Migración EF (eliminar tablas y FKs)
  - id: '4'
    name: Entidades agregadas (User, Customer, Supplier) sin FK ni navegación
  - id: '5'
    name: Validación Admin en handlers create/update (D6)
  - id: '6'
    name: Retirar API CRUD geo y handlers locales
  - id: '7'
    name: Login, idioma (D3/D4) y TODOs i18n
  - id: '8'
    name: Seeds, SetupService, MasterDataSeeder (D5)
  - id: '9'
    name: Tests e integración
  - id: '10'
    name: Documentación arquitectura y validación
spec_ref: docs/features/refactorization-geolocalizacion-admin-ssot/spec.md
clarify_ref: docs/features/refactorization-geolocalizacion-admin-ssot/clarify.md
contract_ref: docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json
---

# Plan: Refactorización geolocalización — Admin SSOT

Referencias: [`spec.md`](./spec.md), [`clarify.md`](./clarify.md), [`admin-geolocation-contract.json`](./admin-geolocation-contract.json). Decisiones **D1–D7** en clarify.

---

## Fase pre: Rama y baseline

**Skill:** `iniciar-rama` (paths.skillCapsules.iniciar-rama).

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| pre.1 | Crear rama `feat/refactorization-geolocalizacion-admin-ssot` desde main/master actualizado | Rama activa |
| pre.2 | Commit inicial de documentación (objectives, spec, clarify, contrato JSON, plan) | Historial trazable |

**Invocación:** `.\scripts\skills\iniciar-rama\Iniciar-Rama.bat feat refactorization-geolocalizacion-admin-ssot`

**Criterio:** No implementar en `master`; documentación persistida en la rama feat.

---

## Fase 1: Cliente HTTP Admin — geolocalización

| Tarea | Descripción | Entregable |
|-------|-------------|------------|
| 1.1 | Añadir DTOs de lectura alineados al contrato (`CountryGeoReadDto`, etc.) | `Infrastructure/DTOs` o subcarpeta Geo |
| 1.2 | Extender `IAdminApiClient` o interfaz dedicada con los 5 GET de `/api/geolocation/*` | Métodos async + rutas relativas a `AdminApi:BaseUrl` |
| 1.3 | Reutilizar envío de **`X-Internal-Secret`** (`InternalSecret`) como en `AdminApiClient` — **D2** | Headers en `HttpClient` compartido o factory |
| 1.4 | `MockAdminApiClient` / tests: respuestas según `examples` del JSON | Entorno `Testing` |

**Criterio:** Llamadas compilables; 404/500 manejados donde aplique.

---

## Fase 2: Eliminar entidades de dominio geo

| Tarea | Descripción |
|-------|-------------|
| 2.1 | Eliminar `Country.cs`, `State.cs`, `City.cs`, `PostalCode.cs` |
| 2.2 | Quitar `ICollection<Country>` de `Language.cs` y referencias |

**Criterio:** Dominio sin tipos geo locales.

---

## Fase 3: Migración EF

| Tarea | Descripción |
|-------|-------------|
| 3.1 | Quitar `DbSet` y `ApplyConfigurations` de entidades geo |
| 3.2 | Eliminar archivos `*Configuration` de Country, State, City, PostalCode |
| 3.3 | Generar migración: drop FKs desde User, Customer, Supplier → tablas geo; drop tablas `Countries`, `States`, `Cities`, `PostalCodes` |

**Criterio:** `dotnet ef` (vía proceso/skill acordado) sin errores; BD coherente con modelo.

---

## Fase 4: Agregados con dirección (todas las entidades — D6, D7)

| Tarea | Descripción |
|-------|-------------|
| 4.1 | `User`, `Customer`, `Supplier`: quitar navegaciones `Country`, `State`, `City`, `PostalCode` |
| 4.2 | Actualizar `CustomerConfiguration`, `SupplierConfiguration`, `UserConfiguration`: sin `HasOne` hacia geo |

**Criterio:** Columnas Guid opcionales persistidas; sin FK a tablas eliminadas.

---

## Fase 5: Validación en comandos (create/update)

| Tarea | Descripción |
|-------|-------------|
| 5.1 | Servicio helper o extensiones: validar existencia/coherencia jerárquica vía GET Admin (definir algoritmo en §5.1 de clarify) |
| 5.2 | `CreateUser`, `UpdateUser`, `CreateCustomer`, `UpdateCustomer`, `CreateSupplier`, `UpdateSupplier`: invocar validación cuando DTO traiga ids geo |

**Criterio:** Combinaciones inválidas rechazadas con error claro — **D6**.

---

## Fase 6: Retirar superficie API geo local

| Tarea | Descripción |
|-------|-------------|
| 6.1 | Eliminar o desregistrar `CountryController`, `StateController`, `CityController`, `PostalCodeController` y comandos/handlers asociados |
| 6.2 | Limpiar `RegisterCommandHandlers` / DI si aplica |

**Criterio:** Product no expone CRUD de catálogo geo propio.

---

## Fase 7: Login e idioma

| Tarea | Descripción |
|-------|-------------|
| 7.1 | `LoginCommandHandler`: quitar dependencia de `user.Country`; aplicar **D3/D4** (español por defecto) |
| 7.2 | Comentarios o enlaces a **TODO-i18n-context** / **TODO-default-lang-ES** |

---

## Fase 8: Seeds y setup

| Tarea | Descripción |
|-------|-------------|
| 8.1 | `JsonDataSeeder`: eliminar seed de countries/cities y JSON embebido obsoleto — **D5**, §3.2 |
| 8.2 | `SetupService` / demos: sin consultas a tablas geo locales |
| 8.3 | Revisar `MasterDataSeeder` u otros seeders que usen `CountryId` en entidades geo eliminadas |

**Criterio:** Seed no falla con bloque geo vacío; ids en seed son blandos si existen.

---

## Fase 9: Tests

| Tarea | Descripción |
|-------|-------------|
| 9.1 | Ajustar `IntegrationTests` que crean país/estado vía API Product |
| 9.2 | Tests unitarios de validación geo con mock del cliente Admin |

---

## Fase 10: Documentación y cierre de feature

| Tarea | Descripción |
|-------|-------------|
| 10.1 | Añadir `docs/architecture/geolocalizacion-contract.md` (SSOT comportamiento Product ↔ Admin) |
| 10.2 | Completar `validacion.md` según proceso feature |

---

## Orden recomendado de ejecución

`pre` → **1** (cliente) → **2** + **3** + **4** (modelo y BD) → **5** (validación) → **6** (API) → **7** → **8** → **9** → **10**.

Paralelizable: documentación 10.1 tras estabilizar contrato de código.

---

## Riesgos

- Admin debe aceptar `X-Internal-Secret` en rutas `/api/geolocation/*` (**D2**).
- Validación jerárquica puede ser costosa en latencia; valorar caché corta o batch en iteraciones posteriores.
