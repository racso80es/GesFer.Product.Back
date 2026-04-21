---
type: validacion
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
---

# Validación — run-test-e2e-local

## Criterios

| Criterio | Estado |
|----------|--------|
| Script ejecuta fases en orden (init → … → tests) | Cumplido |
| `E2E_BASE_URL` y `E2E_INTERNAL_SECRET` aplicados al proceso `dotnet test` | Cumplido (variables de entorno del proceso PowerShell) |
| Probe falla si health no responde (sin SkipApiProbe) | Cumplido |
| Salida JSON opcional sin secretos en texto claro | Cumplido (no se loguea el secret) |
| Documentación y manifest alineados | Cumplido |

## Evidencia

- Ejecución local: build + tests con `ProductApiUrl` inalcanzable y `-SkipApiProbe` → tests omitidos, código 0.

## Pendiente

- Validación con ambas APIs reales y los 3 E2E en verde (entorno del desarrollador).
