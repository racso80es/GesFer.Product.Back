# Auditoría de Mantenibilidad - 2026-04-06

## 1. Métricas de Salud (0-100%)
Arquitectura: 90% | Nomenclatura: 100% | Estabilidad Async: 100%

## 2. Pain Points (🔴 Críticos / 🟡 Medios)
🟡 Hallazgo: Redundancia en filtros de Soft Delete (`DeletedAt == null`). EF Core Global Query Filters ya manejan esto automáticamente.
Ubicación: Multiples archivos en `src/application/Handlers/` y `src/Infrastructure/Services/`.

## 3. Acciones Kaizen (Hoja de Ruta para el Executor)
Eliminar las comprobaciones explícitas de `DeletedAt == null`.
Cuando se elimine el `.Where` por completo, asegurar que quede como `.AsQueryable()`.
**Definition of Done (DoD):** Todos los usos redundantes de `DeletedAt == null` eliminados y tests pasando.