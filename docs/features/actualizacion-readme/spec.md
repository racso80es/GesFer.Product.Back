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

## Especificación

El README.md principal del repositorio debe contener todas las instrucciones necesarias para levantar, configurar y comprender la solución.

Actualmente existen:
- `README.md` (raíz): Documentación principal.
- `src/Infrastructure/Data/Seeds/README.md`: Documentación sobre los Seeds de base de datos.
- `src/Infrastructure/Migrations/README.md`: Documentación sobre las migraciones de Entity Framework.

El trabajo consiste en:
1. Extraer el contenido de `src/Infrastructure/Data/Seeds/README.md`.
2. Extraer el contenido de `src/Infrastructure/Migrations/README.md`.
3. Crear nuevas secciones (o actualizar las existentes) en el `README.md` raíz.
4. Ajustar el formato del contenido para que encaje de forma natural en el README.md principal.
5. Eliminar los archivos `src/Infrastructure/Data/Seeds/README.md` y `src/Infrastructure/Migrations/README.md` tras la migración.
