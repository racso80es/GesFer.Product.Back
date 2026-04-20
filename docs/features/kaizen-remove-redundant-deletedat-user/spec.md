---
version: 1.0.0
---
# Specification
- Update `DeleteUserCommandHandler.cs`, `GetUserByIdCommandHandler.cs`, `GetAllUsersCommandHandler.cs`, `UpdateUserCommandHandler.cs`, and `CreateUserCommandHandler.cs` to remove `&& u.DeletedAt == null` and similar checks for `DeletedAt`.
