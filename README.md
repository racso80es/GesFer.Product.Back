# GesFer.Admin.Back

Backend (API) del módulo de administración GesFer. Este repositorio contiene **solo** la API y sus capas de aplicación, infraestructura y dominio; el proyecto se mantiene aislado respecto a otras piezas del ecosistema.

---

## Posición en el ecosistema GesFer

GesFer es un ecosistema modular que históricamente incluía varios componentes. Este repositorio corresponde a **una pieza concreta**:

| Componente | Descripción | Ubicación |
|------------|-------------|-----------|
| **Admin / Back** | API REST de administración | ✅ **GesFer.Admin.Back** (este repo) |
| **Admin / Front** | Interfaz web de administración | [GesFer.Admin.Front](https://github.com/racso80es/GesFer.Admin.Front) |
| **Product / Back** | API REST de producto | Repo separado |
| **Product / Front** | Interfaz web de producto | Repo separado |

**Resumen:** Este proyecto es el **backend (API)** del módulo **Admin**. El frontend de administración ([GesFer.Admin.Front](https://github.com/racso80es/GesFer.Admin.Front)) consume esta API; ambos repos comparten el protocolo SddIA y la estructura de documentación. La base de datos (`GesFer_Admin`) puede ser compartida con otros componentes del ecosistema.

---

## Contexto técnico

- **Stack:** .NET 8, ASP.NET Core Web API, JWT, Entity Framework Core, Serilog, Swagger.
- **Estructura:** Api → Application → Infrastructure → Domain; tests y scripts en `src/`.
- **Base de datos:** MySQL 8 (por defecto `GesFer_Admin` en `localhost:3306`).

---

## Requisitos

- .NET 8 SDK
- Windows 11 + PowerShell 7+ (según convención del proyecto; ver `AGENTS.md`)
- MySQL 8 (local o vía Docker)

---

## Ejecución

### Opción 1: Solo la API (con BD ya levantada)

Desde la raíz del repositorio, en PowerShell:

```powershell
dotnet run --project src/GesFer.Admin.Back.Api/GesFer.Admin.Back.Api.csproj
```

Por defecto la API escucha en **http://localhost:5010**. Swagger está disponible en `/swagger`.

### Opción 2: Entorno completo (Docker + API)

Para levantar MySQL, caché y opcionalmente la API en contenedores:

```powershell
docker-compose up -d gesfer-db cache adminer
```

Luego ejecutar la API localmente (o usar el servicio `gesfer-admin-api` del compose).

---

## Autenticación

La API admite **dos métodos** de autenticación:

| Método | Uso | Header |
|--------|-----|--------|
| **JWT Bearer** | Usuarios administrativos (login) | `Authorization: Bearer {token}` |
| **Shared Secret** | Llamadas sistema a sistema | `X-Internal-Secret: {valor}` |

### JWT (usuarios Admin)

1. **Obtener token:** `POST /api/admin/auth/login` con `{ "Usuario": "admin", "Contraseña": "admin123" }`.
2. La respuesta incluye `Token`. Copia solo el valor (sin `Bearer `).
3. **En Swagger UI:** Pulsa «Authorize», pega el token y confirma. Swagger enviará `Authorization: Bearer {token}` en las peticiones.
4. **En Postman/curl:** Añade el header `Authorization: Bearer {token}`.

> El token expira según `JwtSettings:ExpirationMinutes` (por defecto 60). Tras expirar, hay que hacer login de nuevo.

### Shared Secret (sistema)

Para integraciones entre servicios (p. ej. Product → Admin), envía el header:

```
X-Internal-Secret: {SharedSecret}
```

El valor se configura en `appsettings` (`SharedSecret`). En desarrollo: `dev-internal-secret-change-in-production`.

### Endpoints protegidos

- **`[AuthorizeSystemOrAdmin]`** — Acepta JWT con rol Admin **o** Shared Secret.
- **`[Authorize(Policy = "AdminOnly")]`** — Solo JWT con rol Admin (no Shared Secret).

El login (`POST /api/admin/auth/login`) es público.

### Configuración JWT (producción)

En `appsettings` o variables de entorno:

| Clave | Descripción |
|-------|-------------|
| `JwtSettings:SecretKey` | Clave secreta (mínimo 32 caracteres para HS256) |
| `JwtSettings:Issuer` | Emisor del token (p. ej. `GesFerApi`) |
| `JwtSettings:Audience` | Audiencia (p. ej. `GesFerClient`) |
| `JwtSettings:ExpirationMinutes` | Minutos hasta expiración (por defecto 60) |

---

## Credenciales por defecto (desarrollo)

### Usuario administrativo (login API)

Tras ejecutar migraciones y seeds, existe un usuario de prueba:

| Campo | Valor |
|-------|-------|
| **Usuario** | `admin` |
| **Contraseña** | `admin123` |

Endpoint de login: `POST /api/admin/auth/login` con `{ "Usuario": "admin", "Contraseña": "admin123" }`.

> ⚠️ **Seguridad:** Estas credenciales son solo para desarrollo. En producción usar variables de entorno y seeds seguros.

### Base de datos (MySQL)

| Campo | Valor por defecto |
|-------|-------------------|
| **Host** | `localhost` |
| **Puerto** | `3306` |
| **Base de datos** | `GesFer_Admin` |
| **Usuario** | `admin` |
| **Contraseña** | `GesFerAdmin@pthrjkl` |

Coinciden con la configuración de `docker-compose.yml` y `appsettings.json`.

---

## Herramientas de soporte

El proyecto incluye herramientas (Cúmulo: `paths.toolCapsules`) para automatizar tareas:

- **prepare-full-env** — Levanta servicios Docker (MySQL, cache, Adminer) y opcionalmente la API.
- **invoke-mysql-seeds** — Aplica migraciones EF y carga datos de seed.
- **start-api** — Levanta la API con verificación de health.
- **run-tests-local** — Ejecuta la suite de tests.
- **postman-mcp-validation** — Valida la API con Postman/Newman.

---

## Documentación de objetivos

Los objetivos y el alcance del proyecto están documentados en **[Objetivos.md](./Objetivos.md)**.

## Protocolo del proyecto

Para convenciones, leyes universales y sistema multi-agente, ver **[AGENTS.md](./AGENTS.md)**.
