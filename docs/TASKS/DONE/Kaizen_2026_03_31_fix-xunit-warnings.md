---
name: Fix xUnit2013 Warning
date: 2026-03-31
type: kaizen
---
# Fix xUnit2013 Warning

Change `Assert.Equal(0, mockAdminApiClient.Invocations.Count)` to `Assert.Empty(mockAdminApiClient.Invocations)` to avoid xUnit2013 warnings as indicated by project norms.