---
id: T-2026-03-28-001
created: 2026-03-28
type: Kaizen
status: pending
---

# Tarea Kaizen: Renombrar namespace en GetAllUsersCommandHandlerPerformanceTests.cs

**Origen:** Hallazgo Medio en AUDITORIA_2026_03_28.md

**Descripción:**
Infracción a la política estricta de nomenclatura. El test de rendimiento `GetAllUsersCommandHandlerPerformanceTests.cs` usa el namespace `GesFer.Product.UnitTests.Handlers.User` en lugar de `GesFer.Product.Back.UnitTests.Handlers.User`.

**Acciones a realizar:**
Reemplazar el namespace base del test `GetAllUsersCommandHandlerPerformanceTests` a `GesFer.Product.Back.UnitTests.Handlers.User`.

**Definition of Done (DoD):**
- El archivo `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` incluye el prefijo completo `GesFer.Product.Back`.
- La solución compila correctamente.
- Las pruebas se ejecutan de manera satisfactoria y sin regresiones.