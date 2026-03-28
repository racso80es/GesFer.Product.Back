---
feature: fix-namespace
type: kaizen
---
# Objectives

The objective is to fix a reported namespace infraction in `GetAllUsersCommandHandlerPerformanceTests.cs`.
The audit indicated the test class was not using the strictly required `GesFer.Product.Back` prefix.
However, upon inspection, the namespace is already correct.
