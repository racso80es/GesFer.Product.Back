---
feature_name: API Documentation Discoverability
created: 2024-06-14
---
# Specification
Review all API controllers in `src/Api/Controllers/` to ensure that every endpoint method decorated with `[Http*]` attributes includes a proper `/// <summary>` XML documentation block.
If any are found missing, they should be added.
Since a programmatic check revealed that all existing endpoints already have XML documentation blocks, this task will be closed without code modifications.
