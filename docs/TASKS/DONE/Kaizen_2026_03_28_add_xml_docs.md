---
id: Kaizen_2026_03_28_add_xml_docs
title: Add XML Docs to ArticleFamiliesController and TaxTypesController
priority: Low
date: 2026-03-28
status: PENDING
---

# Add XML Docs to ArticleFamiliesController and TaxTypesController

## Objective
Add missing XML documentation comments to `ArticleFamiliesController.cs` and `TaxTypesController.cs` for better API discoverability via Swagger/OpenAPI.

## Description
Currently, `ArticleFamiliesController.cs` and `TaxTypesController.cs` lack properly `<summary>` tags for endpoints. This Kaizen action will add them to ensure consistent and fully documented API endpoints.

## Scope
- Modify `src/Api/Controllers/ArticleFamiliesController.cs` to include XML docs for all endpoints.
- Modify `src/Api/Controllers/TaxTypesController.cs` to include XML docs for all endpoints.