---
name: Remove redundant DeletedAt checks in Supplier handlers
created: 2026-04-13
type: feature
---
# Objectives
Remove redundant `DeletedAt == null` checks from Supplier handlers since EF Core Global Query Filter already handles logical deletes automatically.