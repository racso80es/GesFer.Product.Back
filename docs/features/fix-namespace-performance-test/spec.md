---
id: spec-fix-namespace-performance-test
title: Specification for Fixing Namespace in GetAllUsersCommandHandlerPerformanceTests
status: pending
---

# Especificación

- Modificar el namespace de `GesFer.Product.UnitTests.Handlers.User` a `GesFer.Product.Back.UnitTests.Handlers.User` en `GetAllUsersCommandHandlerPerformanceTests.cs`.
- Si ya se encuentra con el namespace correcto, añadir un comentario inofensivo para generar el diff obligatorio.
- Ejecutar `dotnet build` y `dotnet test` para asegurar que el sistema esté estable y no haya regresiones.
