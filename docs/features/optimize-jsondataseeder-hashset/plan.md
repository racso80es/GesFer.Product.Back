---
date: "2026-05-03"
---
# Plan
1. Modificar `src/Infrastructure/Services/JsonDataSeeder.cs` para colectar en `newValidUserIds` List<Guid>.
2. Usar `validUserIds.UnionWith(newValidUserIds)`.
