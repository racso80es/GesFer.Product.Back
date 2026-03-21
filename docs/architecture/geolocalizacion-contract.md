---
title: Contrato geolocalización (Admin SSOT)
created: '2026-03-21'
feature_ref: docs/features/refactorization-geolocalizacion-admin-ssot
---

# Contrato: Geolocalización — Admin SSOT

## Principio

**Admin** es la única fuente de verdad (SSOT) para **Country**, **State**, **City** y **PostalCode** (catálogo de lectura). **Product** no mantiene tablas de catálogo geo ni FKs hacia ellas.

## Acceso desde Product

- **Lectura / validación:** `IAdminApiClient` — métodos bajo `api/geolocation/*` (ver contrato JSON en la feature).
- **Validación en comandos:** `IAdminGeolocationValidationService` — coherencia jerárquica al crear/actualizar entidades con `CountryId`, `StateId`, `CityId`, `PostalCodeId`.
- **Cabecera:** `X-Internal-Secret` (`InternalSecret` en configuración), mismo criterio que otras llamadas Product → Admin.

## Columnas en Product

- **Tipo:** `Guid?` opcionales en entidades que guardan dirección (`User`, `Customer`, `Supplier`, y futuras según spec).
- **Sin FK local:** No hay navegación ni restricción referencial en BD; los valores son referencias blandas a ids emitidos por Admin.

## Documentación de API

- Contrato canónico en el repo: `docs/features/refactorization-geolocalizacion-admin-ssot/admin-geolocation-contract.json`.
