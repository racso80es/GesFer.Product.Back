# Skill: git-create-pr

**Cápsula:** `paths.skillCapsules.git-create-pr`.

## Prerrequisito

`gh` instalado y autenticado (`gh auth status`).

## Entrada JSON

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `title` | string | Obligatorio |
| `body` | string | Opcional |
| `base` | string | Opcional (`--base`) |
| `head` | string | Opcional (`--head`) |
| `draft` | bool | `--draft` |
| `workingDirectory` | string | Opcional |

## Binario

`bin/git_create_pr.exe`.
