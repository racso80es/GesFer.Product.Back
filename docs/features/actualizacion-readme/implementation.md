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

## Implementación

He modificado el archivo `README.md` principal para incluir el texto consolidado que antes residía en:
1. `src/Infrastructure/Data/Seeds/README.md`
2. `src/Infrastructure/Migrations/README.md`

Las secciones de Base de Datos y Migraciones ahora incluyen cómo correr `dotnet ef database update` y sobre cómo tratar las migraciones existentes.
La sección de `2.1 Datos Iniciales (Seeds)` cubre cómo modificar `master-data.json`, `demo-data.json`, la idempotencia del seeder, y las credenciales por defecto.

Los archivos secundarios se han eliminado correctamente.
