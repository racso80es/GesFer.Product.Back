---
status: complete
---
# Implementation
- Removed redundant explicit checks for `DeletedAt == null` in User command handlers, because EF Core global query filter already handles it automatically.
