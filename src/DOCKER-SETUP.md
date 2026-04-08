# Configuración de Docker para GesFer Product Back

## ✅ Configuración Verificada

El archivo `docker-compose.yml` está correctamente configurado con:

**Proyecto Docker:** `GesFer_Product_Back` (name explícito para evitar conflictos con otros proyectos GesFer)

### Servicios Configurados

1. **MySQL 8.0** (`GesFer_product_db`)
   - Puerto en el host: `3307` (mapeado al `3306` interno del contenedor)
   - Base de datos: `GesFer_Product`
   - Usuario: `product`
   - Contraseña: `GesFerProduct@pthrjkl`
   - Root password: `rootpassword`
   - Charset: `utf8mb4_unicode_ci`
   - Healthcheck configurado

2. **Memcached** (`gesfer_product_cache`)
   - Puerto en el host: `11212` (mapeado al `11211` interno del contenedor)
   - Memoria: 128MB
   - Healthcheck configurado

3. **Adminer** (`gesfer_product_adminer`)
   - Puerto en el host: `8081` (mapeado al `8080` interno del contenedor)
   - Interfaz web para gestión de MySQL
   - Espera a que MySQL esté saludable antes de iniciar

### Red Docker

Todos los servicios están en la red `gesfer_network` (bridge).

## 🚀 Iniciar Servicios

### Opción 1: Script PowerShell (Recomendado)

```powershell
.\docker-start.ps1
```

### Opción 2: Docker Compose Manual

```bash
docker-compose up -d
```

### Verificar Estado

```bash
docker-compose ps
```

### Ver Logs

```bash
# Todos los servicios
docker-compose logs -f

# Solo MySQL
docker-compose logs -f gesfer-db

# Solo Memcached
docker-compose logs -f cache
```

## 🛑 Detener Servicios

### Opción 1: Script PowerShell

```powershell
.\docker-stop.ps1
```

### Opción 2: Docker Compose Manual

```bash
docker-compose down
```

### Detener y Eliminar Volúmenes (⚠️ Elimina datos)

```bash
docker-compose down -v
```

## 🔍 Verificar Conexión a MySQL

### Desde PowerShell

```powershell
docker exec -it GesFer_product_db mysql -u product -pGesFerProduct@pthrjkl GesFer_Product -e "SELECT 1;"
```

### Desde Adminer

1. Abrir navegador en: `http://localhost:8081`
2. Sistema: `MySQL`
3. Servidor: `gesfer-db`
4. Usuario: `product`
5. Contraseña: `GesFerProduct@pthrjkl`
6. Base de datos: `GesFer_Product`

## 📝 Cadena de Conexión

La cadena de conexión configurada en `appsettings.Development.json` es:

```
Server=localhost;Port=3307;Database=GesFer_Product;User=product;Password=GesFerProduct@pthrjkl;CharSet=utf8mb4;AllowUserVariables=True;AllowLoadLocalInfile=True;
```

## ⚙️ Configuración de EF Core

El `ApplicationDbContext` está configurado con:
- ✅ Reintentos automáticos en caso de fallo (5 intentos, 30s de delay)
- ✅ Logging detallado en desarrollo
- ✅ String comparison habilitado
- ✅ Versión MySQL 8.0

## 🔧 Solución de Problemas

### MySQL no inicia

1. Verificar logs: `docker-compose logs gesfer-db`
2. Verificar que el puerto 3307 del host no esté en uso
3. Eliminar volúmenes y reiniciar: `docker-compose down -v && docker-compose up -d`

### Error de conexión desde la API

1. Verificar que MySQL esté corriendo: `docker-compose ps`
2. Verificar que el healthcheck esté OK: `docker inspect GesFer_product_db | grep -A 10 Health`
3. Probar conexión manual con Adminer
4. Verificar que la cadena de conexión en `appsettings.json` sea correcta

### Puerto ya en uso

Si el puerto **3307** del host está ocupado, cambiar en `docker-compose.yml` (y en `docker-compose.override.yml` si aplica):

```yaml
ports:
  - "3308:3306"  # u otro puerto libre en el host
```

Y actualizar la cadena de conexión local (`appsettings.Development.json` / `appsettings.json`) con el mismo puerto en `Server=localhost;Port=...`.

## 📊 Comandos Útiles

```bash
# Ver uso de recursos
docker stats

# Entrar al contenedor MySQL
docker exec -it GesFer_product_db bash

# Backup de la base de datos
docker exec GesFer_product_db mysqldump -u product -pGesFerProduct@pthrjkl GesFer_Product > backup.sql

# Restaurar base de datos
docker exec -i GesFer_product_db mysql -u product -pGesFerProduct@pthrjkl GesFer_Product < backup.sql

# Ver variables de entorno
docker exec GesFer_product_db env
```

## ✅ Checklist de Verificación

- [ ] Docker Desktop está corriendo
- [ ] `docker-compose up -d` ejecutado sin errores
- [ ] MySQL healthcheck está OK: `docker inspect GesFer_product_db | grep -A 5 Health`
- [ ] Puedo conectar desde Adminer: `http://localhost:8081`
- [ ] La API puede conectarse (verificar logs de la aplicación)
- [ ] Las migraciones se aplican correctamente
