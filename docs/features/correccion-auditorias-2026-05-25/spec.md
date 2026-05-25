---
id: 'correccion-auditorias-2026-05-25-spec'
title: 'Specification for Audit 2026-05-25'
description: 'Search perimeter and boundaries for the 2026-05-25 structural audit'
created: '2026-05-25'
---

# Specification

The search perimeter includes all `.cs` files inside `src/`. The goal is to verify the non-existence of `.Result`, `.Wait()`, `Task.WaitAll`, `async void`, and technical debt markers (`TODO`). The solution `src/GesFer.Product.sln` must compile successfully (0 errors, 0 warnings) and all tests must pass.
