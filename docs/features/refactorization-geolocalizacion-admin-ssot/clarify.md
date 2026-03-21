---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
updated: '2026-03-21'
contract_ref: admin-geolocation-contract.json
status: decisiones_casi_cerradas
---

# Clarify — Geolocalización Admin SSOT

Documento de **aclaraciones**: contrato Admin, **decisiones ya tomadas** y puntos que siguen abiertos antes de `plan.md` y ejecución.

---

## 1. Congelado (contrato Admin — lectura)

**Fuente canónica en este repo:** [`admin-geolocation-contract.json`](./admin-geolocation-contract.json) (export `1.0.0`, generado desde `GeoDtos.cs` + `GeolocationController.cs` en Admin).

| Decisión | Valor |
|----------|--------|
| Base path | `/api/geolocation` |
| Verbos | Solo **GET** (catálogo de lectura; solo registros **activos**) |
| Autorización | `AuthorizeSystemOrAdmin` — mismo criterio que otros catálogos protegidos en Admin |
| Jerarquía API | país → estados del país → ciudades del estado → códigos postales de la ciudad |

### Endpoints (resumen)

| Método | Ruta | Respuesta |
|--------|------|-----------|
| GET | `/api/geolocation/countries` | `CountryGeoReadDto[]` |
| GET | `/api/geolocation/countries/{countryId}` | `CountryGeoReadDto` (404 con `{ "message": "string" }`) |
| GET | `/api/geolocation/countries/{countryId}/states` | `StateGeoReadDto[]` |
| GET | `/api/geolocation/states/{stateId}/cities` | `CityGeoReadDto[]` |
| GET | `/api/geolocation/cities/{cityId}/postal-codes` | `PostalCodeGeoReadDto[]` |

### DTOs (nombres Admin)

- **CountryGeoReadDto:** `id`, `name`, `code`
- **StateGeoReadDto:** `id`, `countryId`, `name`, `code` (opcional/null)
- **CityGeoReadDto:** `id`, `stateId`, `name`
- **PostalCodeGeoReadDto:** `id`, `cityId`, `code`

En Product, los tipos en `Infrastructure/DTOs` deben mapear **camelCase JSON** tal como en el contrato (p. ej. propiedades C# en PascalCase con `[JsonPropertyName]` si hace falta).

---

## 2. Decisiones cerradas (producto / integración)

| ID | Decisión | Detalle |
|----|----------|---------|
| **D1** | Product consume Admin para geo | El backend **Product** debe usar la **API Admin** (`/api/geolocation/*` según contrato). No es opcional: integración explícita vía cliente HTTP (mismo eje que company/logs). El front puede además llamar a Admin directamente para UI; eso no exime a Product de validar/persistir referencias coherentes con Admin cuando aplique la capa de aplicación. |
| **D2** | Autenticación Product → Admin (por defecto y comportamiento persistente) | Por defecto y como norma documentada: comunicación **Product → Admin** mediante cabecera **`X-Internal-Secret`** (mismo mecanismo que `IAdminApiClient` / `InternalSecret` en configuración). Nombre del header en runtime: `X-Internal-Secret` (guión; no `X-Internal_Secret`). Las rutas geo deben aceptar el mismo esquema `AuthorizeSystemOrAdmin` que el resto de catálogos internos en Admin; si en algún entorno Admin exigiera solo JWT, documentar excepción en despliegue. |
| **D3** | Idioma en login / respuesta auth (simplificación) | **Por ahora** se simplifica: contexto de idioma efectivo **español** (`es`). No se depende de `CountryLanguageId` desde país local. Los campos de login que reflejaban idioma por país se alinean a esta simplificación (p. ej. `CountryLanguageId` / efectivos pueden fijarse o derivarse como **es** hasta el trabajo de i18n completo). |
| **D4** | Idioma maestro en dominio Product | De acuerdo con D3: **idioma por defecto ES (español)** en el comportamiento actual; la evolución hacia modelo de idioma completo (Admin, claims, `LanguageId` por usuario) queda explícita en **TODO** (ver §4). |
| **D5** | Seeds en Product e ids geo | Los ids dependientes de geolocalización en **seeds de Product** son **referencias blandas** a Admin: **sin integridad referencial** en BD (no hay FK). Los valores deben coincidir con ids existentes en Admin **si** se incluyen en seed; Product no valida en migración/insert masivo más allá de lo que defina el propio job de seed. |
| **D6** | Validación en aplicación — **todas** las entidades con geo | **Cualquier** entidad de dominio que persista `CountryId`, `StateId`, `CityId` y/o `PostalCodeId` (uno o varios; todos opcionales salvo regla de negocio específica) debe seguir **la misma estructura**: sin FK local; en **crear y actualizar**, si el comando/DTO incluye alguno de esos campos, deben ser **válidos frente a Admin** (existencia y coherencia jerárquica según implementación). No se persisten combinaciones inválidas; error explícito. *Entidades actuales en el modelo:* `User`, `Customer`, `Supplier`. *Entidades futuras:* mismo contrato; actualizar esta documentación si se añaden campos o reglas nuevas. |
| **D7** | Regla de alcance (sin excepciones silenciosas) | No se introducen patrones alternativos de geolocalización en Product (tablas locales, FK a geo, validación solo en front) salvo nueva spec que sustituya o complemente esta feature. |

---

## 3. ¿Qué son los `id` de geolocalización?

No son inventados en Product: son **UUID (`Guid`) emitidos por Admin** como claves primarias del catálogo geo.

### 3.0 Entidades de Product que llevan datos geo

| Entidad (estado actual) | Campos |
|-------------------------|--------|
| `User` | `CountryId`, `StateId`, `CityId`, `PostalCodeId` (opcionales) |
| `Customer` | Idem |
| `Supplier` | Idem |

Cualquier **nueva** entidad que requiera dirección debe usar el **mismo patrón** (D6, D7, §3.1–§3.2).

| Campo almacenado en Product (sin FK local) | Origen |
|------------------------------------------|--------|
| `CountryId` | `CountryGeoReadDto.id` de Admin |
| `StateId` | `StateGeoReadDto.id` |
| `CityId` | `CityGeoReadDto.id` |
| `PostalCodeId` | `PostalCodeGeoReadDto.id` |

**Persistencia:** Product guarda solo el **Guid**; la verdad descriptiva (`name`, `code`, jerarquía) está en Admin.

### 3.1 Seeds de Product (sin integridad referencial)

- Los seeds que rellenen `CountryId` / `StateId` / `CityId` / `PostalCodeId` lo hacen como **datos de conveniencia**, sin FK: la BD de Product **no** garantiza que existan en Admin.
- **Operativa deseada:** alimentar seeds con ids coherentes con un Admin ya sembrado, o **omitir** esos campos.

### 3.2 Seeds vacíos (sin ids geo)

- Si el seed **no** define ids geo (configuración vacía o sin bloque geo), el comportamiento es: **no fallar** el seed por ello; no rellenar dirección geo de demostración (o dejar `null` según reglas del seeder).
- La **exigencia de validez** no recae en el seed masivo, sino en los **flujos de aplicación** que crean o modifican entidades con geo (véase **D6**).

### 3.3 Tests / CI

Los ids **no** son estables salvo dataset fijo en Admin. Opciones: mock del cliente geo; o lista blanca documentada. Ver **§5.2** si se publica catálogo de referencia.

---

## 4. TODO explícitos (deuda / siguiente iteración)

| TODO | Descripción |
|------|-------------|
| **TODO-i18n-context** | Adecuar **contexto de idioma** completo (más allá de español por defecto): claims, `User.LanguageId`, alineación con Admin si aplica, y eliminación de la simplificación “siempre es” de D3/D4. |
| **TODO-default-lang-ES** | Mantener visible en implementación que el **idioma por defecto** del comportamiento actual es **español (`es`)** hasta resolver TODO-i18n-context. |

Referencias cruzadas en código: al implementar login/handlers, enlazar comentarios o tareas a **TODO-i18n-context** / **TODO-default-lang-ES** para no perder el alcance temporal de D3/D4.

---

## 5. Preguntas aún abiertas

### 5.1 Detalle de implementación de la validación geo

**Cerrado a nivel de regla:** **D6** aplica a **User**, **Customer**, **Supplier** y a **cualquier** entidad futura con los mismos campos.

**Pendiente (técnico):** Algoritmo concreto para comprobar **coherencia jerárquica** con los solo-GET de Admin (orden de llamadas, short-circuit en 404, mensajes de error). No afecta al alcance de entidades.

---

### 5.2 Seeds y CI — dataset de ids de referencia

**Pendiente:** ¿Se publicará un conjunto de **ids Guid fijos** (tras seed de Admin) para desarrollo local y CI, o se asume **mock** del cliente geo en integración? (Complementa **§3.3**; **D5** y **§3.2** ya cubren seeds vacíos y ausencia de FK.)

---

## 6. Próximo paso

- Redactar **`plan.md`** con **D1–D7** y §3 asumidos.
- Resolver **§5.1** (algoritmo de validación) y **§5.2** durante implementación si aplica.
- Ejecutar **TODO-i18n-context** / **TODO-default-lang-ES** en una iteración posterior o en la misma rama si se tocan login DTOs.
