# Skill iniciar-rama — Cápsula de implementación

**skillId:** iniciar-rama  
**Ruta canónica:** Cúmulo paths.skillCapsules["iniciar-rama"] (scripts/skills/iniciar-rama/)

## Uso

Desde la raíz del repositorio:

- **Con launcher .bat** (argumentos posicionales):  
  `.\scripts\skills\iniciar-rama\Iniciar-Rama.bat feat mi-feature`  
  `.\scripts\skills\iniciar-rama\Iniciar-Rama.bat fix correccion-timeout`

- **Con PowerShell** (parámetros nombrados):  
  `.\scripts\skills\iniciar-rama\Iniciar-Rama.ps1 -BranchType feat -BranchName "mi-feature"`  
  `.\scripts\skills\iniciar-rama\Iniciar-Rama.ps1 -BranchType fix -BranchName "correccion-timeout" -SkipPull`

Si existe `bin/iniciar_rama.exe` (Rust), el .bat lo invoca; si no, usa el script .ps1. Definición del skill: SddIA/skills/iniciar-rama/spec.md y spec.json.
