# Normas de interacción: disparadores de usuario

**Alcance:** Comportamiento del agente cuando el usuario escribe ciertos **disparadores** en el mensaje. El dominio **SddIA** define este comportamiento; el agente debe consultar estas normas y aplicarlas.

**Fuente canónica:** Este documento y su equivalente machine-readable `interaction-triggers.json`. Las rutas de skills y tools se resuelven desde **Cúmulo** (`SddIA/agents/cumulo.json`).

---

## Disparador: #Skill

**Cuándo:** El usuario escribe `#Skill` (solo o dentro de un mensaje).

**Comportamiento:**

1. **Reconocer** que el usuario quiere ver o elegir una skill del proyecto.
2. **Sugerir las skills existentes** listando `skill_id` y descripción breve.
3. **Fuente del listado:** paths.skillsDefinitionPath (README.md, tabla de skills) y/o las carpetas en **paths.skillsDefinitionPath** (Cúmulo). Para skills con implementación ejecutable: **paths.skillsIndexPath** o **paths.skillCapsules** (Cúmulo).
4. **Formato de respuesta:** Tabla o lista clara en español; indicar cuáles tienen cápsula (paths.skillCapsules) y cuáles son solo definición.
5. **Cierre:** Ofrecer seguir con una skill concreta: *"¿Con cuál quieres trabajar o qué necesitas hacer?"* Detalle: paths.skillsDefinitionPath/<skill-id>/ (archivo .md con frontmatter YAML); implementación en paths.skillCapsules[skill-id].

### Listado de referencia (actualizar si cambia paths.skillsDefinitionPath)

| skill_id | Descripción | Cápsula |
|----------|-------------|---------|
| invoke-command | Interceptor de comandos de sistema (git, dotnet, npm, pwsh). | paths.skillCapsules.invoke-command |
| invoke-commit | Operaciones de commit con parámetros directos (--message, --files, --all). Sin ficheros .txt. | paths.skillCapsules.invoke-commit |
| git-workspace-recon | Inspección del workspace Git (rama, porcelana, ahead/behind). | paths.skillCapsules.git-workspace-recon |
| git-branch-manager | Crear, checkout o eliminar ramas locales. | paths.skillCapsules.git-branch-manager |
| git-save-snapshot | Stage y commit con mensaje (snapshot rápido). | paths.skillCapsules.git-save-snapshot |
| git-sync-remote | fetch, pull o push contra remoto. | paths.skillCapsules.git-sync-remote |
| git-tactical-retreat | Stash / reset / clean con confirmación destructiva. | paths.skillCapsules.git-tactical-retreat |
| git-create-pr | Crear PR con GitHub CLI (`gh`). | paths.skillCapsules.git-create-pr |
| git-close-cycle | Cierre de ciclo local tras fusión remota (troncal, pull, fetch --prune, borrar rama). | paths.skillCapsules.git-close-cycle |
| git-operations | Uso seguro de Git (ramas feat/fix, commits convencionales). | — |
| documentation | Estándares SSOT y gestión de documentación. | — |
| filesystem-ops | Operaciones de archivo seguras (PowerShell). | — |
| dotnet-development | Estándares .NET (build, test, logging). | — |
| frontend-build | Build Next.js (Product, Admin) fallback. | — |
| security-audit | Auditoría y hooks pre-commit/pre-push. | — |

---

## Disparador: #Action

**Cuándo:** El usuario escribe `#Action` (solo o dentro de un mensaje).

**Comportamiento:**

1. **Reconocer** que el usuario quiere ver o elegir una acción del ciclo de desarrollo.
2. **Sugerir las acciones existentes** listando action_id y descripción breve.
3. **Fuente del listado:** paths.actionsPath (Cúmulo) — cada acción en carpeta paths.actionsPath/<action-id>/ (spec.md). Orden típico en el proceso feature: spec → clarify → planning → implementation → execution → validate → finalize-process.
4. **Formato de respuesta:** Tabla o lista clara en español con action_id y propósito.
5. **Cierre:** Ofrecer seguir con una acción concreta: *"¿Cuál quieres ejecutar o sobre cuál necesitas detalle?"* Detalle: paths.actionsPath/<action-id>/ (archivo .md con frontmatter YAML).

### Listado de referencia (actualizar si cambia paths.actionsPath)

| action_id | Descripción |
|-----------|-------------|
| spec | Especificación: transformar requerimientos en SPEC técnico formal. Entrada del ciclo. |
| clarify | Clarificación: resolver ambigüedades y gaps en SPECs antes de planificación. |
| planning | Plan: convertir spec y clarificaciones en hoja de ruta técnica ejecutable. |
| implementation | Implementación (doc): indicar touchpoints en código y documento de implementación; no modifica código. |
| execution | Ejecución: aplicar al código los cambios del documento de implementación. |
| validate | Validación: comprobar calidad antes del PR (git diff, build, tests, docs); generar validacion.md. |
| finalize-process | Finalizar proceso o tarea: cierre del ciclo (commits, Evolution Logs, push, PR a rama de integración). Suite táctica: git-save-snapshot / invoke-commit, git-sync-remote, git-create-pr; emergencia git-tactical-retreat. |
| sddia-difusion | Difusión de SddIA: mantener .cursor/rules, .github y otros gestores IA alineados con AGENTS y SddIA/norms. |

---

## Disparador: #Process

**Cuándo:** El usuario escribe `#Process` (solo o dentro de un mensaje).

**Comportamiento:**

1. **Reconocer** que el usuario quiere ver o elegir un proceso de tarea.
2. **Sugerir los procesos existentes** listando el identificador y descripción breve.
3. **Fuente del listado:** paths.processPath (Cúmulo: README.md y carpetas por proceso). Cada proceso tiene carpeta &lt;process-id&gt;/ con archivo .md con frontmatter YAML (contrato: process-contract.json).
4. **Formato de respuesta:** Tabla o lista clara en español con process_id y propósito; indicar definición: paths.processPath/&lt;process-id&gt;/.
5. **Cierre:** Ofrecer seguir con un proceso: *"¿Con cuál quieres iniciar una tarea o necesitas detalle?"* Detalle: paths.processPath/README.md y paths.processPath/&lt;process-id&gt;/ (archivo .md con frontmatter YAML).

### Listado de referencia (actualizar si cambia paths.processPath)

| process_id | Descripción | Definición |
|------------|-------------|------------|
| feature | Desarrollo de una funcionalidad: rama feat/&lt;nombre_feature&gt;, documentación en paths.featurePath/&lt;nombre_feature&gt;/ (Cúmulo). | paths.processPath/feature/ |
| bug-fix | Corrección de un bug: rama fix/&lt;nombre_fix&gt;, documentación en paths.fixPath/&lt;nombre_fix&gt;/ (Cúmulo). Alcance mínimo. | paths.processPath/bug-fix/ |
| refactorization | Refactorización: rama feat/refactorization-&lt;nombre_refactor&gt;, documentación en paths.featurePath/refactorization-&lt;nombre_refactor&gt;/ (Cúmulo). | paths.processPath/refactorization/ |
| create-tool | Creación de una nueva herramienta: rama feat/create-tool-&lt;tool-id&gt;, cápsula en paths.toolCapsules, índice y Cúmulo actualizados. | paths.processPath/create-tool/ |
| correccion-auditorias | Corrección de hallazgos de auditoría: rama feat/correccion-segun-auditorias o feat/correccion-auditorias-&lt;id&gt;, documentación en paths.featurePath. Entrada: paths.auditsPath. | paths.processPath/correccion-auditorias/ |
| create-pattern | Creación de patrón de diseño: carpeta en paths.patternsPath con spec.md y spec.json. | paths.processPath/create-pattern.json |
| create-principle | Creación de principio técnico: carpeta en paths.principlesPath con spec.md y spec.json. | paths.processPath/create-principle.json |
| create-template | Creación de plantilla: rama feat/create-template-&lt;template-id&gt;, carpeta en paths.templatesPath. Configuración predefinida de proceso con fin concreto. | paths.processPath/create-template/ |
| audit-tool | Auditoría de herramienta: verificación empírica del funcionamiento de una tool. Resultado: informe en paths.auditsPath/tools/&lt;tool-id&gt;/. | paths.processPath/audit-tool/ |
| create-skill | Creación de skill: rama feat/create-skill-&lt;skill-id&gt;, cápsula en paths.skillCapsules; implementación Rust (.exe). | paths.processPath/create-skill/ |
| validate-pull-requests | Validación integral de PR (S+ Grade): revisión multi-agente sobre la rama origen; informe en paths.featurePath; semillas Kaizen en paths.tasksPath. | paths.processPath/validate-pull-requests/ |

---

## Disparador: subir (acción ejecutable)

**Cuándo:** El usuario escribe **subir**, **subir la rama**, **subir a la nube** o pide explícitamente publicar la rama en el remoto (en el contexto de finalize-process o cierre).

**Comportamiento:**

1. **Reconocer** que el usuario quiere que la rama actual se publique en el remoto (`origin`).
2. **Ejecutar la publicación:** Desde la raíz del repo, ejecutar la skill **git-sync-remote** (push con tracking si aplica). Entorno: Windows (PowerShell).
3. **Comprobar resultado:** Leer la salida del comando. Si hay error (credenciales, red, rama rechazada), informar al usuario con el mensaje de error. Si hay éxito (ej. `branch '...' set up to track 'origin/...'` o `Everything up-to-date`), confirmar que la rama está subida.
4. **No sustituir por documentación:** El agente no debe limitarse a decir que «el paso es subir»; debe **ejecutar** el comando de push y reportar el resultado.

**Relación con finalize-process:** La acción finalize-process (paths.actionsPath/finalize-process/) incluye este paso como obligatorio; cuando el usuario pide «subir» o «finalizar» el proceso (y se aplica el cierre), el agente debe ejecutar el push vía skill git-sync-remote.

---

## Otros disparadores (reservado)

En este documento se podrán añadir más disparadores (p. ej. `#Tool`) con el mismo formato: cuándo, comportamiento, fuente del listado.

---
*El comportamiento del agente ante el usuario lo define el dominio SddIA. Referencia: AGENTS.md (protocolo maestro).*
