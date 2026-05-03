# Skill: git-sync-remote

**Cápsula:** `paths.skillCapsules.git-sync-remote`.

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `operation` | string | `fetch` \| `pull` \| `push` |
| `remote` | string | Default `origin` |
| `branch` | string | Opcional (ref para fetch/pull/push) |
| `workingDirectory` | string | Opcional |

Push sin rama explícita en JSON:

- Si la rama actual **tiene** upstream: `git push <remote> HEAD`.
- Si **no** tiene upstream: `git push -u <remote> HEAD` (crea seguimiento; evita `push` “pelado” que falla o desvía el flujo).

Fallos de Git devuelven JSON con `success: false`, `exitCode` ≠ 0, `message` y `data.error` con el detalle.

## Binario

`bin/git_sync_remote.exe`.
