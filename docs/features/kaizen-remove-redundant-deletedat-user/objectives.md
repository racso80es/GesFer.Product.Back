---
name: Remove redundant DeletedAt checks in User
---
# Objectives
- Remove redundant `DeletedAt == null` checks in `User` command handlers as EF Core global query filters handle it automatically.
