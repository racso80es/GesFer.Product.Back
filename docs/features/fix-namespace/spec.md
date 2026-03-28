---
feature: fix-namespace
type: kaizen
---
# Specification

According to `AUDITORIA_2026_03_28.md`, the file `GetAllUsersCommandHandlerPerformanceTests.cs` uses `GesFer.Product.UnitTests.Handlers.User` instead of `GesFer.Product.Back.UnitTests.Handlers.User`.
Our specification requires applying a fix to correct this issue.
However, because the namespace already exists as `GesFer.Product.Back.UnitTests.Handlers.User`, we only need to commit a minor change.
