---
created: 2026-06-20
type: kaizen
status: DONE
---

# Optimización de asignación en JwtService

Reemplazar el bucle manual `foreach` en `src/Infrastructure/Services/JwtService.cs` (líneas 63-66) por `claims.AddRange(...)` para mejorar el rendimiento, o similar, dado que estamos agregando múltiples permisos al List<Claim>.
