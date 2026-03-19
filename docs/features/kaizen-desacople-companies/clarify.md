---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
purpose: Clarificaciones y decisiones preliminares
---

# Clarificaciones

## Decisiones tomadas

### 1. CompanyId sin FK

**Decisión:** CompanyId permanece como columna en las entidades Product, pero **sin FK** a tabla Companies. La integridad referencial se valida en capa de aplicación mediante llamadas a Admin API (p. ej. CreateUserCommandHandler valida con GetCompanyAsync).

**Motivo:** Product y Admin tienen BDs separadas (Product: GesFer_Product; Admin: contexto propio). No es posible tener FK entre BDs distintas. CompanyId es un dato referencial.

### 2. Tabla Companies eliminada de Product

**Decisión:** Eliminar completamente la tabla Companies de la BD Product. No mantener tabla vacía ni "stub".

**Motivo:** SSOT en Admin; Product no debe gestionar Companies.

### 3. Acceso a compañía del usuario actual

**Decisión:** Siempre vía IAdminApiClient. El CompanyId se obtiene del claim JWT del usuario autenticado. Los controllers que necesiten CompanyId deben usar `User.FindFirst("CompanyId")?.Value` o equivalente.

**Motivo:** Coherencia con MyCompanyController y flujo actual.

### 4. Handlers con CompanyId en comando

**Decisión:** Los comandos (CreateTaxTypeCommand, etc.) pueden recibir CompanyId como parámetro, pero el **controller** debe pasar el CompanyId del usuario autenticado, no un valor arbitrario del cliente. Evitar que el cliente envíe CompanyId en el body para operaciones de su propio contexto.

**Aclarar en spec:** Revisar endpoints que acepten CompanyId en query/body; restringir al claim cuando sea operación del usuario actual.

---

## Respuestas del usuario (clarificación ejecutada)

### 1. Filtro por CompanyId y origen del dato

**Respuesta:** Sí. Si la tabla dispone de CompanyId, se ha de filtrar por ella. La API Product ha de tener persistido el CompanyId del usuario autenticado. **CompanyId no ha de ser dado por el frontend.**

**Decisión:** Todos los endpoints que operen sobre entidades con CompanyId deben obtener CompanyId del claim JWT del usuario autenticado. El controller inyecta CompanyId; el cliente no envía CompanyId en query ni body.

### 2. Seeds y responsabilidad de Admin

**Respuesta:** Sí. Será responsabilidad de Admin que existan esas empresas. Si no existen, Product ha de contemplar esto como posible error.

**Decisión:** Seed:CompanyId sigue siendo lista blanca. Product valida contra Admin API (o asume que Admin tiene las empresas). Si una empresa no existe en Admin, Product debe manejar el error (log, descarte, o fallo claro).

### 3. Integración Admin y AdminApiClient

**Respuesta:** Sí, appsettings, junto con los datos de autenticación necesarios. Revisar situación actual de AdminApiClient.

**Decisión:** Configuración en appsettings; validada la situación actual (ver sección siguiente).

---

## Situación actual de AdminApiClient (revisión)

### Configuración (appsettings)

| Archivo | AdminApi | Autenticación |
|---------|----------|---------------|
| `appsettings.json` | BaseUrl: `http://localhost:5001`; LogsEndpoint, AuditLogsEndpoint | — |
| `appsettings.Development.json` | BaseUrl: `http://localhost:5010` | InternalSecret: `dev-internal-secret-change-in-production` |

### Implementación

- **IAdminApiClient:** GetCompanyAsync(Guid), GetCompanyByNameAsync(string), UpdateCompanyAsync(Guid, Dto).
- **AdminApiClient:** Usa HttpClient con BaseAddress; añade header `X-Internal-Secret` desde `InternalSecret` en configuración.
- **Registro DI:** En entorno `Testing` → MockAdminApiClient; en resto → AdminApiClient con HttpClient (BaseUrl desde `AdminApi:BaseUrl`).

### Conclusión

La configuración está correcta: BaseUrl y autenticación (InternalSecret) en appsettings. AdminApiClient ya usa estos valores. **No requiere cambios** para el desacople de Companies; el cliente está listo para las llamadas a Admin.
