---
title: Plan
feature_id: kaizen-2026-03-27
status: IN_PROGRESS
---

# Plan

1. Open `src/Infrastructure/Services/MasterDataSeeder.cs`.
2. Locate the intermediate `await _context.SaveChangesAsync();` within the nested `foreach` loops in the `SeedSpanishCitiesAndPostalCodesAsync` method.
3. Remove those `await _context.SaveChangesAsync();` statements.
4. Verify code compiles.
5. Verify tests run and pass without regressions.
