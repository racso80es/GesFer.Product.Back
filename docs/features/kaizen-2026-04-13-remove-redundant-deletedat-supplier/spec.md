---
name: Remove redundant DeletedAt checks in Supplier handlers
created: 2026-04-13
type: feature
---
# Specification
Remove explicitly coded `DeletedAt == null` checks in LINQ queries for the `Supplier` entity in:
- `CreateSupplierCommandHandler.cs`
- `GetSupplierByIdCommandHandler.cs`
- `UpdateSupplierCommandHandler.cs`
- `DeleteSupplierCommandHandler.cs`
- `GetAllSuppliersCommandHandler.cs`