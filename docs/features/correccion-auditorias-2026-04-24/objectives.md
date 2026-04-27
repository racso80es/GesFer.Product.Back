---
status: Open
priority: High
---
# Objetivos de la corrección
- Refactorizar `GetUserPermissionsAsync` en `AuthService` para optimizar la recolección de permisos.
- Eliminar los bucles `foreach` y utilizar constructores e intersecciones nativas de `HashSet<string>`.
