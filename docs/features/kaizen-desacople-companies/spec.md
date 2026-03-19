---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
base: origin/main
scope: src/domain, src/Infrastructure, src/Api, src/application, src/tests
contract_ref: SddIA/norms/features-documentation-pattern.md
---

# SPEC: Kaizen desacople de Companies

## Contexto

La responsabilidad de **Companies** recae en el contexto **Admin**. Product no debe gestionar ni almacenar la entidad Company. Desde Product solo se accede a la compañía del usuario actual mediante llamada a API Admin (IAdminApiClient), con la autenticación adecuada (JWT con CompanyId en claims).

**Estado actual:** Product tiene tabla Companies, entidad Company, DbSet y configuraciones EF. Las entidades (User, Article, TaxType, etc.) tienen CompanyId como FK a Companies. IAdminApiClient ya existe y se usa en MyCompanyController, AuthService, CreateUserCommandHandler, etc. JsonDataSeeder no crea Companies (usa Seed:CompanyId como lista blanca).

---

## Especificación técnica

### Punto 1: Eliminar entidad Company del dominio Product

**Descripción:** Eliminar la entidad `Company` del dominio Product.

**Archivos a modificar/eliminar:**
- `src/domain/Entities/Company.cs` — **eliminar**
- Referencias a `Company` en otros archivos del dominio (si las hay)

**Criterio de aceptación:** No existe entidad Company en src/domain.

---

### Punto 2: Eliminar tabla Companies y DbSet del ApplicationDbContext

**Descripción:** Eliminar DbSet Companies y la configuración de la tabla.

**Archivos a modificar:**
- `src/Infrastructure/Data/ApplicationDbContext.cs` — eliminar `DbSet<Company> Companies`
- `src/Infrastructure/Data/Configurations/CompanyConfiguration.cs` — **eliminar**
- `modelBuilder.ApplyConfigurationsFromAssembly` aplicará configs; al eliminar CompanyConfiguration, Company deja de configurarse

**Criterio de aceptación:** ApplicationDbContext no referencia Company ni Companies.

---

### Punto 3: Convertir CompanyId en columna sin FK

**Descripción:** Las entidades mantienen `CompanyId` como columna (Guid), pero **sin FK** a tabla Companies. La integridad referencial se valida en capa de aplicación vía Admin API.

**Estrategia:** Generar migración EF que:
1. Elimine las FKs existentes (FK_Users_Companies_CompanyId, etc.)
2. Elimine la tabla Companies
3. Mantenga la columna CompanyId en cada entidad (sin FK)

**Configuraciones EF:** Las configuraciones actuales (TaxTypeConfiguration, UserConfiguration, etc.) ya indican "sin navegación". Debe eliminarse cualquier `HasOne(Company)` o `HasForeignKey` que apunte a Company. Si EF infiere relaciones desde la entidad Company (que se elimina), al eliminar Company esas relaciones desaparecen.

**Criterio de aceptación:** Migración aplica correctamente; CompanyId existe como columna en cada entidad; no hay tabla Companies ni FKs a Companies.

---

### Punto 4: Eliminar relaciones implícitas desde Company

**Descripción:** La entidad Company tiene `ICollection<User> Users`, `ICollection<Article> Articles`, etc. Al eliminar Company, esas relaciones desaparecen. Verificar que ninguna configuración EF en otras entidades tenga `HasOne(x => x.Company)` o similar.

**Archivos a revisar:** Todas las configuraciones en `src/Infrastructure/Data/Configurations/` y `src/Infrastructure/Persistence/Configurations/`.

**Criterio de aceptación:** No hay referencias a Company en configuraciones EF.

---

### Punto 5: Validar flujo de CompanyId (sin CompanyId desde frontend)

**Descripción:** CompanyId **no ha de ser dado por el frontend**. Siempre se obtiene del claim JWT del usuario autenticado. Si la tabla tiene CompanyId, se filtra por ella.

| Componente | Acción |
|------------|--------|
| JwtService | Ya incluye CompanyId en claims — verificar |
| MyCompanyController | GetCompanyId() desde claims — OK |
| ArticleFamiliesController | GetCompanyId() desde claims — OK |
| Handlers que reciben CompanyId en comando | Controller inyecta CompanyId desde User antes de crear comando |
| CreateTaxTypeCommandHandler | Controller pasa CompanyId del usuario; no aceptar del body |
| GetAllTaxTypesCommand | CompanyId del claim; no aceptar de query |
| Todos los endpoints con entidades CompanyId | Revisar y rechazar/ignorar CompanyId del cliente |

**Criterio de aceptación:** Ningún endpoint acepta CompanyId del frontend; siempre se usa el del usuario autenticado (salvo operaciones de admin/seeds).

---

### Punto 6: Seeds y DbInitializer

**Descripción:** JsonDataSeeder ya no crea Companies. Seed:CompanyId es lista blanca. Admin es responsable de que existan las empresas. Si no existen, Product ha de contemplar el error (log, descarte, fallo claro).

**Verificar:**
- JsonDataSeeder: eliminar referencias a `CompanySeed` o `data.Companies` si se usan para algo distinto a validación
- DbInitializer: no depende de tabla Companies local
- Manejo de error: cuando CompanyId no existe en Admin, Product debe manejar el caso (p. ej. GetCompanyAsync devuelve null → error o descarte)

**Criterio de aceptación:** Seeds y DbInitializer funcionan sin tabla Companies; errores por empresa inexistente en Admin manejados correctamente.

---

### Punto 7: Tests

**Descripción:** Actualizar tests que dependan de Company o Companies.

| Test | Acción |
|------|--------|
| CreateUserCommandHandlerTests | Ya mockea GetCompanyAsync — verificar que no use DbSet Companies |
| UserControllerTests | Usa _testCompanyId — verificar |
| SupplierControllerTests | Idem |
| JsonDataSeederTests | Seed:CompanyId config — verificar |
| DbInitializer / smoke tests | Verificar |

**Criterio de aceptación:** Todos los tests pasan tras los cambios.

---

### Punto 8: Documentación del contrato

**Descripción:** Documentar en clarify.md o en docs/architecture que:
- Admin es SSOT para Companies
- Product accede solo vía IAdminApiClient
- CompanyId en entidades Product es dato referencial (sin FK)
- Autenticación: JWT con CompanyId en claims

**Criterio de aceptación:** Contrato documentado en paths.featurePath o paths.architecturePath.

---

## Dependencias

- **Admin API:** Debe exponer GetCompanyAsync, GetCompanyByNameAsync, UpdateCompanyAsync.
- **JWT:** Debe incluir CompanyId en claims (ya implementado).
- **AdminApiClient:** Configurado en appsettings (AdminApi:BaseUrl, InternalSecret). Revisado: OK.
- **Migraciones:** EF Core; MySQL.

---

## Riesgos y mitigación

| Riesgo | Mitigación |
|--------|------------|
| Datos huérfanos (CompanyId sin empresa en Admin) | Validación en handlers vía Admin API antes de persistir |
| Migración en BD con datos existentes | Backup; migración debe eliminar FKs antes de tabla Companies |
| Tests de integración dependen de Companies | Ajustar fixtures; Admin API mockeado |
