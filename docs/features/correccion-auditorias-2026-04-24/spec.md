---
version: 1.0.0
---
# Especificación
El método `GetUserPermissionsAsync` será modificado para instanciar `permissions` como `new HashSet<string>(directPermissions)` y se utilizará `.UnionWith()` para añadir los permisos de grupo filtrados.
