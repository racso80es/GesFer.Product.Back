# invoke-commit

**skillId:** invoke-commit  
**Cápsula:** paths.skillCapsules.invoke-commit (Cúmulo)

## Objetivo

Ejecutar **git add + git commit** con parámetros directos, sin generar ficheros `.txt` intermedios. Consumible por acciones, procesos y flujos SddIA. Sustituye a invoke-command para operaciones de commit.

## Uso

Desde la raíz del repositorio:

```powershell
# Commit con archivos específicos (separados por coma)
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "descripción" --files "a.md,b.json"

# Commit con todos los cambios
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "descripción" --all

# Conventional Commits (tipo y scope)
.\scripts\skills\invoke-commit\Invoke-Commit.bat --message "detectar MySQL no disponible" --files "spec.md" --type feat --scope start-api
```

## Parámetros

| Parámetro | Obligatorio | Descripción |
|-----------|-------------|-------------|
| --message, -m | Sí | Mensaje del commit |
| --files | Condicional | Rutas separadas por coma (ej. "a.md,b.json") |
| --all, -a | Condicional | git add -A (excluyente con --files) |
| --type | No | feat, fix, chore, docs, refactor (default: feat) |
| --scope | No | Scope Conventional Commits |
| --fase | No | Fase para telemetría (default: Accion) |
| --contexto | No | Contexto para registro (default: GesFer) |

## Salida

- exitCode 0 si commit exitoso
- Registro en docs/diagnostics/{branch}/execution_history.json

## Implementación

Ejecutable Rust en `bin/invoke_commit.exe`. Fuente: scripts/skills-rs/src/bin/invoke_commit.rs.
