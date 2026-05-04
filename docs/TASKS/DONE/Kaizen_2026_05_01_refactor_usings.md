---
title: Refactor unused using directives
type: kaizen
status: done
---

# Refactor unused using directives

## Context
During code exploration, unused using directives were spotted in controllers (e.g. `GesFer.Product.Back.Application.DTOs.Admin` and `GesFer.Product.Back.Infrastructure.Logging` in `DashboardController.cs`). According to memory guidelines: "When performing code health improvements, ensure that `using` statements are cleaned up to prevent the loading of unnecessary metadata from unused namespaces in the compiled binary."

## Objective
Remove unused `using` statements in all API controllers to improve code health.

## Execution
- Update controllers to clean up unused namespaces.
- Run `dotnet build src/GesFer.Product.sln` to ensure build succeeds.
- Run `dotnet test src/GesFer.Product.sln` to ensure tests are not broken.
