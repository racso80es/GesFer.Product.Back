---
feature_name: kaizen-desacople-companies
created: '2026-03-19'
process: feature
branch: feat/kaizen-desacople-companies
contract_ref: SddIA/norms/features-documentation-pattern.md
---

# Objetivos

## Objetivo

Realizar un **Kaizen** (mejora continua) para **desacoplar Companies** del contexto Product. La responsabilidad de Companies recae en Admin; desde Product solo se accede a la compañía del usuario actual mediante llamada a API Admin.

## Alcance

- **Dominio Product:** Eliminar entidad Company y tabla Companies de la BD.
- **Acceso a compañía:** Únicamente vía `IAdminApiClient` (GetCompanyAsync, GetCompanyByNameAsync, UpdateCompanyAsync).
- **CompanyId:** Mantener como FK en entidades Product (User, Article, TaxType, etc.) sin navegación a Company; el valor proviene del claim JWT del usuario autenticado.
- **Autenticación:** JWT ya incluye CompanyId en claims; MyCompanyController ya usa Admin API.

## Ley aplicada

- **Ley SOBERANÍA:** docs/ y SddIA/ son SSOT.
- **Ley COMPILACIÓN:** Código roto inaceptable; verificar localmente tras cambios.
- **Ley GIT:** Rama feat/kaizen-desacople-companies; documentación en paths.featurePath.

## Referencias canónicas

- IAdminApiClient (Infrastructure/Services)
- MyCompanyController (Api/Controllers)
- JwtService (CompanyId en claims)
- JsonDataSeeder (Seed:CompanyId como lista blanca; no crea Companies)

---

# Análisis preliminar

## Situación actual

### 1. Entidad y tabla Company en Product

| Ubicación | Descripción |
|-----------|-------------|
| `src/domain/Entities/Company.cs` | Entidad Company con Name, TaxId, Address, navegación a PostalCode, City, State, Country, Language, y colecciones Users, Articles, Customers, Suppliers, Tariffs |
| `ApplicationDbContext` | `DbSet<Company> Companies` |
| `CompanyConfiguration` | Configuración EF para tabla Companies |
| Migraciones | Tabla Companies creada; FKs desde User, Article, TaxType, etc. hacia Companies |

### 2. Uso de Company/Companies en el código

| Componente | Uso |
|------------|-----|
| **IAdminApiClient** | GetCompanyAsync(Guid), GetCompanyByNameAsync(string), UpdateCompanyAsync — ya implementado |
| **MyCompanyController** | Obtiene CompanyId del claim; llama Admin API para Get/Update — **ya desacoplado** |
| **AuthService** | Login: GetCompanyByNameAsync para resolver empresa por nombre — **ya usa Admin** |
| **JwtService** | Incluye CompanyId en claims del token |
| **CreateUserCommandHandler** | Valida CompanyId con adminApiClient.GetCompanyAsync — **ya usa Admin** |
| **GetUserByIdCommandHandler** | Obtiene CompanyName vía adminApiClient.GetCompanyAsync — **ya usa Admin** |
| **UpdateUserCommandHandler** | Idem GetUserById |
| **ArticleFamiliesController** | GetCompanyId() desde claims (company_id o CompanyId) |
| **TelemetryController** | CompanyId del claim en propiedades |
| **JsonDataSeeder** | Seed:CompanyId como lista blanca; **no crea Companies**; valida CompanyId contra Admin implícitamente |
| **DbInitializer** | Smoke test: verifica admin.CompanyId y adminClient.GetCompanyAsync |

### 3. Entidades con CompanyId (FK)

| Entidad | CompanyId | Navegación a Company |
|---------|-----------|----------------------|
| User | Sí | No (comentario: "datos vía Admin API") |
| Article | Sí | No (config: "sin navegación") |
| ArticleFamily | Sí | No |
| TaxType | Sí | No |
| Tariff | Sí | No |
| TariffItem | Sí | No |
| Supplier | Sí | No |
| Customer | Sí | No |
| PurchaseDeliveryNote | Sí | No |
| PurchaseInvoice | Sí | No |
| SalesDeliveryNote | Sí | No |
| SalesInvoice | Sí | No |

**Nota:** Las configuraciones EF (TaxTypeConfiguration, TariffConfiguration, etc.) indican "CompanyId: FK a Companies (Admin); sin navegación". La entidad Company tiene colecciones inversas (Users, Articles, etc.) que generan relaciones en el modelo EF, pero las entidades hijas no tienen propiedad de navegación `Company` en dominio.

### 4. Migraciones y FKs

Las migraciones crean FKs explícitas:
- `FK_Families_Companies_CompanyId`
- `FK_PurchaseInvoices_Companies_CompanyId`
- `FK_SalesInvoices_Companies_CompanyId`
- `FK_Tariffs_Companies_CompanyId`
- `FK_TaxTypes_Companies_CompanyId`
- `FK_Users_Companies_CompanyId`
- `FK_Articles_Companies_CompanyId`
- `FK_Customers_Companies_CompanyId`
- `FK_Suppliers_Companies_CompanyId`
- etc.

Estas FKs apuntan a la tabla **Companies en Product**. Al eliminar la tabla, las FKs deben convertirse en **columnas sin FK** (CompanyId como dato referencial) o la tabla Companies debe existir solo en Admin y Product no tener FK real.

### 5. Comandos/handlers que reciben CompanyId

| Handler | Origen CompanyId |
|---------|------------------|
| CreateTaxTypeCommandHandler | command.CompanyId (obligatorio) |
| DeleteTaxTypeCommandHandler | command.CompanyId |
| GetAllTaxTypesCommand | CompanyId opcional (filtro) |
| CreateUserCommandHandler | command.CompanyId (validado vía Admin) |
| UpdateSupplierCommandHandler | supplier.CompanyId (desde entidad) |

---

## Objetivos extraídos (Kaizen)

1. **Eliminar entidad Company** del dominio Product y su tabla Companies de la BD.
2. **Eliminar DbSet Companies** y CompanyConfiguration del ApplicationDbContext.
3. **Convertir CompanyId en columna sin FK** en las entidades Product (o mantener FK si Admin y Product comparten BD; aclarar en clarify).
4. **Eliminar relaciones HasOne(Company)** de las configuraciones EF que las tengan implícitas (vía Company.Users, etc.).
5. **Generar migración** que elimine tabla Companies y FKs; CompanyId permanece como columna en cada entidad.
6. **Validar** que IAdminApiClient sigue siendo la única fuente de datos de empresa (GetCompanyAsync para CompanyName, etc.).
7. **Validar** que el claim CompanyId en JWT se obtiene correctamente en todos los controllers que lo necesitan.
8. **Actualizar seeds** si referencian Companies (JsonDataSeeder ya no crea Companies; Seed:CompanyId es lista blanca).
9. **Actualizar tests** (CreateUserCommandHandlerTests, UserControllerTests, SupplierControllerTests, etc.) que mockean GetCompanyAsync o usan CompanyId.
10. **Documentar** el contrato: Product no gestiona Companies; Admin es SSOT; acceso solo vía API con autenticación.
