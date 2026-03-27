---
title: Auth Fix Plan
---
# Plan
1. Update `CityControllerTests`, `CountryControllerTests`, `GroupControllerTests`, `StateControllerTests`, and `SetupControllerTests` to implement `IAsyncLifetime` and use `SetAuthTokenAsync`.
2. Update existing `SetAuthTokenAsync` in `UserControllerTests`, `CustomerControllerTests`, `SupplierControllerTests` to use `EnsureSuccessStatusCode`.
