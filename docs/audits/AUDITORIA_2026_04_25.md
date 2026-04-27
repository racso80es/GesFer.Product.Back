# Auditoría de Infraestructura y Mantenibilidad (S+)
**Fecha**: 2026-04-25

## 1. Métricas de Salud (0-100%)
*   **Arquitectura:** 90%
*   **Nomenclatura:** 100%
*   **Estabilidad Async:** 100%

## 2. Pain Points (🔴 Críticos / 🟡 Medios)
**Hallazgo:** 🔴 Problema de rendimiento N+1 en MasterDataSeeder. Se están realizando múltiples consultas a la base de datos dentro de bucles anidados utilizando `FirstOrDefaultAsync` para verificar la existencia de idiomas, países, provincias, ciudades y códigos postales. Esto causa una degradación severa del rendimiento (N+1 queries).

**Ubicación:** `src/Infrastructure/Services/MasterDataSeeder.cs` (Múltiples líneas: 68, 93, 159, 243, 275, 304).

## 3. Acciones Kaizen (Hoja de Ruta para el Executor)
**Objetivo:** Optimizar `MasterDataSeeder.cs` para cargar los datos en memoria utilizando diccionarios compuestos y evitar consultas individuales por cada registro.

**Instrucciones Exactas:**
1.  **Refactorizar `SeedLanguagesAsync`**: Obtener todos los idiomas en un solo `ToListAsync` y cargarlos en un diccionario usando `lang.Code` como clave.
2.  **Refactorizar `SeedSpainDataAsync`**: El país (España) se comprueba y añade correctamente (solo 1 query). Las provincias también deben optimizarse si es posible, aunque es un bucle pequeño.
3.  **Refactorizar `SeedSpanishCitiesAndPostalCodesAsync`**:
    *   Cargar todas las ciudades de las provincias obtenidas en un diccionario: `var existingCities = await _context.Cities.IgnoreQueryFilters().Where(c => stateIds.Contains(c.StateId)).ToDictionaryAsync(c => new { c.StateId, c.Name });`.
    *   Cargar todos los códigos postales de las ciudades a procesar en un diccionario: `var existingPostalCodes = await _context.PostalCodes.IgnoreQueryFilters().Where(pc => cityIds.Contains(pc.CityId)).ToDictionaryAsync(pc => new { pc.CityId, pc.Code });`.
    *   Sustituir los `FirstOrDefaultAsync` dentro de los bucles de ciudades y códigos postales por búsquedas O(1) en los diccionarios (`existingCities.TryGetValue` y `existingPostalCodes.TryGetValue`).

**Definition of Done (DoD):**
*   Ningún `FirstOrDefaultAsync` (ni otras consultas a DB) dentro de bucles `foreach` en `MasterDataSeeder.cs`.
*   El seeder debe funcionar correctamente (creando o restaurando registros) utilizando las claves compuestas dictadas por el sistema.
*   El código debe compilar (`dotnet build`).
*   Los tests relevantes deben pasar.
