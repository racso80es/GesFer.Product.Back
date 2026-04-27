---
title: "Plan - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Plan

1.  **Refactor `SeedLanguagesAsync`**:
    *   Query all languages without query filters into a dictionary keyed by `Code`.
    *   Iterate through the required languages and use the dictionary to find existing records instead of `FirstOrDefaultAsync`.

2.  **Refactor `SeedSpainDataAsync`**:
    *   Query existing states for the country `ES` without query filters into a dictionary keyed by `Code`.
    *   Use the dictionary to check if a state exists and restore it if necessary.

3.  **Refactor `SeedSpanishCitiesAndPostalCodesAsync`**:
    *   Gather all `state.Id`s and query existing cities without query filters. Load into a dictionary keyed by `new { StateId, Name }`.
    *   Gather all `city.Id`s (from existing and newly created cities, although newly created ones won't be in the DB yet, they are added to context) and query existing postal codes without query filters. Load into a dictionary keyed by `new { CityId, Code }`.
    *   Replace `FirstOrDefaultAsync` calls with O(1) dictionary lookups.
