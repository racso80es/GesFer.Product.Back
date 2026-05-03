# Skill: git-tactical-retreat

**Cápsula:** `paths.skillCapsules.git-tactical-retreat`.

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `stash` | bool | `git stash push -m stashMessage` |
| `stashMessage` | string | Default `WIP` |
| `hardReset` | bool | `git reset --hard resetTarget` |
| `resetTarget` | string | Default `HEAD` |
| `cleanUntracked` | bool | `git clean -fd` |
| `confirmDestructive` | bool | **Obligatorio `true`** si `hardReset` o `cleanUntracked` (Visión Zero) |
| `workingDirectory` | string | Opcional |

## Binario

`bin/git_tactical_retreat.exe`.
