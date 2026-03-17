# Protocolo de Aceptación de Pull Requests

Este documento define las **condiciones obligatorias** que deben cumplirse antes de que cualquier Pull Request (PR) sea creado o aceptado. Este protocolo es aplicable a:
- Agentes de IA (Jules) durante la acción `finalize`.
- Desarrolladores locales (Cursor) antes de hacer `push`.
- Sistemas de CI/CD (GitHub Actions) como condición de bloqueo.

## Requisitos Obligatorios

1.  **Nomenclatura (Naming):**
    - Ejecución exitosa de `scripts/validate-nomenclatura.ps1`.
    - Ramas: `feat/<kebab-case>`, `fix/<kebab-case>`, `feat/refactorization-<kebab-case>`.
    - Commits: Formato convencional (opcional pero recomendado).

2.  **Compilación (.NET):**
    - La solución completa `src/GesFer.Admin.Back.sln` debe compilar sin errores.
    - Comando: `dotnet build src/GesFer.Admin.Back.sln`.

3.  **Pruebas (Tests):**
    - Todos los tests de la solución deben pasar.
    - Comando: `dotnet test src/GesFer.Admin.Back.sln`.

4.  **Ejecución:**
    - Se debe utilizar la skill `verify-pr-protocol` (Rust) para garantizar la ejecución uniforme de estos chequeos.

## Violaciones
Cualquier intento de eludir este protocolo resultará en:
- Rechazo automático del PR por GitHub Actions.
- Bloqueo de la acción `finalize` por parte del Agente.
- Advertencia en el editor local.
