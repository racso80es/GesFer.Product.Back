---
title: Specification
feature_id: kaizen-2026-03-27
status: IN_PROGRESS
---

# Specification

Modify `src/Infrastructure/Services/MasterDataSeeder.cs` inside the method `SeedSpanishCitiesAndPostalCodesAsync`. Remove the `await _context.SaveChangesAsync();` lines found around 259, 266, 293, and 300 so that insertions and updates are batched properly instead of causing an N+1 performance bottleneck. A single `await _context.SaveChangesAsync();` call is preserved outside these internal loops on line 336.
