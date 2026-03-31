---
title: "Technical Specification: Fix namespace in GetAllUsersCommandHandlerPerformanceTests"
date: "2026-03-28"
---
# Specification

## Problem
The unit test file `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` has a namespace violation according to `docs/audits/AUDITORIA_2026_03_28.md`.

## Solution
Ensure the namespace of `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` starts strictly with `GesFer.Product.Back` (e.g., `namespace GesFer.Product.Back.UnitTests.Handlers.User;`).
