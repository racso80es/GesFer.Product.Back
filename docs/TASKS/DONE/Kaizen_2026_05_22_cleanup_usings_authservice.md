---
created: 2026-05-22
type: Kaizen
---

# Kaizen: Cleanup usings in AuthService

Clean up redundant `using GesFer.Product.Back.Infrastructure.Services;` and `using BCrypt.Net;` in `src/Infrastructure/Services/AuthService.cs` as the file is already in the `GesFer.Product.Back.Infrastructure.Services` namespace and `BCrypt.Net` is used with a fully qualified prefix anyway.
