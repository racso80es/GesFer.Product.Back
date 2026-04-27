---
title: "Specification - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Specification

Based on the audit report (AUDITORIA_2026_04_25), the optimization will focus strictly on `src/Infrastructure/Services/MasterDataSeeder.cs`.

## Targeted Methods
*   `SeedLanguagesAsync()`
*   `SeedSpainDataAsync()`
*   `SeedSpanishCitiesAndPostalCodesAsync()`

## Changes Required
1.  **Languages:** Load all languages in memory before the loop. Check dictionary for existing languages.
2.  **States:** The existing check is relatively small, but `FirstOrDefaultAsync` inside the `spanishProvinces` loop can be optimized.
3.  **Cities & Postal Codes:** Use `.ToDictionaryAsync(c => new { c.StateId, c.Name })` for cities and `.ToDictionaryAsync(pc => new { pc.CityId, pc.Code })` for postal codes before entering the loops. Replace all `FirstOrDefaultAsync` calls with `dictionary.TryGetValue`.

## Constraints
*   Global Query Filters must be bypassed using `.IgnoreQueryFilters()` to handle restoring soft-deleted records correctly.
*   Dictionary keys must exactly match the constraints: `new { c.StateId, c.Name }` and `new { pc.CityId, pc.Code }`.
*   Maintain the logic that restores records (`DeletedAt = null`, `IsActive = true`) if they are found as soft-deleted.