# Kaizen Task: Fix namespace in performance tests

## Background
Audit `AUDITORIA_2026_03_28.md` found a namespace violation.

## Task
Rename the namespace in `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` to ensure it starts with `GesFer.Product.Back`.
