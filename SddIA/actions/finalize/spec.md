---
action_id: finalize
contract_ref: actions-contract.md
flow_steps:
- Precondiciones (rama, documentación, validación)
- Commits pendientes (skill invoke-commit)
- Evolution Logs
- verify-pr-protocol (opcional; Rust)
- Invoke-Finalize.ps1 (skill finalizar-git, fase pre_pr — push + PR)
- Merge del PR en remoto (revisión humana o política del repo)
- Limpieza post-merge (skill finalizar-git, fase post_pr)
implementation_script_ref: scripts/actions/finalize/Invoke-Finalize.ps1
inputs:
- Carpeta feature (Cúmulo)
- Rama feat/ o fix/
outputs:
- Commits en la rama de trabajo
- Rama publicada en origin
- Evolution Logs actualizados
- Pull Request hacia master
- Tras merge — master local alineado y ramas de trabajo eliminadas (post_pr)
- finalize.md opcional (frontmatter YAML + Markdown)
skill_ref: finalizar-git
related_skills:
- invoke-commit
- finalizar-git
---

# Action: Finalize

## Propósito

La acción **finalize** (finalizar) cierra el ciclo de la feature o fix: incorpora **commits pendientes** con trazabilidad, actualiza **Evolution Logs**, **publica la rama** y **abre el Pull Request** hacia `master`; tras la **aprobación y merge** del PR en el remoto, aplica **limpieza** (unificar `master` local, eliminar rama de trabajo). Todo el trabajo con Git debe hacerse **solo vía skills** (Ley COMANDOS / `git-via-skills-or-process.md`): **`invoke-commit`** para commits, **`finalizar-git`** para push, creación de PR y fase **post_pr**.

Solo debe ejecutarse cuando la validación ha pasado; en caso contrario, debe advertir o bloquear.

## Principio

- **No tocar master directamente:** Todo el trabajo permanece en la rama `feat/` o `fix/`; el merge a master es vía **PR** en el remoto (o política equivalente del proveedor Git).
- **Documentación como SSOT:** La descripción del PR y los logs enlazan la carpeta de la tarea (paths.featurePath o paths.fixPath según Cúmulo).
- **Ciclo completo:** «Finalizar proceso» no es solo abrir PR: incluye **(1)** vaciar cambios locales con commit, **(2)** push y PR, **(3)** merge revisado, **(4)** sincronización y limpieza con **finalizar-git** `post_pr`.

## Ciclo de finalización de proceso (orden obligatorio conceptual)

| Fase | Qué cubre | Skill / artefacto |
| :--- | :--- | :--- |
| **A** | Hay cambios sin commitear → agrupar en commits atómicos (Conventional Commits) | **invoke-commit** (paths.skillCapsules.invoke-commit) |
| **B** | Precondiciones: rama ≠ master/main, existencia de documentación de tarea, validacion.md coherente | Acción finalize (comprobación manual o Invoke-Finalize.ps1) |
| **C** | Evolution Logs (docs/EVOLUTION_LOG.md o paths.evolutionPath según Cúmulo) | Edición de documentación |
| **D** | verify-pr-protocol (si el proyecto lo exige) | `cargo run --bin verify_pr_protocol` desde scripts/skills-rs (opcional `-NoVerify` en Invoke-Finalize) |
| **E** | Push de la rama + creación del PR hacia master | **finalizar-git** fase **pre_pr**: `Invoke-Finalize.ps1` → `push_and_create_pr.exe` o Push-And-CreatePR.ps1 en paths.skillCapsules["finalizar-git"] |
| **F** | Merge del PR en el remoto (GitHub/GitLab, etc.) | Humano o política de rama; **no** merge local a master desde la rama de feature sin PR |
| **G** | Tras merge: checkout master, pull, borrar rama local y opcionalmente remota | **finalizar-git** fase **post_pr**: Merge-To-Master-Cleanup.bat / `merge_to_master_cleanup.exe` |

Sin la fase **A**, el push puede quedar incompleto o sin trazabilidad. Sin la fase **G**, el entorno local queda en rama obsoleta.

## Entradas

- **Carpeta de la tarea:** Ruta Cúmulo (ej. paths.featurePath/&lt;nombre_feature&gt;/ o paths.fixPath/&lt;nombre_fix&gt;/).
  - Se espera al menos **objectives.md** y **validacion.md** con resultado coherente con el cierre.
- **Rama actual:** `feat/` o `fix/`; los commits deben estar resueltos antes o durante finalize (fase A).

## Salidas

- **Commits:** Historial en la rama de trabajo con mensajes convencionales.
- **Rama publicada:** `origin` contiene la rama de trabajo (fase E).
- **Evolution Logs** actualizados (formato definido en la acción / proceso).
- **Pull Request** creado o instrucciones/URL si no hay `gh` CLI.
- **Tras merge (F + G):** `master` local actualizado; rama de trabajo eliminada localmente; opcionalmente rama remota eliminada.

## Skills de referencia

### invoke-commit (commits pendientes)

Antes de **Invoke-Finalize.ps1**, si quedan cambios sin commit, el ejecutor debe usar la skill **invoke-commit** (paths.skillsDefinitionPath/invoke-commit/, cápsula paths.skillCapsules.invoke-commit): parámetros `--message`, `--files` o `--all`, tipo/scope convencional. No usar `git commit` directo en shell (Ley COMANDOS).

### finalizar-git (push, PR y limpieza)

La acción finalize **utiliza la skill finalizar-git** (skill_id: `finalizar-git`; definición paths.skillsDefinitionPath/finalizar-git/; implementación paths.skillCapsules["finalizar-git"]):

- **pre_pr:** push + creación de PR (`push_and_create_pr.exe` o Push-And-CreatePR.ps1).
- **post_pr:** tras el merge del PR en remoto — `merge_to_master_cleanup.exe` o Merge-To-Master-Cleanup.bat.

**Nota:** No existe la skill `finalizar-proceso`; el identificador canónico es **finalizar-git**.

### Ejecución del orquestador Invoke-Finalize.ps1 (pre_pr)

Desde la raíz del repositorio:

```powershell
.\scripts\actions\finalize\Invoke-Finalize.ps1 -Persist "docs/features/<nombre_feature>/"
```

Parámetros: `-Persist` (obligatorio), `-BranchName`, `-NoVerify`, `-Title`.

El script comprueba rama, carpeta, opcionalmente verify-pr-protocol e invoca la cápsula **finalizar-git** (no ejecuta git suelto).

### Ejecución post-merge (post_pr)

Cuando el PR esté **mergeado** en el remoto:

```powershell
.\scripts\skills\finalizar-git\Merge-To-Master-Cleanup.bat "<rama_actual>" -DeleteRemote
```

O el ejecutable equivalente con los parámetros documentados en paths.skillsDefinitionPath/finalizar-git/spec.md. Opción `-NoDeleteRemote` si no se debe borrar la rama remota.

## Flujo de ejecución (detallado)

1. **Precondiciones:** Rama no es `master`/`main`; existe carpeta de tarea; validacion.md favorable cuando el proceso lo exija.
2. **Commits pendientes (fase A):** Si `git status` indicara cambios sin commit, invocar **invoke-commit** hasta dejar el árbol limpio (el ejecutor no hace commit directo sin la skill).
3. **Evolution Logs (fase C):** Añadir entradas según convención del proyecto.
4. **verify-pr-protocol (fase D):** Salvo `-NoVerify`, si existe el binario/toolchain, ejecutar verify-pr-protocol; si falla, abortar.
5. **Invoke-Finalize.ps1 (fase E):** Push y PR vía **finalizar-git** pre_pr.
6. **finalize.md opcional** en la carpeta de la tarea con `pr_url`, rama, timestamp.
7. **Merge (fase F):** Revisión y merge del PR en la plataforma (fuera del script salvo CI).
8. **post_pr (fase G):** **finalizar-git** — Merge-To-Master-Cleanup para alinear master local y limpiar ramas.

## Implementación técnica

- **Script:** `scripts/actions/finalize/Invoke-Finalize.ps1`
- **Skill:** paths.skillCapsules["finalizar-git"] (binarios .exe y launchers .bat documentados en SddIA/skills/finalizar-git/spec.md)

## Integración con agentes

- **Tekton Developer / Finalizer:** Ejecuta fases A–E y documenta F–G para el usuario o ejecuta G tras merge.
- **QA Judge:** Valida que validate haya pasado antes del cierre fuerte.
- **Cúmulo:** Rutas canónicas de documentación y Evolution Logs.

## Agente responsable (referencia)

| Concepto | Descripción |
| :--- | :--- |
| **Skills necesarios** | **invoke-commit** (commits), **finalizar-git** (push, PR, post-merge cleanup), opcionalmente **invoke-command** para pasos auditados no cubiertos. |
| **Restricciones** | Nunca commit en master en el cliente sin flujo de PR; operaciones git vía skills. |

## Estándares de calidad

- **Grado S+:** Objetivo → spec → … → validacion → commits → Evolution Logs → PR → merge → post_pr limpio.
- **Ley GIT:** Ningún commit directo a master; merge vía PR.

## Dependencias con otras acciones

- **validate:** Debe preceder al cierre seguro.
- **feature / bug-fix / refactorization:** finalize es la última acción del ciclo documentado en paths.processPath.

---
*Definición de la acción Finalize. Fase 8 del proceso feature (y equivalente en refactorization, etc.).*
