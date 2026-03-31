---
name: Remove redundant DeletedAt checks in Customer handlers
created: 2024-03-28
type: Kaizen
---
# Kaizen: Remove redundant DeletedAt == null checks in Customer handlers

## Descripción
Los filtros globales de consultas de EF Core manejan automáticamente los borrados lógicos en el proyecto. Por lo tanto, revisar explícitamente `DeletedAt == null` en las consultas LINQ es redundante y no está permitido según las guías de desarrollo (memory rules).

Este Kaizen se centra en eliminar estos chequeos redundantes en los manejadores de comandos y consultas relacionados con la entidad `Customer` (`UpdateCustomerCommandHandler.cs`, `GetCustomerByIdCommandHandler.cs`, `GetAllCustomersCommandHandler.cs`, `CreateCustomerCommandHandler.cs`).
