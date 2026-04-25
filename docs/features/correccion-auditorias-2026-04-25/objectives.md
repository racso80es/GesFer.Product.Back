---
title: "Objectives - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Objectives

Optimize `MasterDataSeeder.cs` to eliminate N+1 queries during the seeding process for Spain data (languages, states, cities, and postal codes). The current implementation uses `FirstOrDefaultAsync` inside nested loops, which degrades performance and scale.

The objective is to replace these single-record lookups with pre-loaded, in-memory dictionaries containing all relevant records, enabling O(1) lookups inside the seeding loops. This will adhere to the "Testability, Audit & Judge" pattern to eliminate this technical debt and improve startup times.
