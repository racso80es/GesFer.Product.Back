---
title: "Clarifications - MasterDataSeeder Optimization"
date: "2026-04-25"
---

# Clarifications

*   **Memory overhead**: Will loading all cities and postal codes for Spain into memory cause out-of-memory exceptions?
    *   *Resolution*: No, the datasets are small enough (few thousand records max) to easily fit into memory, and it is significantly more efficient than thousands of individual queries.
*   **Query Filters**: Must we use `.IgnoreQueryFilters()`?
    *   *Resolution*: Yes, per the system memory and constraints, seeders must ignore query filters to properly find and restore soft-deleted entities rather than creating duplicates.
