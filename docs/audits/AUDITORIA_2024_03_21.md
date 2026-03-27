# Auditoría de Código y Arquitectura

## 1. Métricas de Salud (0-100%)
Arquitectura: 90% | Nomenclatura: 95% | Estabilidad Async: 80%

## 2. Pain Points (🔴 Críticos / 🟡 Medios)

**Hallazgo:** 🔴 [Crítico] - Fallo Intermitente en Tests de Integración por Estado de Autenticación Mutado en `MyCompanyControllerTests`
**Descripción:** El test `UpdateMyCompany_WithValidToken_ShouldReturn200` renombra la empresa del test principal ("Empresa Demo") a "Empresa Demo Actualizada". Dado que todos los tests se ejecutan sobre la misma fixture compartida (`[Collection("DatabaseStep")]`), los tests de otros controladores (como `UserControllerTests` y `SupplierControllerTests`) que intentan autenticarse posteriormente usando las credenciales iniciales de "Empresa Demo" fallan intermitentemente con un 401 Unauthorized.
**Ubicación:** `src/IntegrationTests/Controllers/MyCompanyControllerTests.cs` (línea 75 y 88).

**Hallazgo:** 🟡 [Medio] - Inconsistencia en inicialización de Auth
**Descripción:** `CustomerControllerTests` y otros no usan `IAsyncLifetime` sino llamadas manuales a `SetAuthTokenAsync`, mientras que otros dependen de la interfaz global. Esto causó confusión inicial al diagnosticar el fallo. Aunque esto por sí solo no rompe los tests si la BD no se muta de forma destructiva (como ocurre con el renombrado de empresa).

## 3. Acciones Kaizen (Hoja de Ruta para el Executor)

1.  **Aislar mutaciones de estado compartido en tests de integración:**
    *   **Instrucciones:** Modificar `UpdateMyCompany_WithValidToken_ShouldReturn200` en `MyCompanyControllerTests.cs` para que no modifique la propiedad `Name` de "Empresa Demo" a "Empresa Demo Actualizada". Dejar que siga siendo "Empresa Demo" para que las peticiones subsiguientes de inicio de sesión de otros controladores no arrojen un código `401 Unauthorized`.
    *   **Definition of Done (DoD):** El comando `dotnet test src/GesFer.Product.sln` se ejecuta con 105 tests exitosos (100% de tests en verde sin intermitencias).

2. **Verificar nomenclatura "Shared":**
    *   **Instrucciones:** Ejecutar una búsqueda global de la palabra "Shared" en nombres de archivo y contenidos (exceptuando bin/obj).
    *   **Definition of Done (DoD):** No existen referencias al namespace, carpeta o clase `Shared` en el código productivo. (Ya validado, no se encontraron resultados).

## 4. Ejecución
Aplicar correcciones de mutación de estado en `MyCompanyControllerTests.cs`.
