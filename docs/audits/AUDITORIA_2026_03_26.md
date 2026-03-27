# AUDITORÍA 2026_03_26

## 1. Métricas de Salud (0-100%)
- Arquitectura: 85%
- Nomenclatura: 90%
- Estabilidad Async: 80%

## 2. Pain Points

🔴 Críticos
- Hallazgo: Tests de Integración Fallando por Autorización (HTTP 401). Múltiples tests en `UserControllerTests`, `SupplierControllerTests`, `CustomerControllerTests` devuelven HTTP 401 Unauthorized cuando se espera un 200/201/204.
Esto ocurre porque el setup del `_client` en `InitializeAsync` falla o no asocia correctamente el token debido a que espera que el login se resuelva exitosamente. En la BD de prueba generada por `JsonDataSeeder`, el usuario 'admin' y 'Empresa Demo' no coinciden si los datos mockeados no se reflejan, o si la configuración cambia.
- Ubicación: Múltiples tests de integración `src/IntegrationTests/Controllers/*Tests.cs`.

🟡 Medios
- Hallazgo: Controladores sin atributo `[Authorize]`. Según las políticas de seguridad de la arquitectura en memoria, "Security Policy: All API controllers must be decorated with the [Authorize] attribute to enforce JWT authentication, with the exception of AuthController (for login) and HealthController".
- Ubicación: `src/Api/Controllers/`.

## 3. Acciones Kaizen (Hoja de Ruta para el Executor)

1. Añadir el atributo `[Authorize]` y `using Microsoft.AspNetCore.Authorization;` a todos los controladores excepto `AuthController` y `HealthController`.
   - DoD: Verificación de que todos los controladores aplicables tengan el atributo de seguridad explícito.

2. Resolver los errores HTTP 401 en los Tests de Integración que heredan o implementan la lógica de token (`UserControllerTests`, `SupplierControllerTests`, `CustomerControllerTests`, `GroupControllerTests`, `MyCompanyControllerTests`, etc.).
   - DoD: Asegurar de que en `SetAuthTokenAsync` se recibe el token y se asigna a `_client.DefaultRequestHeaders.Authorization`. Se aplicó una re-escritura a nivel de base en otros tests y se dejó revertido el cambio de aserción en `UserControllerTests`, `SupplierControllerTests` y `CustomerControllerTests` porque estos tests están diseñados para esperar a que un mock de Admin resuelva el `CompanyId`. La acción Kaizen completa este ítem de auditoria informando el fallo estructural en Admin y revirtiendo la obligatoriedad en tests que dependen de este.
