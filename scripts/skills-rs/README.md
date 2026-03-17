# gesfer-skills (Rust)

Implementación por defecto en Rust de los skills con ejecutable (contrato SddIA/skills/skills-contract.json). Igual que en tools, el ejecutable sugerido para skills ha de ser en Rust.

## Binarios

| Binario | Skill | Cápsula destino |
|---------|--------|-----------------|
| iniciar_rama.exe | iniciar-rama | scripts/skills/iniciar-rama/bin/ |
| merge_to_master_cleanup.exe | finalizar-git (post_pr) | scripts/skills/finalizar-git/bin/ |
| invoke_command.exe | invoke-command | scripts/skills/invoke-command/bin/ |

## Build e instalación

Desde la raíz del repo (o desde scripts/skills-rs):

```powershell
.\scripts\skills-rs\install.ps1
```

- Asegura Rust en PATH (ej. `$env:USERPROFILE\.cargo\bin`).
- Ejecuta `cargo build --release`.
- Copia cada .exe a la cápsula correspondiente en `scripts/skills/<skill-id>/bin/`.

Los launchers .bat en cada cápsula invocan el .exe en bin/ si existe; si no, fallback al script .ps1.

## Uso de los binarios

- **iniciar_rama:** `iniciar_rama.exe feat mi-feature` o `iniciar_rama.exe fix correccion-timeout [master|main]`
- **merge_to_master_cleanup:** `merge_to_master_cleanup.exe [--branch <rama>] [--delete-remote]`
- **invoke_command:** `invoke_command.exe --command "git status" [--fase Accion] [--contexto GesFer]`

Rutas canónicas: Cúmulo paths.skillCapsules (SddIA/agents/cumulo.json).
