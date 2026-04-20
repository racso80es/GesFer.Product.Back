---
name: Remove redundant DeletedAt checks in User handlers
created: 2026-04-16
type: Kaizen
---
# Kaizen: Remove redundant DeletedAt == null checks in User handlers

## Descripción
Los filtros globales de consultas de EF Core manejan automáticamente los borrados lógicos en el proyecto. Por lo tanto, revisar explícitamente `DeletedAt == null` en las consultas LINQ es redundante y no está permitido según las guías de desarrollo (memory rules).

Este Kaizen se centra en eliminar estos chequeos redundantes en los manejadores de comandos y consultas relacionados con la entidad `User` (`CreateUserCommandHandler.cs`, `GetUserByIdCommandHandler.cs`, `UpdateUserCommandHandler.cs`, `DeleteUserCommandHandler.cs`, `GetAllUsersCommandHandler.cs`).
