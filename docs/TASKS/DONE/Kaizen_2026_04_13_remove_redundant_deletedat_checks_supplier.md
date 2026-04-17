---
name: Remove redundant DeletedAt checks in Supplier handlers
created: 2026-04-13
type: Kaizen
---
# Kaizen: Remove redundant DeletedAt == null checks in Supplier handlers

## Descripción
Los filtros globales de consultas de EF Core manejan automáticamente los borrados lógicos en el proyecto. Por lo tanto, revisar explícitamente `DeletedAt == null` en las consultas LINQ es redundante y no está permitido según las guías de desarrollo (memory rules).

Este Kaizen se centra en eliminar estos chequeos redundantes en los manejadores de comandos y consultas relacionados con la entidad `Supplier` (`CreateSupplierCommandHandler.cs`, `GetSupplierByIdCommandHandler.cs`, `UpdateSupplierCommandHandler.cs`, `DeleteSupplierCommandHandler.cs`, `GetAllSuppliersCommandHandler.cs`).
