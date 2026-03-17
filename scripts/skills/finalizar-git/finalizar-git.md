# Skill finalizar-git — Cápsula de implementación

**skillId:** finalizar-git  
**Ruta canónica:** Cúmulo paths.skillCapsules["finalizar-git"] (scripts/skills/finalizar-git/)

## Fases

### pre_pr (antes del merge a master)
- **Certificar rama:** `Unificar-Rama.ps1 -BranchName "<rama_actual>" -CommitMessage "chore: finalizar tarea"` (build, documentación, commit).
- **Push y crear PR:** `Push-And-CreatePR.ps1 -Persist "docs/features/<nombre_feature>/"` — hace push y, si está instalado GitHub CLI (`gh`), ejecuta `gh pr create`; si no, muestra la URL para crear el PR manualmente.
- **Validar dependencia gh:** `gh --version` y `gh auth status` (debe estar logueado en github.com con scope `repo`).

### post_pr (tras aceptar el PR en remoto)
- Launcher: `Merge-To-Master-Cleanup.bat` o `Merge-To-Master-Cleanup.ps1 -BranchName "<rama>" -DeleteRemote`
- Si existe `bin/merge_to_master_cleanup.exe` (Rust), el .bat lo invoca; si no, usa el .ps1.
- Checkout a master, pull, eliminar rama local (y opcionalmente remota).

Definición: SddIA/skills/finalizar-git/spec.md y spec.json.
