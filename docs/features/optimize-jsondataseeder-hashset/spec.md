---
date: "2026-05-03"
---
# Specification
The `JsonDataSeeder.cs` class iterates over `users` and adds them one by one to `validUserIds` HashSet via `.Add()`. This plan replaces it with collecting to a local list and utilizing `validUserIds.UnionWith()` after the loop.
