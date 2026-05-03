# Skill: git-branch-manager

**Cápsula:** `paths.skillCapsules.git-branch-manager`.

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `operation` | string | `create` \| `checkout` \| `delete` |
| `branchName` | string | Nombre de rama |
| `startPoint` | string | Solo `create`: base (default `HEAD`) |
| `force` | bool | `checkout`: `git checkout -f`; `delete`: `git branch -D` |
| `workingDirectory` | string | Opcional |

## Binario

`bin/git_branch_manager.exe` (tras `install.ps1`).
