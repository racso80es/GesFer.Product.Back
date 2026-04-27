---
title: "Execution - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Execution

*   Modified `MasterDataSeeder.cs` to eliminate N+1 queries.
*   Replaced `FirstOrDefaultAsync` inside `foreach` loops with dictionary lookups.
*   Used `.IgnoreQueryFilters()` correctly for data seeders to manage soft-deletes.
*   Confirmed build success.
*   Confirmed test execution.