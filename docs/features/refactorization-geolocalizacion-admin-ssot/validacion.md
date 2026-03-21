---
feature_name: refactorization-geolocalizacion-admin-ssot
created: '2026-03-21'
branch: feat/refactorization-geolocalizacion-admin-ssot
checks:
  build: ok
  unit_tests: ok
  integration_tests: parcial
git_changes: ejecutar validación final en rama feat
---

# Validación — geolocalización Admin SSOT

## Resultados (marzo 2026)

| Comprobación | Estado |
|--------------|--------|
| `dotnet build` (Api) | Correcto |
| Unit tests (`GesFer.Product.UnitTests`) | 22 superados |
| Integration tests | 56 superados / 23 fallos (401 en login en colección DatabaseStep en entorno local; revisar Docker/seed si aplica) |

## Pendiente manual

- Ejecutar integración con MySQL + Docker estable y confirmar login + CRUD User/Supplier tras migración aplicada.
- Aplicar migración `RemoveLocalGeoCatalog` en entornos compartidos antes de desplegar.
