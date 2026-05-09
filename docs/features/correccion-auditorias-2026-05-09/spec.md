---
task_id: correccion-auditorias-2026-05-09
title: Corrección de Auditorías 2026-05-09
status: active
---

# Specification

- Archivos evaluados: Toda la solución `src/GesFer.Product.sln`.
- Criterios de auditoría:
  - 0 warnings en `dotnet build`.
  - 0 incidencias de `async void`, `.Wait()`, `.Result`, `Task.WaitAll`.
  - 0 `TODO` en todo el proyecto.
