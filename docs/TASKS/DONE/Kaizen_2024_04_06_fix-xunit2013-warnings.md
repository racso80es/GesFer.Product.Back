---
title: Fix xUnit2013 warnings in Performance Tests
created: 2024-04-06
priority: high
---
# Fix xUnit2013 warnings in Performance Tests

The test `GetAllUsersCommandHandlerPerformanceTests.cs` uses `Assert.Equal(0, mockAdminApiClient.Invocations.Count)`. According to project memory, this causes xUnit2013 warnings. It must be updated to use `Assert.Empty(mockAdminApiClient.Invocations)`.
