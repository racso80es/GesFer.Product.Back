---
type: implementation
---
# Implementation
Change `Assert.Equal(0, mockAdminApiClient.Invocations.Count)` to `Assert.Empty(mockAdminApiClient.Invocations)`.