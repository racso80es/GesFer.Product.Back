# Auditoría de Salud del Código - 2026-04-24

## 1. Métricas de Salud (0-100%)
Arquitectura: 90% | Nomenclatura: 100% | Estabilidad Async: 100%

## 2. Pain Points (🔴 Críticos / 🟡 Medios)

🟡 **Hallazgo:** Ineficiencia en la agregación de permisos. Uso de bucle `foreach` manual para agregar elementos a un `HashSet<T>` en lugar de inicializar el conjunto con la colección de permisos directos y usar `UnionWith` para los permisos de grupo. Esto afecta el rendimiento en la recuperación de permisos, una ruta crítica de seguridad y validación de autorización.

**Ubicación:** `src/Infrastructure/Services/AuthService.cs` líneas 76-105 aprox. (Método `GetUserPermissionsAsync`)

## 3. Acciones Kaizen (Hoja de Ruta para el Executor)

### 3.1. Refactorizar `GetUserPermissionsAsync` en `AuthService`
**Instrucción:** Modificar la recolección de permisos para aprovechar las capacidades nativas de `HashSet<string>`. Inicializar el HashSet con `directPermissions` y agregar los permisos de grupo usando `.UnionWith(...)` filtrando los nulos/vacíos.

**Fragmento de código sugerido (reemplazo):**
```csharp
    public async Task<HashSet<string>> GetUserPermissionsAsync(Guid userId)
    {
        // Permisos directos del usuario
        var directPermissions = await _context.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission.Key)
            .ToListAsync();

        var permissions = new HashSet<string>(directPermissions);

        // Permisos de los grupos a los que pertenece el usuario
        var groupPermissions = await _context.UserGroups
            .Include(ug => ug.Group)
                .ThenInclude(g => g!.GroupPermissions)
                    .ThenInclude(gp => gp!.Permission)
            .Where(ug => ug.UserId == userId && ug.Group != null)
            .SelectMany(ug => ug.Group!.GroupPermissions
                .Where(gp => gp != null && gp.Permission != null)
                .Select(gp => gp!.Permission!.Key))
            .ToListAsync();

        permissions.UnionWith(groupPermissions.Where(p => !string.IsNullOrEmpty(p)));

        return permissions;
    }
```

**Definition of Done (DoD):**
- El bucle `foreach` para agregar permisos debe ser eliminado.
- La creación de `HashSet<string>` debe inicializarse utilizando el constructor que recibe `IEnumerable` (`directPermissions`).
- Se debe usar `.UnionWith` para agregar los permisos de grupo a la colección de forma optimizada.
- El proyecto debe compilar correctamente.
- Las pruebas de la solución deben pasar (`dotnet test src/GesFer.Product.sln`).
