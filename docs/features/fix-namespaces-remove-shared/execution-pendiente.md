---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
type: execution-pendiente
plan_ref: docs/features/fix-namespaces-remove-shared/plan-pendiente.md
---

# Execution pendiente: Fase 5-8

## Fase 5: Namespaces IntegrationTests — ✓ Completada

| Tarea | Resultado |
|-------|-----------|
| 5.1-5.10 | Todos los namespaces `GesFer.IntegrationTests` → `GesFer.Product.Back.IntegrationTests` |

**Archivos modificados:** DatabaseFixture, DatabaseCollection, IntegrationTestWebAppFactory, SharedTestCollection, WebApplicationFactory, Helpers (4), Controllers (12), Services (4).

**Build:** ✓ Sin errores  
**Tests:** ✓ 108 pasando

---

## Fase 6: Scripts — ✓ Verificada (sin cambios)

| Archivo | Estado |
|---------|--------|
| setup-database.ps1 | Ruta `../Infrastructure/GesFer.Infrastructure.csproj` correcta |
| Migrations/README.md | Comandos correctos |

---

## Fase 8: Dockerfile — ⚠ Requiere actualización

**Estructura actual del repo:** `src/Api/`, `src/application/`, `src/domain/`, `src/Infrastructure/`

**Dockerfile actual referencia:**
- `src/Product/Back/Api/` — no existe (debería ser `src/Api/`)
- `src/Product/Back/application/` — no existe
- `src/Product/Back/domain/` — no existe
- `src/Shared/Back/` — eliminado en este refactor

**Acción recomendada:** Actualizar Dockerfile y Dockerfile.test para usar rutas `src/Api/`, `src/application/`, `src/domain/`, `src/Infrastructure/` y eliminar referencia a Shared.

---

## Fase 7: Renombrar proyectos — Pendiente (opcional)

No ejecutada. Recomendada como feature separada por impacto en CI/CD.
