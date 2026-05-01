---
action_id: finalize-process
contract_ref: actions-contract.md
flow_steps:
- Precondiciones de cierre (rama, documentación, validación)
- Commits atómicos vía skill registrada
- Evolution Logs (producto y/o SddIA según alcance)
- Publicación y PR vía suite táctica Git (skills)
- Opcional persistencia finalize-process.md en carpeta de tarea
inputs:
- Carpeta de tarea (paths.featurePath o paths.fixPath según Cúmulo)
- Rama feat/ o fix/
outputs:
- Rama en origin
- Evolution Logs actualizados donde aplique
- Pull Request hacia rama de integración
- finalize-process.md opcional (frontmatter YAML + Markdown)
---

# Action: finalize-process

## Propósito

La acción **finalize-process** cierra el **proceso o tarea** documentada en la carpeta canónica (Cúmulo): asegura trazabilidad (commits, logs), **publicación** de la rama en el remoto y **apertura del Pull Request** hacia la rama de integración del repositorio (p. ej. `main`). Solo debe aplicarse cuando la validación ha pasado; si no, el ejecutor debe bloquear o advertir.

**Disparadores conceptuales:** «proceso finalizado», «tarea finalizada», «cierre del ciclo», «publicar rama y abrir PR» en el marco de esta acción.

## Principio

- **No tocar la rama de integración directamente:** el merge es vía PR.
- **Documentación SSOT:** PR y logs enlazan a paths.featurePath/ o paths.fixPath/ según proceso.
- **Orquestación, no ejecución directa:** esta spec **no** referencia scripts `.ps1`/`.bat` ni comandos literales del SO. El ejecutor invoca **únicamente** skills/tools registradas (ver flujo).

## Entradas

- **Carpeta de la tarea:** Ruta desde Cúmulo (ej. paths.featurePath/&lt;nombre&gt;/).
  - Se espera `objectives.md` y, para cierre seguro, `validacion.md` con resultado global coherente con la política del proyecto.
- **Rama:** feat/ o fix/ con cambios listos para consolidar.

## Salidas

- **Rama publicada en `origin`** antes de considerar el PR creado.
- **Evolution Logs** según alcance (producto: paths.evolutionPath; ecosistema SddIA: paths.sddiaEvolutionPath — norma sddia-evolution-sync.md).
- **Pull Request** con cuerpo que enlace la documentación de la tarea.
- **Opcional:** `finalize-process.md` en la carpeta de la tarea (frontmatter: pr_url, branch, timestamp).

## Flujo de orquestación (skills / tools)

El ejecutor sigue este orden lógico invocando las **skills** publicadas en Cúmulo (sin ejecutar git/gh ni shell directamente):

1. **Precondiciones:** Rama distinta de la de integración; existencia de documentación mínima en la carpeta de tarea.
2. **Commits atómicos:** **git-save-snapshot** o **invoke-commit** según convención del hito.
3. **Evaluación de impacto SddIA:** Si hubo mutación bajo `./SddIA/`, cumplir norma de evolución (paths.sddiaEvolutionPath) **antes** de publicar; registrar con **sddia-evolution-register** (binario en paths.skillCapsules.sddia-evolution-register).
4. **Publicación:** **git-sync-remote** con operación `push` (y tracking si aplica).
5. **Pull Request:** **git-create-pr** (`gh` debe estar disponible y autenticado en el entorno del ejecutor).
6. **Emergencia / reversión táctica:** **git-tactical-retreat** solo con confirmación explícita (Ley VISIÓN ZERO).

Opcionalmente, si el proyecto mantiene una skill de verificación pre-PR (p. ej. verify-pr-protocol), debe invocarse **como skill registrada**, no como comando suelto en esta spec.

## Dependencias con otras acciones

- **validate:** Debe haber producido evidencia de calidad antes de un cierre seguro.
- **feature** (y procesos análogos): **finalize-process** es la fase final de cierre documental y publicación.

---
*Definición de la acción finalize-process (cierre de proceso/tarea). Sustituye a la acción histórica `finalize`.*
