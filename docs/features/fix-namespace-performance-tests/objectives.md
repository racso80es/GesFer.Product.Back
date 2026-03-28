---
title: "Fix namespace in GetAllUsersCommandHandlerPerformanceTests"
date: "2026-03-28"
status: "IN_PROGRESS"
---
# Fix namespace in GetAllUsersCommandHandlerPerformanceTests

## Objective
Fix the namespace violation in `GetAllUsersCommandHandlerPerformanceTests.cs` as reported in the audit `AUDITORIA_2026_03_28.md`. The namespace must strictly start with the base namespace 'GesFer.Product.Back'.

## Context
The file `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` uses an incorrect namespace `GesFer.Product.UnitTests.Handlers.User` instead of `GesFer.Product.Back.UnitTests.Handlers.User`.

## Definition of Done (DoD):
- El archivo `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` incluye el prefijo completo `GesFer.Product.Back`.
- La solución compila correctamente (`dotnet build`).
- Las pruebas se ejecutan de manera satisfactoria y sin regresiones (`dotnet test`).