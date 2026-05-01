# Skill: git-save-snapshot

**Cápsula:** `paths.skillCapsules.git-save-snapshot`.

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `commitMessage` | string | Obligatorio |
| `stageAll` | bool | Default `true`: `git add -A` |
| `paths` | string[] | Si `stageAll` es false: rutas a `git add` |
| `allowEmpty` | bool | `git commit --allow-empty` |
| `workingDirectory` | string | Opcional |

## Binario

`bin/git_save_snapshot.exe`.
