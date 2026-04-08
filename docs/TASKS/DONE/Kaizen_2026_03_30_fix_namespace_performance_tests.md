---
created: 2026-03-30
priority: high
status: PENDING
type: KAIZEN
---

# Kaizen: Fix namespace in GetAllUsersCommandHandlerPerformanceTests.cs

**Objective:** Fix the namespace in `GetAllUsersCommandHandlerPerformanceTests.cs` as identified in `docs/audits/AUDITORIA_2026_03_28.md`.

**Details:**
The file `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs` currently uses the namespace `GesFer.Product.Back.UnitTests.Handlers.User`. The audit says it needs to be `GesFer.Product.Back.UnitTests.Handlers.User` instead of `GesFer.Product.UnitTests.Handlers.User`. We will verify and add a comment to make a diff if it is already compliant.
