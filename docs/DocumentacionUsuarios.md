# Especificación Técnica de Entidad: User

## 1. Estructura de Datos (Tablas y Modelos)

### Modelo de Base de Datos: `Users` (Hereda de `BaseEntity`)

| Campo | Tipo en DB | Nullable | Restricciones / Constraints | Descripción |
| :--- | :--- | :---: | :--- | :--- |
| `Id` | `uniqueidentifier` | No | Primary Key | GUID secuencial (COMB GUID) generado por EF Core para optimizar índices agrupados. |
| `CompanyId` | `uniqueidentifier` | No | | Foreign Key conceptual a `Companies` en microservicio Admin (sin navegación directa en Product). |
| `Username` | `nvarchar(100)` | No | `MaxLength(100)` | Nombre de usuario para login. |
| `PasswordHash` | `nvarchar(500)` | No | `MaxLength(500)` | Hash de la contraseña (BCrypt). |
| `FirstName` | `nvarchar(100)` | No | `MaxLength(100)` | Nombre(s) del usuario. |
| `LastName` | `nvarchar(100)` | No | `MaxLength(100)` | Apellido(s) del usuario. |
| `Email` | `nvarchar(200)` | Sí | `MaxLength(200)` | Value Object `Email` mapeado como string. Conversión automática aplicada. |
| `Phone` | `nvarchar(50)` | Sí | `MaxLength(50)` | Número de teléfono de contacto. |
| `Address` | `nvarchar(500)` | Sí | `MaxLength(500)` | Dirección postal (calle, número, etc.). |
| `PostalCodeId` | `uniqueidentifier` | Sí | FK a `PostalCodes`, `OnDelete: Restrict` | Relación opcional con tabla `PostalCodes`. |
| `CityId` | `uniqueidentifier` | Sí | FK a `Cities`, `OnDelete: Restrict` | Relación opcional con tabla `Cities`. |
| `StateId` | `uniqueidentifier` | Sí | FK a `States`, `OnDelete: Restrict` | Relación opcional con tabla `States`. |
| `CountryId` | `uniqueidentifier` | Sí | FK a `Countries`, `OnDelete: Restrict` | Relación opcional con tabla `Countries`. |
| `LanguageId` | `uniqueidentifier` | Sí | FK a `Languages`, `OnDelete: Restrict` | Relación opcional con tabla `Languages`. |
| `CreatedAt` | `datetime2` | No | Default: `DateTime.UtcNow` | Fecha de creación del registro. |
| `UpdatedAt` | `datetime2` | Sí | | Fecha de última modificación. |
| `DeletedAt` | `datetime2` | Sí | | Implementación de Soft Delete global. |
| `IsActive` | `bit` | No | Default: `true` | Bandera de estado activo/inactivo. |

### Relaciones Many-to-Many

- **`UserGroups`**: Vincula `UserId` con `GroupId`. (`OnDelete: Cascade` en ambas FK). Índice único en `{ UserId, GroupId }`.
- **`UserPermissions`**: Vincula `UserId` con `PermissionId`. (`OnDelete: Cascade` en ambas FK). Índice único en `{ UserId, PermissionId }`.

### Índices

- Índice único compuesto: `[CompanyId, Username]` para evitar usernames duplicados por compañía.
- Índices de Foreign Keys (no únicos): `PostalCodeId`, `CityId`, `StateId`, `CountryId`, `LanguageId`.

---

## 2. Semillas (Seeds)

El seeding de usuarios se realiza mediante el `JsonDataSeeder` procesando estructuras JSON de test-data. El seeder valida la existencia del `CompanyId` e incluye lógica de soft delete (re-activación).

```json
{
  "Users": [
    {
      "Id": "00000000-0000-0000-0000-000000000000",
      "CompanyId": "00000000-0000-0000-0000-000000000000",
      "Username": "string",
      "Password": "admin123", // Hasheado con BCrypt
      "FirstName": "string",
      "LastName": "string",
      "Email": "string@domain.com",
      "Phone": "string",
      "LanguageId": "00000000-0000-0000-0000-000000000000"
    }
  ]
}
```

```csharp
// Seed User C# Class
private class UserSeed
{
    public string Id { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string LanguageId { get; set; } = string.Empty;
}
```

Nota en el seeding: La contraseña `admin123` utiliza un hash BCrypt determinista fijo: `$2a$11$IRkoFxAcLpHUIwLTqkJaHu6KYx.dgfGY.sFUIsCTY9xHPhL3jcpgW` para mantener compatibilidad en test-data.

---

## 3. Lógica de Negocio y Casos de Uso (CRUD + Extras)

- **Creación (`CreateUserCommandHandler`)**:
  - Verifica que el `CompanyId` exista mediante una llamada síncrona a `IAdminApiClient.GetCompanyAsync`.
  - Regla de unicidad: No puede existir otro usuario con el mismo `Username` dentro de la misma `CompanyId`.
  - Verifica la integridad referencial de todos los campos de dirección opcionales proporcionados (`PostalCodeId`, `CityId`, `StateId`, `CountryId`, `LanguageId`).
  - Hashea la contraseña usando `BCrypt.Net.BCrypt.HashPassword` (WorkFactor: 11).
  - Validaciones de dominio sobre `Email` (Value Object).

- **Actualización (`UpdateUserCommandHandler`)**:
  - Verifica la existencia del usuario a actualizar.
  - Regla de unicidad de `Username` dentro de la misma `CompanyId` (excluyendo el usuario actual).
  - Validaciones de integridad de dependencias foráneas (como en la creación).
  - Hashea la nueva contraseña solo si es proveída.
  - Actualiza el campo `UpdatedAt` a `DateTime.UtcNow`.
  - Actualiza bandera `IsActive`.

- **Eliminación (`DeleteUserCommandHandler`)**:
  - Implementa **Soft Delete**.
  - Setea `DeletedAt = DateTime.UtcNow` y `IsActive = false`.
  - No borra registros físicos.

- **Consulta (`GetAllUsersCommandHandler` / `GetUserByIdCommandHandler`)**:
  - Obtiene el listado de usuarios de acuerdo al `CompanyId` extraído del token del usuario autenticado en los controladores, garantizando asilamiento Multi-Tenant.
  - Responde con representaciones `UserDto`.

---

## 4. Interfaces y DTOs

```csharp
/// <summary>
/// DTO para respuesta de usuario
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid? PostalCodeId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? StateId { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? LanguageId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO para crear usuario
/// </summary>
public class CreateUserDto
{
    public Guid CompanyId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid? PostalCodeId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? StateId { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? LanguageId { get; set; }
}

/// <summary>
/// DTO para actualizar usuario
/// </summary>
public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid? PostalCodeId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? StateId { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? LanguageId { get; set; }
    public bool IsActive { get; set; }
}
```

---

## 5. Endpoints (Contratos de API)

Ruta Base: `/api/User`
Requisitos: Requiere Autorización (`[Authorize]`). Extrae `CompanyId` del contexto/token del invocador para limitar el scope de lectura/escritura (Multi-Tenant).

| Método HTTP | Ruta | Parámetros | DTO Entrada | DTO Respuesta | Códigos de Estado Esperados |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/User` | N/A | N/A | `List<UserDto>` | `200 OK`, `500 Internal Server Error` |
| `GET` | `/api/User/{id}` | `Guid id` (Path) | N/A | `UserDto` | `200 OK`, `404 Not Found`, `500 Internal Server Error` |
| `POST` | `/api/User` | N/A | `CreateUserDto` (Body) | `UserDto` | `201 Created`, `400 Bad Request`, `500 Internal Server Error` |
| `PUT` | `/api/User/{id}` | `Guid id` (Path) | `UpdateUserDto` (Body)| `UserDto` | `200 OK`, `400 Bad Request`, `404 Not Found`, `500 Internal Server Error` |
| `DELETE` | `/api/User/{id}`| `Guid id` (Path) | N/A | N/A (No Content) | `204 No Content`, `404 Not Found`, `500 Internal Server Error` |

---

## 6. Código Legacy / Acoplamiento Oculto

- **Acoplamiento con Microservicio Admin**: Existe una dependencia síncrona fuerte al momento de crear o actualizar un usuario, debido a la llamada requerida a `IAdminApiClient.GetCompanyAsync(command.Dto.CompanyId)` para confirmar si la empresa existe remotamente.
- **Acoplamiento con Autenticación / Setup**: `AuthService.cs` y componentes de login (tokens) dependen íntimamente de la existencia de `Users`, además del `JsonDataSeeder` usando strings hardcodeadas y dependencias indirectas sobre `CompanyId` en el contexto.
- **Relaciones Geográficas Locales**: La entidad y sus Commands se encuentran acoplados a entidades geográficas de ubicación (PostalCode, City, State, Country, Language) locales en el microservicio `Product`. Durante la extracción de la responsabilidad a un nuevo servicio "Admin", se deberá resolver qué base de datos poseerá la fuente de la verdad para estas entidades catalogadas o cómo referenciarlas de forma distribuida.
