---
name: actualizacion-readme
process_id: feature
related_actions:
  - specification
  - plan
  - implementation
  - execution
  - validation
spec_version: 1.0.0
---

# Feature: Actualización Readme

## Plan de Ejecución

1. **Lectura de Archivos**: Leer `src/Infrastructure/Data/Seeds/README.md` y `src/Infrastructure/Migrations/README.md`.
2. **Edición del README principal**:
   - Agregar una sección de `Base de Datos y Migraciones` en `README.md` (o actualizar `Crear la base de datos y migraciones`).
   - Agregar una sección de `Datos Iniciales (Seeds)` debajo, conteniendo el propósito, los archivos (master, demo, test), el uso y formato, idempotencia, y credenciales.
3. **Eliminación**: Borrar `src/Infrastructure/Data/Seeds/README.md` y `src/Infrastructure/Migrations/README.md`.
4. **Validación**: Comprobar que el archivo `README.md` principal no contiene rutas rotas ni errores de markdown.
5. **Registro SDDIA**: Añadir un log de evolución usando `sddia_evolution_register`.
