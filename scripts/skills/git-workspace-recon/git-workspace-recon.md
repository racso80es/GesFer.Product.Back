# Skill: git-workspace-recon

**Cápsula:** `paths.skillCapsules.git-workspace-recon` (Cúmulo).

## Uso

Entrada JSON (`stdin`, `--input-path` o `GESFER_CAPSULE_REQUEST`):

- `skillId` (opcional): `git-workspace-recon`
- `workingDirectory` (opcional): repo Git; por defecto `GESFER_REPO_ROOT` o directorio actual.
- `includeRawPorcelain` (opcional, default `true`): incluir salida completa de `git status --porcelain`.

Ejemplo:

```json
{"skillId":"git-workspace-recon","workingDirectory":"."}
```

Salida: JSON según `SddIA/skills/skills-contract.md` (`skillId`, `exitCode`, `success`, `data` con `gitTopLevel`, `branch`, `headSha`, `ahead`, `behind`, etc.).

## Binario

Tras `scripts/skills-rs/install.ps1`: `bin/git_workspace_recon.exe`.
