# Skill: git-sync-remote

**Cápsula:** `paths.skillCapsules.git-sync-remote`.

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `operation` | string | `fetch` \| `pull` \| `push` |
| `remote` | string | Default `origin` |
| `branch` | string | Opcional (ref para fetch/pull/push) |
| `workingDirectory` | string | Opcional |

Push sin rama: `git push <remote> HEAD`.

## Binario

`bin/git_sync_remote.exe`.
