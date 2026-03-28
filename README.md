# GesFer.Product.Back

Backend (API) del módulo de Producto GesFer (Sistema de Gestión de Compra/Venta de Chatarra). Este repositorio contiene **solo** la API y sus capas de aplicación, infraestructura y dominio; el proyecto se mantiene aislado respecto a otras piezas del ecosistema.

---

## Posición en el ecosistema GesFer

GesFer es un ecosistema modular que históricamente incluía varios componentes. Este repositorio corresponde a **una pieza concreta**:

| Componente | Descripción | Ubicación |
|------------|-------------|-----------|
| **Product / Back** | API REST de producto (Gestión, inventario, compra/venta) | ✅ **GesFer.Product.Back** (este repo) |
| **Product / Front** | Interfaz web de producto | Repo separado |
| **Admin / Back** | API REST de administración | Repo separado |
| **Admin / Front** | Interfaz web de administración | Repo separado |

**Resumen:** Este proyecto es el **backend (API)** del módulo **Product**. El frontend de producto consume esta API; ambos repos comparten el protocolo SddIA y la estructura de documentación. La base de datos puede ser compartida con otros componentes del ecosistema.

---

## Contexto técnico y Arquitectura

El proyecto sigue los principios de **Clean Architecture** y **SOLID**, organizado en capas:

- **Stack:** .NET 8, ASP.NET Core Web API, JWT, Entity Framework Core (Code First), Serilog, Swagger/OpenAPI.
- **Estructura:** Api → Application → Infrastructure → Domain; tests y scripts en `src/`.
- **Base de datos:** MySQL 8 (con UTF8MB4).
- **Caché:** Memcached.

---

## Requisitos Previos

- .NET 8 SDK
- Windows 11 + PowerShell 7+ (según convención del proyecto; ver `AGENTS.md`)
- Docker Desktop (para MySQL y Memcached)

---

## Ejecución y Configuración Inicial

### 1. Iniciar servicios con Docker

Para levantar MySQL, caché y Adminer en contenedores, desde la raíz del repositorio, ejecuta:

```bash
docker-compose up -d
```

Esto iniciará:
- MySQL en el puerto 3307 en el host (BD `GesFer_Product` u otra definida en compose)
- Memcached en el puerto 11212
- Adminer (opcional) en el puerto 8081 para gestión visual de BD

### 2. Crear la base de datos y migraciones

El proyecto unifica las migraciones en una sola (**InitialCreate**) que refleja el modelo actual del `ApplicationDbContext`.

Para aplicar la migración o actualizar la base de datos, desde el directorio de la API:

```bash
cd src/Api
dotnet ef database update --project ../Infrastructure/GesFer.Infrastructure.csproj --context ApplicationDbContext
```

Si necesitas añadir una nueva migración (tras modificar el modelo):
```bash
dotnet ef migrations add NombreMigracion --project ../Infrastructure/GesFer.Infrastructure.csproj --context ApplicationDbContext
```

> **Nota para bases de datos existentes:**
> - En desarrollo, lo ideal es eliminar la BD y recrearla.
> - Para conservar datos, asegúrate de que el esquema coincide e inserta manualmente la fila de la migración (`20260213141112_InitialCreate` o la que corresponda) en la tabla `__EFMigrationsHistory`.

### 2.1 Datos Iniciales (Seeds)

Los datos iniciales se cargan automáticamente al iniciar la aplicación (en entorno **Development**) a partir de archivos JSON en `src/Infrastructure/Data/Seeds/`. El proceso es **idempotente** (no duplica ni pisa datos ya existentes).

- `master-data.json`: Datos maestros compartidos por todas las empresas (Idiomas, Permisos del sistema, Grupos de usuarios).
- `demo-data.json`: Datos de prueba para desarrollo (Empresas, Usuarios, Familias, Artículos, Proveedores, Clientes).
- `test-data.json`: Datos específicos para tests de integración.

Para añadir datos de prueba, edita directamente estos JSON (usando GUIDs únicos y hashes bcrypt en el campo `password`) y reinicia la aplicación. No se deben añadir datos en el código C#.

**Credenciales de prueba por defecto (según demo-data):**
| Empresa         | Usuario   | Contraseña |
|-----------------|-----------|------------|
| Empresa Admin   | admin     | admin123   |
| Empresa Cliente | user_test | admin123   |

### 3. Ejecutar la API localmente

Desde el directorio del proyecto de la API:

```bash
cd src/Api
dotnet run
```

O puedes compilar/ejecutar la solución completa `src/GesFer.Product.sln`:
```bash
dotnet build src/GesFer.Product.sln
dotnet run --project src/Api/GesFer.Api.csproj
```

Por defecto la API escucha en los puertos definidos (ej. HTTP: `http://localhost:5020` o HTTPS: `https://localhost:5021`).
Swagger estará disponible en `/swagger`.

---

## Autenticación y Seguridad

El sistema implementa autenticación multi-tenant con RBAC (Role-Based Access Control) y soporte para JWT.

### Endpoint de Login (Usuarios del Producto)

```http
POST /api/auth/login
Content-Type: application/json

{
  "empresa": "NombreEmpresa",
  "usuario": "usuario",
  "contraseña": "password"
}
```

La respuesta incluirá un token JWT y los permisos del usuario para usarse en las cabeceras `Authorization: Bearer {token}`.

### Shared Secret (sistema a sistema)

Para integraciones entre servicios (p. ej. Admin → Product), puede existir una cabecera `X-Internal-Secret: {SharedSecret}` configurada en los endpoints del sistema.

---

## Módulos Principales

### 1. Autenticación y Seguridad
- Login multi-tenant (Empresa + Usuario + Contraseña)
- Sistema RBAC con permisos directos y por grupos
- Cálculo automático de permisos combinados

### 2. Inventario y Catálogo
- **Family**: Familias de artículos con % IVA
- **Article**: Artículos con código único, precios y stock

### 3. Tarifas
- **Tariff**: Tarifas de compra/venta
- **TariffItem**: Precios específicos por artículo en cada tarifa

### 4. Terceros
- **Supplier**: Proveedores con tarifa de compra opcional
- **Customer**: Clientes con tarifa de venta opcional

### 5. Operaciones de Compra
- **PurchaseDeliveryNote**: Albaranes de compra (aumentan stock)
- **PurchaseInvoice**: Facturas de compra

### 6. Operaciones de Venta
- **SalesDeliveryNote**: Albaranes de venta (disminuyen stock)
- **SalesInvoice**: Facturas de venta

---

## Características Implementadas

✅ **Soft Delete** global en todas las entidades
✅ **Multi-tenant** con CompanyId en todas las entidades de negocio
✅ **Precisión decimal** decimal(18,4) para todos los importes
✅ **UTF8** configurado para soportar caracteres especiales
✅ **Gestión automática de stock** en albaranes
✅ **Cálculo automático de precios** desde tarifas
✅ **Cálculo automático de IVA** según familia del artículo
✅ **RBAC** con permisos directos y por grupos

---

## Herramientas de soporte

El proyecto incluye herramientas (Cúmulo: `paths.toolCapsules`) para automatizar tareas:

- **prepare-full-env** — Levanta servicios Docker (MySQL, cache, Adminer) y opcionalmente la API.
- **invoke-mysql-seeds** — Aplica migraciones EF y carga datos de seed.
- **start-api** — Levanta la API con verificación de health.
- **run-tests-local** — Ejecuta la suite de tests (`dotnet test src/GesFer.Product.sln`).
- **postman-mcp-validation** — Valida la API con Postman/Newman.

---

## Documentación de objetivos

Los objetivos y el alcance del proyecto están documentados en **[Objetivos.md](./Objetivos.md)**.

## Protocolo del proyecto

Para convenciones, leyes universales y sistema multi-agente, ver **[AGENTS.md](./AGENTS.md)**.
