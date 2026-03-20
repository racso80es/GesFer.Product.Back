---
id: "correccion-auditorias"
status: "completed"
title: "Corrección Auditorias 2024-05-23"
version: "1.0.0"
---

# Objetivos de Corrección Auditorías 2024-05-23

## Hallazgos Consolidados

### Críticos
*   **Aislamiento y Nomenclatura:** Se detectó el uso de nombres de espacio obsoletos (`GesFer.Product.Application`) y el uso de la palabra `Shared` contraviniendo las directivas de clean architecture impuestas.
    *   *Acción:* Fueron refactorizados a `GesFer.Product.Back.Application` y "Common/Internal" respectivamente.
    *   *Estado:* Completado y commiteado en paso previo.

### Medios
*   **Intermitencia de Tests de Integración:** Los tests `SupplierControllerTests.cs` y `UserControllerTests.cs` reportan error 401 Unauthorized de forma intermitente debido a la pérdida del token.
    *   *Acción:* Se implementó `IAsyncLifetime` en ambas clases de test y se movió la ejecución de `SetAuthTokenAsync()` a `InitializeAsync()` para garantizar que el JWT sea proveído al cliente antes de cada `[Fact]`.
    *   *Estado:* Completado y verificado que no hay falsos positivos ni intermitencia en el setup.

## Criterios de Cierre
1.  ✅ Los hallazgos críticos de nomenclatura deben permanecer corregidos en master.
2.  ✅ Los tests de integración deben ser refactorizados para aplicar `IAsyncLifetime`.
3.  ✅ El `dotnet test` debe correr sin interrupciones y con una tasa de pase de 100%.

**RESULTADO FINAL:** Las auditorías del backend se consideran subsanadas y cerradas exitosamente. Todo el código pasa las pruebas y revisiones pertinentes sin reportar deuda técnica de Clean Code.