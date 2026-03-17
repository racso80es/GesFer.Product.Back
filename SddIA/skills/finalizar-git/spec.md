---
common_workflows:
  after_pr_merged: Invocar Merge-To-Master-Cleanup (cápsula) -BranchName <branch> -DeleteRemote; o comandos git equivalentes
  push_and_open_pr: Push-And-CreatePR.ps1 -Persist paths.featurePath/<nombre_feature>/ (push + gh pr create o URL manual)
contract_ref: paths.skillsDefinitionPath + skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.finalizar-git
owner: tekton-developer
phases:
- description: Push de la rama (Push-And-CreatePR.ps1) y creación del PR con gh pr create si está disponible; si no, URL/instrucciones.
  id: pre_pr
  name: Pre-PR
- description: 'Tras merge del PR en remoto: actualizar master local, eliminar rama, volver a master.'
  id: post_pr
  name: Post-PR
related_agents:
- tekton-developer
- finalizer
- release-agent
related_artefacts:
- paths.actionsPath/finalize/
rules:
- Nunca commit directo en master; todo en feat/ o fix/ y merge vía PR
- 'Mensajes de commit: Conventional Commits (feat:, fix:, chore:)'
- 'Pre-push: validación (validacion.json pass) cuando lo exija el proceso'
skill_id: finalizar-git
spec_version: 1.0.0
status: Active
---

# Skill: Finalizar Git

**skill_id:** `finalizar-git`

## Objetivo

Centralizar todas las interacciones con Git necesarias para el cierre de una feature o fix: desde la rama de trabajo hasta **aceptar el PR en master**, **unificar** la rama, **eliminar la rama unificada** (local y opcionalmente remota) y **volver a master** con el repositorio limpio. Esta skill es consumible por la acción **finalize** (paths.actionsPath/finalize/, Cúmulo) y por agentes que ejecuten el cierre del ciclo.

## Alcance

- **Pre-PR (rama de trabajo):** Push de la rama, creación del PR hacia master (la acción finalize orquesta esto; la skill puede describir los comandos de apoyo).
- **Post-PR (una vez el PR está aceptado/mergeado):** Checkout a master, pull de master, eliminación de la rama local ya fusionada, eliminación opcional de la rama remota, y comprobación del estado final.

## Especificación (Spec)

### Entradas

| Entrada | Tipo | Descripción |
| :--- | :--- | :--- |
| Rama actual | string | Nombre de la rama de trabajo (ej. `feat/nombre-feature` o `fix/nombre-fix`). |
| Persist | string | (Opcional) Ruta de la carpeta de la feature/fix (ej. paths.featurePath/<nombre_feature>/) para trazabilidad. |
| Fase | enum | `pre_pr` (push y preparación de PR) o `post_pr` (post-merge: unificar local, limpiar, volver a master). |

### Salidas

| Salida | Descripción |
| :--- | :--- |
| Rama publicada | En fase `pre_pr`: rama pusheada a origin. |
| PR creado / referencia | En fase `pre_pr`: PR hacia master (o instrucciones para crearlo). |
| Master actualizado | En fase `post_pr`: repositorio local en `master` con los cambios mergeados. |
| Ramas limpias | En fase `post_pr`: rama local eliminada; opcionalmente rama remota eliminada. |

### Flujo de ejecución

#### Fase `pre_pr` (antes del merge a master)

1. Comprobar que la rama actual no es `master`.
2. Comprobar estado: cambios sin commitear → commit atómico con mensaje convencional.
3. **Push y creación del PR:** ejecutar el componente `bin/push_and_create_pr.exe` de la cápsula:
   - Hace `git push origin <rama_actual>`.
   - Si **GitHub CLI (gh)** está instalado y autenticado, ejecuta `gh pr create --base master|main --head <rama> --title <título> --body "Documentación: <ruta Cúmulo>"`.
   - Si no hay `gh`, muestra la URL para crear el PR manualmente (GitHub) o instrucciones para otro proveedor.
4. La descripción del PR debe enlazar a la ruta de documentación (Cúmulo) si se pasa el parámetro correspondiente.

#### Fase `post_pr` (después de aceptar/mergear el PR en el remoto)

1. Asegurarse de que el PR ya está mergeado en `master` (o `main`) en el remoto.
2. **Implementación:** Cápsula paths.skillCapsules["finalizar-git"]: ejecutable `bin/merge_to_master_cleanup.exe`. Launcher: `Merge-To-Master-Cleanup.bat`.

   Desde la raíz del repo:
   ```powershell
   .\scripts\skills\finalizar-git\Merge-To-Master-Cleanup.bat "<rama_actual>" -DeleteRemote
   ```

3. **Alternativa manual:** Si el script no se usa, ejecutar en orden: `git checkout master` (o `main`), `git pull origin master`, `git branch -d <rama_actual>`, opcionalmente `git push origin --delete <rama_actual>`, y comprobar con `git status` y `git branch -vv`.

### Reglas (Ley GIT)

- **Nunca commit directo en master:** Todo el trabajo se hace en rama feat/ o fix/; el merge a master se hace vía PR en el remoto (o, en flujo local, vía `git merge` desde master después de certificar).
- **Mensajes convencionales:** Los commits deben seguir Conventional Commits (ej. `feat:`, `fix:`, `chore:`).
- **Pre-push:** Ejecutar validación local (build/tests) antes de push cuando lo exija el proceso (validacion.json pass antes de finalize).

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:**
- `scripts/skills/finalizar-git/bin/merge_to_master_cleanup.exe`
- `scripts/skills/finalizar-git/bin/push_and_create_pr.exe`

**Fuente Rust:** `scripts/skills-rs/src/finalizar_git/`

**Estándar:** Solo se generan ejecutables `.exe`. No se deben crear archivos `.ps1`.

### Integración con la cápsula

**Cápsula:** paths.skillCapsules["finalizar-git"] (Cúmulo).

| Fase    | Componente | Ubicación | Uso |
|--------|------------|-----------|-----|
| **post_pr** | merge_to_master_cleanup.exe | `bin/merge_to_master_cleanup.exe` | Tras aceptar el PR: posicionar en master/main, sincronizar y eliminar la rama mergeada (local y opcionalmente remota). |
| **pre_pr**  | push_and_create_pr.exe | `bin/push_and_create_pr.exe` | Push de la rama y **crear el PR** (GitHub CLI `gh pr create` si está disponible; si no, URL/instrucciones). |

### Invocación

**Post-PR (merge y cleanup):**
```powershell
# Mediante launcher
.\scripts\skills\finalizar-git\Merge-To-Master-Cleanup.bat "<rama_actual>" -DeleteRemote

# Invocación directa
& "scripts/skills/finalizar-git/bin/merge_to_master_cleanup.exe" --branch-name "<rama_actual>" --delete-remote
```

**Pre-PR (push y crear PR):**
```powershell
# Mediante launcher
.\scripts\skills\finalizar-git\Push-And-CreatePR.bat "<rama_actual>" -Persist "<ruta_docs>"

# Invocación directa
& "scripts/skills/finalizar-git/bin/push_and_create_pr.exe" --branch-name "<rama_actual>" --persist "<ruta_docs>"
& "scripts/skills/finalizar-git/bin/push_and_create_pr.exe" --branch-name "<rama>" --body "Descripción del PR"
& "scripts/skills/finalizar-git/bin/push_and_create_pr.exe" --branch-name "<rama>" --body-file "docs/features/X/pr_body.md"
```

**Parámetros de body del PR (precedencia: body-file > body > persist > rama):**
- **--body:** Descripción directa del PR (evita fichero .txt).
- **--body-file:** Ruta a fichero con el body (para cuerpos largos; evita límites de línea de comandos).

### Dependencia opcional: GitHub CLI (gh)

Para que **Push-And-CreatePR.ps1** cree el PR automáticamente, debe estar instalado y autenticado **GitHub CLI** (`gh`). Validación:

- `gh --version` — comprobar que está instalado.
- `gh auth status` — debe mostrar sesión activa en github.com con scope `repo`.

Si `gh` no está disponible, el script muestra la URL para crear el PR manualmente.

### Consumidores

- **Acción finalize:** Usa esta skill para los pasos de Git (push, PR y, si aplica, pasos post-pr).
- **Agentes:** Tekton Developer, Finalizer / Release Agent (según paths.actionsPath/finalize/).

---
*Especificación del skill Finalizar Git. Definición en paths.skillsDefinitionPath/finalizar-git/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
