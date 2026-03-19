---
title: Contrato Companies (Admin SSOT)
created: '2026-03-19'
feature_ref: docs/features/kaizen-desacople-companies
---

# Contrato: Companies — Admin SSOT

## Principio

**Admin** es la única fuente de verdad (SSOT) para la entidad **Company**. El contexto **Product** no gestiona ni almacena Companies.

## Acceso desde Product

- **Único canal:** IAdminApiClient (GetCompanyAsync, GetCompanyByNameAsync, UpdateCompanyAsync)
- **Autenticación:** JWT con CompanyId en claims; header X-Internal-Secret para llamadas a Admin API
- **Configuración:** appsettings (AdminApi:BaseUrl, InternalSecret)

## CompanyId en Product

- **Tipo:** Columna Guid en entidades (User, Article, TaxType, etc.)
- **Sin FK:** No hay tabla Companies ni FK en Product; CompanyId es dato referencial
- **Origen:** Siempre del claim JWT del usuario autenticado; **nunca del frontend**
- **Validación:** Handlers validan CompanyId vía Admin API antes de persistir

## Endpoints Product

Los controllers que operan sobre entidades con CompanyId obtienen CompanyId del claim. El cliente no envía CompanyId en query ni body.
