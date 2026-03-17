# Git hooks (plantillas)

Los hooks en esta carpeta deben **copiarse** a `.git/hooks/` para que Git los ejecute (`.git/` no se versiona).

## pre-commit

- **Origen:** `scripts/git-hooks/pre-commit`
- **Destino:** `.git/hooks/pre-commit`
- **Uso:** Genera el reporte de interacciones (INTERACCIONES_*.json y .md) en `docs/audits/` antes de cada commit. Feature: auditoria-interacciones-entidades.
- **Comportamiento:** No bloquea el commit si falla la generación (solo intenta y sigue).
- **Instalación (PowerShell desde raíz del repo):**
  ```powershell
  Copy-Item scripts/git-hooks/pre-commit .git/hooks/pre-commit
  ```
- **Requisito:** PowerShell 7+ y que `pwsh` esté en PATH (o ajustar el script para usar `powershell.exe -File ...`).
