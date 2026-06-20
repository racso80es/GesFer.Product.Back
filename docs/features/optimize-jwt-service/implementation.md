---
title: Implementation for JwtService optimization
---
# Implementation
Modified `src/Infrastructure/Services/JwtService.cs` replacing `foreach` with `claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));`.
