---
action_id: finalize
contract_ref: actions-contract.md
flow_steps:
- Precondiciones
- Commits atómicos
- Evolution Logs
- Invocar Invoke-Finalize.ps1 (skill finalizar-proceso, pre_pr)
- finalize.json
- Auditoría
- 'post_pr: Finalizar-Proceso.ps1 (skill finalizar-proceso)'
implementation_script_ref: scripts/actions/finalize/Invoke-Finalize.ps1
inputs:
- Carpeta feature (Cúmulo)
- Rama feat/ o fix/
outputs:
- Rama en origin
- Evolution Logs
- PR
- finalize.md opcional (frontmatter YAML + Markdown)
skill_ref: finalizar-proceso
---

# Action: Finalize

## Propósito

La acción **finalize** (finalizar) cierra el ciclo de la feature: asegura commits atómicos en la rama, actualiza los Evolution Logs, **sube la rama al remoto (push)** y crea el Pull Request hacia `master`. Solo debe ejecutarse cuando la validación ha pasado; en caso contrario, debe advertir o bloquear. **Comportamiento obligatorio:** al realizar finalize, el ejecutor debe comprender e incluir el paso de **subir (push)**: publicar la rama actual en `origin` antes de crear el PR; sin este paso el cierre no está completo. Proporciona trazabilidad y cierre formal alineado con las Leyes Universales (no commit en master, documentación en paths.featurePath según Cúmulo).

## Principio

- **No tocar master:** Todo el trabajo permanece en la rama feat/ o fix/; el merge se hace vía PR, no con commit directo en master.
- **Documentación como SSOT:** La descripción del PR y los logs hacen referencia a la carpeta de la feature (ej. paths.featurePath/<nombre_feature>/).
- **Auditoría:** Toda finalización queda registrada en Evolution Logs y, opcionalmente, en auditoría.

## Entradas

- **Carpeta de la feature:** Ruta obtenida de Cúmulo (ej. paths.featurePath/<nombre_feature>/).
  - Se espera que existan al menos: `objectives.md`, y preferiblemente `validacion.md` con resultado global pass.
- **Rama actual:** Rama feat/ o fix/ con todos los cambios ya commiteados (o la acción puede incluir un paso de “commit pendientes” según criterio del proyecto).

## Salidas

- **Rama publicada (subir / push):** La rama actual debe quedar subida en `origin`; es una salida obligatoria de finalize antes de considerar el PR creado.
- **Evolution Logs actualizados:**
  - paths.evolutionPath + paths.evolutionLogFile (raíz docs: docs/EVOLUTION_LOG.md según proyecto): una línea con formato `[YYYY-MM-DD] [feat/<nombre>] [Descripción breve del resultado.] [Estado].`
  - paths.evolutionPath + paths.evolutionLogFile: una sección con fecha, título de la feature, resumen de acción/alcance/resultado y referencia a la carpeta de la feature (Cúmulo)/objectives.md.
- **Pull Request:** Creado hacia `master`, con descripción que enlace a la documentación de la feature (ej. paths.featurePath/<nombre_feature>/).
- **Opcional:** Referencia al PR o estado en validacion.json o finalize.json de la carpeta de la feature (Cúmulo) (ej. URL del PR, timestamp de cierre).

## Skill de referencia: FinalizarProceso (finalizar-proceso)

La acción finalize **utiliza y ejecuta la skill** **FinalizarProceso** (skill_id: `finalizar-proceso`; definición en paths.skillsDefinitionPath/finalizar-proceso/; implementación en paths.skillCapsules[\"finalizar-proceso\"]) para centralizar todas las interacciones con Git. El ejecutor **debe invocar** esta skill para las fases **pre_pr** (push y creación del PR) y, cuando corresponda, **post_pr** (tras aceptar el PR: unificar en main, eliminar rama, volver a main). La skill es la única fuente de verdad para los comandos y flujos git de cierre (Ley COMANDOS: no ejecutar git directamente; toda operación vía skill/herramienta).

### Ejecución de la skill FinalizarProceso (obligatoria)

Para **ejecutar** la acción finalize (pasos de push y PR), se debe invocar el script orquestador que a su vez invoca la skill **finalizar-proceso**:

- **Script orquestador:** `scripts/actions/finalize/Invoke-Finalize.ps1`
- **Desde la raíz del repositorio:**
  ```powershell
  .\scripts\actions\finalize\Invoke-Finalize.ps1 -Persist "docs/features/<nombre_feature>/"
  ```
- **Parámetros:** `-Persist` (obligatorio, ruta de la carpeta de la feature), `-BranchName` (opcional), `-NoVerify` (opcional, omitir verify-pr-protocol), `-Title` (opcional, título del PR).
- **Comportamiento del script:** Comprueba precondiciones (rama no master, objectives.md, validacion.md); opcionalmente ejecuta verify-pr-protocol (Rust); **invoca la skill finalizar-proceso** ejecutando `Push-And-CreatePR.ps1` de la cápsula (paths.skillCapsules[\"finalizar-proceso\"]). El push y la creación del PR los realiza únicamente la skill; el agente no ejecuta comandos git directamente.

## Flujo de ejecución (propuesto)

1. **Comprobación de precondiciones:**
   - Rama actual no es `master`.
   - Existe objectives.md en la carpeta de la feature (Cúmulo).
   - Existe validacion.md en la carpeta de la feature (Cúmulo) y su resultado global es pass (o se permite finalize con advertencia si el proyecto lo define).
2. **Commits atómicos:** Si hay cambios sin commitear, el agente puede agruparlos en commits atómicos según convención (un commit por ítem lógico o por fase).
3. **Ejecutar Protocolo de Aceptación (verify-pr-protocol):**
   - **OBLIGATORIO:** Antes de subir cambios o crear PR, se debe invocar la skill `verify-pr-protocol` (Rust).
   - Comando: `cargo run --bin verify_pr_protocol`.
   - Si la skill falla (exit code != 0), la acción **finalize** debe abortar inmediatamente. El agente debe reportar los errores y no proceder al paso 5.
4. **Actualización de Evolution Logs:**
   - Añadir entrada en docs/EVOLUTION_LOG.md (raíz) o paths.evolutionPath + paths.evolutionLogFile.
   - Añadir sección en paths.evolutionPath + paths.evolutionLogFile con resumen y enlace a la carpeta de la feature.
5. **Ejecutar cierre (push + PR):** **Invocar** `.\scripts\actions\finalize\Invoke-Finalize.ps1 -Persist "docs/features/<nombre_feature>/"` desde la raíz del repo. Este script comprueba precondiciones, opcionalmente ejecuta verify-pr-protocol y ejecuta el cierre vía skills/herramientas autorizadas (Ley COMANDOS). Sin este paso ejecutado con éxito, el cierre no está completo. El agente no ejecuta `git ...` ni `gh ...` directamente.
6. **Persistencia opcional:** Escribir finalize.md en la carpeta de la feature (Cúmulo) con frontmatter YAML (pr_url, branch, timestamp) + cuerpo Markdown.
7. **Auditoría:** Registrar el evento de finalización en paths.auditsPath + paths.accessLogFile (Cúmulo).
8. **Post-PR (skill finalizar-proceso, fase post_pr):** Una vez el PR esté aceptado/mergeado en el remoto, el ejecutor (o el usuario) aplica la fase **post_pr** de la skill **FinalizarProceso** invocando `.\scripts\skills\finalizar-proceso\Finalizar-Proceso.ps1 -BranchName "feat/<nombre_feature>"` (o Finalizar-Proceso.bat). Por defecto se elimina la rama remota; usar `-NoDeleteRemote` para no borrarla. Ver paths.skillsDefinitionPath/finalizar-proceso/spec.md.

## Implementación técnica

La acción finalize **hace uso de la skill finalizar-proceso** (FinalizarProceso) mediante el script orquestador:

- **Ruta del script:** `scripts/actions/finalize/Invoke-Finalize.ps1` (desde la raíz del repo).
- **Invocación mínima:** `.\scripts\actions\finalize\Invoke-Finalize.ps1 -Persist "docs/features/<nombre_feature>/"`
- **Parámetros:** `-Persist` (obligatorio), `-BranchName`, `-NoVerify` (omitir verify-pr-protocol), `-Title`.
- El script invoca internamente **Push-And-CreatePR.ps1** de la cápsula paths.skillCapsules[\"finalizar-proceso\"]; no se ejecutan comandos git fuera de la skill.

## Integración con agentes

- **Tekton Developer (ejecutor del cierre):** Puede ser el responsable de ejecutar finalize: commits finales, actualización de logs, push y apertura del PR, siempre mediante invoke-command para comandos de sistema y git.
- **QA Judge:** Debe haber validado antes (validacion.json pass); si finalize se ejecuta sin validación previa, puede registrarse una advertencia.
- **Cúmulo:** Validan que la documentación de la feature esté en la ruta canónica y que los Evolution Logs referencien correctamente esa ruta (SSOT).

## Agente responsable (referencia para definición de agente)

| Concepto | Descripción |
| :--- | :--- |
| **Id sugerido** | `tekton-developer` (cierre y PR) o un agente dedicado `finalizer` / `release-agent` si se desea separar responsabilidades. |
| **Rol** | Cierre: commits atómicos, actualización de Evolution Logs, push, creación del PR. Respetar Ley GIT y SSOT. |
| **Skills necesarios** | `finalizar-proceso` (FinalizarProceso, obligatorio para pasos Git de cierre), `git-operations`, `documentation`, `invoke-command` (y posiblemente integración con API del repositorio para crear PR). |
| **Restricciones** | Nunca commit en master; toda operación git/comando vía invoke-command; descripción del PR debe enlazar a paths.featurePath/<nombre_feature>/ (Cúmulo).**

Si se desea un agente nuevo para no mezclar “escribir código” con “cerrar y hacer PR”, se puede definir:

- **Finalizer / Release Agent:** Solo se encarga de la fase 8: leer validacion.md, actualizar logs, push y crear PR. Invocado por Tekton o por el orquestador de la acción feature.

## Estándares de calidad

- **Grado S+:** Trazabilidad completa: rama → paths.featurePath → spec/clarify/plan → implementation → execution → validacion → Evolution Logs → PR.
- **Ley GIT:** Ningún commit en master; todo el trabajo en rama feat/ o fix/ con documentación en la carpeta de la feature.
- **Single Source of Truth:** La referencia en PR y en Evolution Log es paths.featurePath/<nombre_feature>/ (Cúmulo).

## Dependencias con otras acciones

- **validate:** Debe haber ejecutado y producido `validacion.json` con pass antes de considerar el cierre seguro.
- **feature:** finalize es la última fase (8) del procedimiento feature; depende de que las fases 0–7 estén completadas.

---
*Documento de definición de la acción Finalize. Corresponde a la fase 8 del procedimiento feature (cierre, logs y PR).*
