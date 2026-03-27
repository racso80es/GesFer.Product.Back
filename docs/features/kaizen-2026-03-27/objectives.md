---
title: Objectives
feature_id: kaizen-2026-03-27
status: IN_PROGRESS
---

# Objectives

Improve the seeding performance by reducing the number of database context saves. Specifically, remove intermediate `SaveChangesAsync` calls from loops in `MasterDataSeeder.cs` so entities can be inserted efficiently using the single `SaveChangesAsync` call at the end.
