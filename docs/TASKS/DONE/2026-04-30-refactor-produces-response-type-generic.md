---
title: Refactor ProducesResponseType to generic syntax
type: kaizen
status: active
---

# Refactor ProducesResponseType to generic syntax

## Context
The API project targets .NET 8.0. Currently, API controllers use the older `[ProducesResponseType(typeof(T), statusCode)]` syntax. The memory guideline indicates: "The API project targets .NET 8.0. API controllers must use the modern generic `[ProducesResponseType<T>(statusCode)]` attribute rather than the older `[ProducesResponseType(typeof(T), statusCode)]` syntax."

## Objective
Refactor all instances of `[ProducesResponseType(typeof(T), statusCode)]` in `src/Api/Controllers/*.cs` to use the modern generic syntax `[ProducesResponseType<T>(statusCode)]`.

## Execution
- Update all occurrences in API controllers.
- Run `dotnet build src/GesFer.Product.sln` to verify successful compilation.
- Run `dotnet test src/GesFer.Product.sln` to ensure tests are not broken.
