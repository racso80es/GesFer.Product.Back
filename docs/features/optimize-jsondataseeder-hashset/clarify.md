---
date: "2026-05-03"
---
# Clarifications
- The `validUserIds` HashSet is passed to `SeedUsersAsync` as a parameter. It will be collected in a `List<Guid>` first during iteration and then combined at the end.
