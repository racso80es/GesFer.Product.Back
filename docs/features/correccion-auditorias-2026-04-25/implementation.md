---
title: "Implementation - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Implementation

The code changes trace the execution plan, strictly adhering to the "Testability, Audit & Judge" pattern to remove the technical debt of N+1 database queries.

```csharp
// SeedLanguagesAsync
var existingLanguages = await _context.Languages
    .IgnoreQueryFilters()
    .ToDictionaryAsync(l => l.Code);

foreach(var lang in languages) {
    if(existingLanguages.TryGetValue(lang.Code, out var existing)) {
        // ...
    }
}

// SeedSpainDataAsync
var existingStates = await _context.States
    .IgnoreQueryFilters()
    .Where(s => s.CountryId == countryId)
    .ToDictionaryAsync(s => s.Code);

// SeedSpanishCitiesAndPostalCodesAsync
var stateIds = states.Select(s => s.Id).ToList();
var existingCities = await _context.Cities
    .IgnoreQueryFilters()
    .Where(c => stateIds.Contains(c.StateId))
    .ToDictionaryAsync(c => new { c.StateId, c.Name });
```
