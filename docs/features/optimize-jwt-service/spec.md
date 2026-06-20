---
title: Spec for JwtService optimization
---
# Specification
The `GenerateToken` method in `src/Infrastructure/Services/JwtService.cs` currently uses a `foreach` loop to add permissions to the claims list. This will be replaced with `claims.AddRange` combined with LINQ `Select` to improve performance.
