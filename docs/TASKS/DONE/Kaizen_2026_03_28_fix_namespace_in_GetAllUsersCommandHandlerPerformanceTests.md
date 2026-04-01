---
title: Kaizen - Fix namespace in GetAllUsersCommandHandlerPerformanceTests
created: 2026-03-28
status: PENDING
---

# Kaizen - Fix namespace in GetAllUsersCommandHandlerPerformanceTests

## Description
Renombrar namespace en GetAllUsersCommandHandlerPerformanceTests.cs. Reemplazar el namespace base del test `GetAllUsersCommandHandlerPerformanceTests` de `GesFer.Product.UnitTests.Handlers.User` a `GesFer.Product.Back.UnitTests.Handlers.User`.

## Definition of Done (DoD):
- El archivo `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` incluye el prefijo completo `GesFer.Product.Back`.
- La solución compila correctamente (`dotnet build`).
- Las pruebas se ejecutan de manera satisfactoria y sin regresiones (`dotnet test`).