---
created: 2026-03-28
status: PENDING
priority: HIGH
---
# Kaizen 2026-03-28: Fix Namespace

**Action:** Rename the namespace in `GetAllUsersCommandHandlerPerformanceTests.cs`
**File:** `src/tests/GesFer.Product.UnitTests/Handlers/User/GetAllUsersCommandHandlerPerformanceTests.cs`

**Details:**
The audit log `AUDITORIA_2026_03_28.md` indicates an infraction of the namespace rule. "Ensure all namespaces strictly start with the base namespace 'GesFer.Product.Back'". The test class needs to use `GesFer.Product.Back.UnitTests.Handlers.User`.

**Definition of Done (DoD):**
- The file includes the full prefix `GesFer.Product.Back`.
- The solution compiles correctly (`dotnet build`).
- The tests run successfully without regressions (`dotnet test`).
