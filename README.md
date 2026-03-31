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

O utilizando el script proporcionado (recomendado en Windows):
```powershell
.\src\docker-start.ps1
```

Esto iniciará:
- MySQL en el puerto 3307 en el host (BD `GesFer_Product` u otra definida en compose)
- Memcached en el puerto 11212
- Adminer (opcional) en el puerto 8081 para gestión visual de BD

*Para detener los servicios, usa `docker-compose down` o `.\src\docker-stop.ps1`.*

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

---

## Configuración de Docker Detallada

El archivo `docker-compose.yml` está configurado con el proyecto **GesFer_Product_Back** para evitar conflictos.

### Servicios Configurados

1. **MySQL 8.0** (`GesFer_product_db`)
   - Puerto en host: `3307` (mapeado a `3306`)
   - BD: `GesFer_Product`, Usuario: `product`, Pass: `GesFerProduct@pthrjkl`, Root: `rootpassword`
   - Charset: `utf8mb4_unicode_ci` con healthcheck.

2. **Memcached** (`gesfer_product_cache`)
   - Puerto en host: `11212` (mapeado a `11211`), 128MB.

3. **Adminer** (`gesfer_product_adminer`)
   - Puerto en host: `8081` (mapeado a `8080`) para gestión visual de MySQL.

### Scripts Útiles (PowerShell / Bash)

- **Iniciar:** `.\src\docker-start.ps1` o `docker-compose up -d`
- **Detener:** `.\src\docker-stop.ps1` o `docker-compose down`
- **Verificar:** `docker-compose ps` y `docker-compose logs -f`
- **Cadena de conexión por defecto:** `Server=localhost;Port=3307;Database=GesFer_Product;User=product;Password=GesFerProduct@pthrjkl;CharSet=utf8mb4;AllowUserVariables=True;AllowLoadLocalInfile=True;`
## Configuración y Diagnóstico de Docker

El archivo `docker-compose.yml` está configurado con:
- **Proyecto Docker:** `GesFer_Product_Back` (name explícito para evitar conflictos)
- **Red:** `gesfer_network` (bridge)

### Servicios y Puertos
| Servicio | Nombre Contenedor | Puerto Host | Puerto Interno | Usuario / BD | Password |
|----------|-------------------|-------------|----------------|--------------|----------|
| **MySQL 8.0** | `GesFer_product_db` | 3307 | 3306 | `product` / `GesFer_Product` | `GesFerProduct@pthrjkl` |
| **Memcached** | `gesfer_product_cache`| 11212 | 11211 | N/A | N/A |
| **Adminer** | `gesfer_product_adminer`| 8081 | 8080 | N/A | N/A |

### Cadena de Conexión y EF Core
Configurada en `appsettings.Development.json`:
`Server=localhost;Port=3307;Database=GesFer_Product;User=product;Password=GesFerProduct@pthrjkl;CharSet=utf8mb4;AllowUserVariables=True;AllowLoadLocalInfile=True;`

El `ApplicationDbContext` cuenta con reintentos automáticos (5 intentos, 30s de delay), logging detallado en desarrollo y *string comparison* habilitado.

---

## Solución de Problemas (Troubleshooting)

### La API no inicia o se cuelga
1. **Verificar procesos:** Ejecuta `Get-Process | Where-Object {$_.ProcessName -like "*GesFer*" -or $_.ProcessName -like "*dotnet*"}` (PowerShell) para comprobar si hay instancias colgadas y destrúyelas (`taskkill /PID <PID> /F`).
2. **Puertos en uso:** `netstat -ano | findstr :5000` (o 5001, o 5020).
3. **Migraciones:** Las migraciones ya no bloquean el inicio; revisa los logs en consola o "Output" para ver el error de base de datos.

### MySQL no está disponible
1. Asegúrate de que Docker esté corriendo: `docker-compose ps`.
2. Si no arranca, reinicia los contenedores o ejecuta el healthcheck: `docker inspect GesFer_product_db | grep -A 5 Health`.
3. Para borrar y reiniciar los volúmenes (⚠️ pierde datos): `docker-compose down -v && docker-compose up -d`.

### Swagger no carga o error HTTPS
1. Accede por HTTP (`http://localhost:5000/swagger` o `5020`).
2. Confía en el certificado local: `dotnet dev-certs https --trust`.
3. Endpoint de salud: `http://localhost:5020/api/health` para comprobar que la API responde.
### 1. MySQL no inicia o Error de conexión
- Verifica logs: `docker-compose logs gesfer-db` o `docker-compose logs -f`
- Verifica si el puerto 3307 está en uso:
  ```powershell
  netstat -ano | findstr :3307
  ```
- Si el puerto está ocupado, cámbialo en `docker-compose.yml` (ej. `3308:3306`) y actualiza la cadena de conexión en el `appsettings.json`.
- Para reiniciar la BD eliminando volúmenes (⚠️ elimina datos): `docker-compose down -v && docker-compose up -d`

### 2. La página (Swagger/API) no carga
- **¿Está corriendo la API?** Revisa si hay procesos `dotnet` activos:
  ```powershell
  Get-Process | Where-Object {$_.ProcessName -like "*GesFer*" -or $_.ProcessName -like "*dotnet*"}
  ```
- **Certificado HTTPS no confiable:** Si falla `https://localhost:5001`, ejecuta: `dotnet dev-certs https --trust`
- **Puerto ya en uso:** Revisa los puertos 5000/5001 o 5020/5021 y mata el proceso con `taskkill /PID <PID> /F`.
- Prueba el **Health Check** en: `http://localhost:5020/api/health` (ajustar puerto según corresponda).

### 3. Las migraciones bloquean o fallan al iniciar
Actualmente, las migraciones no bloquean el inicio de la aplicación. Si hay un error, la aplicación continuará sin aplicarlas. Revisa los logs ("Verificando conexión a la base de datos...", "Aplicando migraciones...") para encontrar el error específico (e.g. "Database does not exist", "Access denied").

### Comandos y Verificaciones Útiles
```powershell
# Verificar estado de Docker y healthcheck
docker-compose ps
docker inspect GesFer_product_db | Select-String "Health"

# Conexión manual a MySQL (desde contenedor)
docker exec -it GesFer_product_db mysql -u product -pGesFerProduct@pthrjkl GesFer_Product -e "SELECT 1;"

# Backup de la BD
docker exec GesFer_product_db mysqldump -u product -pGesFerProduct@pthrjkl GesFer_Product > backup.sql
```
