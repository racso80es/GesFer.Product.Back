---
constraints:
- Cola, ACTIVE, DONE, KAIZEN y CLARIFY solo bajo paths.tasksPath (Cúmulo); no mezclar con rutas literales.
- Operaciones git según SddIA/norms/git-via-skills-or-process.md (skills, acciones o proceso).
- Por defecto el ciclo de implementación delega en el proceso feature; documentación de feature en paths.featurePath.
contract_ref: paths.processPath/process-contract.md
default_delegate_process: feature
default_delegate_ref: paths.processPath/feature/
description: Procesa una unidad de tarea del backlog (fichero o carpeta-tarea) con triaje, activación, ejecución y archivo.
name: Automatic Task
paths:
  evolution_ref: paths.evolutionPath y paths.evolutionLogFile (Cúmulo)
  featurePath_ref: paths.featurePath/<nombre_feature> (ciclo feature por defecto)
  tasksPath_ref: paths.tasksPath (Cúmulo)
persist_ref: paths.tasksPath
phases:
- description: Ejecutar git-workspace-recon para validar entorno limpio antes de crear o cambiar de rama.
  id: '0'
  name: Reconocimiento de entorno
- description: Candidatos en raíz de paths.tasksPath y KAIZEN/; prioridad, fecha o indicación del usuario; sin colisión con ACTIVE/.
  id: '1'
  name: Identificación y triaje
- description: Aislar contexto con git-branch-manager (rama feat/ o fix/). Mover unidad a paths.tasksPath/ACTIVE/. Consolidar con git-save-snapshot; push con git-sync-remote según flujo de bloqueo.
  id: '2'
  name: Activación y bloqueo
- description: Ejecutar proceso objetivo (por defecto feature); leer carpeta-tarea si existe spec/plan; generar artefactos en paths.featurePath si aplica. Hitos intermedios con git-save-snapshot; ante fallo estructural, git-tactical-retreat.
  id: '3'
  name: Ejecución
- description: Mover unidad a DONE/; Evolution Log; finalize.md cuando aplique. git-sync-remote; git-create-pr enlazando artefactos de la tarea al cuerpo del Pull Request cuando el cierre lo requiera.
  id: '4'
  name: Finalización y archivo
principles_ref: paths.principlesPath
process_id: automatic_task
process_interface_compliance: >-
  Unidades carpeta-tarea o documentación generada bajo paths.featurePath cumplen process_interface: .md con frontmatter YAML
  (objectives, spec, clarify, plan, implementation, execution, validacion). Sin .json separados en esa ruta. Patrón:
  SddIA/norms/features-documentation-pattern.md.
related_actions:
- triage
- activation
- execution
- finalization
related_skills:
- git-workspace-recon
- git-branch-manager
- git-save-snapshot
- git-sync-remote
- git-tactical-retreat
- git-create-pr
- invoke-command
spec_version: 2.0.0
---

# Proceso: Automatic Task

Este documento define el **proceso de tarea** para que una unidad de ejecución SDDIA procese una tarea del backlog de forma autónoma. Está ubicado en paths.processPath/automatic_task/ (Cúmulo). Asegura la integridad del repositorio y la visibilidad del progreso. Las rutas de cola y estados se obtienen de **Cúmulo** (paths.tasksPath); el ciclo de implementación por defecto usa paths.featurePath y el proceso **feature** (paths.processPath/feature/).

**Interfaz de proceso:** Las unidades **carpeta-tarea** bajo paths.tasksPath y la documentación generada al ejecutar el proceso **feature** cumplen la interfaz en Cúmulo (`process_interface`): artefactos **`.md`** con frontmatter YAML donde aplique el ciclo estándar. Patrón: SddIA/norms/features-documentation-pattern.md.

**Rutas de carpetas:** usar siempre la ruta de tareas del Cúmulo (`paths.tasksPath`), no literales fijos en documentación nueva. En disco, la carpeta puede coincidir con `docs/tasks/` o `docs/TASKS/` según el sistema de archivos; resolver siempre la ruta del contrato (paths.tasksPath).

## Unidad de tarea

Una **unidad de tarea** es lo que se selecciona, activa, ejecuta y archiva como un solo bloque. Puede ser:

- **(A) Fichero suelto:** un único `.md` en la **raíz** de `paths.tasksPath` (no dentro de subcarpetas de primer nivel, salvo las reservadas).
- **(B) Carpeta-tarea:** un subdirectorio de primer nivel bajo `paths.tasksPath` cuyo nombre siga convención legible (p. ej. kebab-case: `s-plus-pr54-mycompany/`), que agrupe uno o varios `.md` del ciclo SDdIA (objectives, spec, plan, implementation, validacion, finalize, etc.) según `SddIA/norms/features-documentation-pattern.md`. Puede incluir `README.md` como índice opcional.

**Carpetas reservadas** (no son unidades de tarea en cola; excluir del triaje en §1.1): `ACTIVE/`, `DONE/`, `CLARIFY/`, `KAIZEN/`. No deben mezclarse tareas sueltas con el mismo nombre que una carpeta reservada.

## Fases del Proceso

### 1. Identificación y Triaje (Triage)

**1.1 Bandeja principal (tareas no Kaizen en cola)**  
Construye la lista de candidatos en la **raíz** de `paths.tasksPath`:

1. Todos los archivos `*.md` **directamente** en la raíz (ficheros sueltos).
2. Todos los **subdirectorios de primer nivel** que no sean carpetas reservadas y que contengan al menos un `*.md` (carpetas-tarea). Criterio opcional de validez: presencia de `spec.md` o `objectives.md` si se quiere excluir carpetas vacías o auxiliares.

Entre los candidatos (ficheros y carpetas tratados con igual categoría de “tarea pendiente”), elige el de prioridad más alta, el que el usuario indique o el de **fecha más antigua**:

- Preferir prefijo de fecha en el **nombre del fichero** o de la **carpeta** (`YYYYMMDD`, `YYYY_MM_DD`, etc.).
- Alternativa o desempate: campos `date` / `created` en el frontmatter del `spec.md` o `objectives.md` dentro de una carpeta-tarea, o del propio `.md` suelto.

Si el **usuario indica** una ruta concreta (archivo `.md` o carpeta bajo `paths.tasksPath`), esa selección prevalece sobre el orden automático.

- Verifica que cumple con un análisis suficiente para poder realizar la tarea.
- Si la tarea no tiene un ID único (ej. T-26-001), asígnale uno basado en la fecha actual en el nombre del fichero, nombre de la carpeta o en su contenido.
- Comprueba que la tarea no está ya en ejecución (no existe una copia homónima en `paths.tasksPath/ACTIVE/` en ninguna rama activa ni master).

**1.2 Cola Kaizen (solo si 1.1 no devuelve ninguna tarea)**  
Si **no** hay ningún candidato pendiente según §1.1, revisa **`paths.tasksPath/KAIZEN/`**.

- Enumera tanto **ficheros `.md` sueltos** en `KAIZEN/` como **subcarpetas** bajo `KAIZEN/` que contengan al menos un `.md` (misma noción de unidad de tarea que en §1.1, pero acotada a Kaizen).
- Si hay uno o más candidatos, selecciona **el más antiguo** (criterio preferente: prefijo de fecha en el nombre, p. ej. `Kaizen_YYYY_MM_DD_*.md` o carpeta `Kaizen_YYYY_MM_DD_<slug>/`; alternativa: campo `created` / fecha en frontmatter).
- Esa tarea se ejecuta con el **mismo procedimiento** que una tarea normal (activación, ejecución, finalización; ver §2–4).

**1.3 Nueva Kaizen (solo si 1.1 y 1.2 no ofrecen trabajo)**  
- Si no hay tareas en la bandeja principal **ni** en `paths.tasksPath/KAIZEN/`, analiza el proyecto en busca de acciones de mejora continua (Kaizen), elige una y **regístrala** como nuevo fichero `.md` en `paths.tasksPath/KAIZEN/` (convención recomendada: `Kaizen_YYYY_MM_DD_<slug>.md`) o, si el equipo usa paquetes, como carpeta bajo `KAIZEN/` con el mismo prefijo de fecha; procédela igual que en §2–4.
- Comprueba que el Kaizen no está ya en ejecución (no existe en `paths.tasksPath/ACTIVE/` en ninguna rama activa ni master).

### 2. Activación y Bloqueo (Activation)

Transición a estado `ACTIVE` para evitar colisiones con otras IAs (Jules/Cursor). Tras **git-workspace-recon** (fase 0), usar **git-branch-manager** para `feat/<nombre_feature>` o `fix/<nombre_fix>`.

- Crea o selecciona la rama de trabajo según el contrato de skills (no iniciar-rama legacy).
- Mueve la **unidad de tarea** desde su origen (raíz de `paths.tasksPath` o `paths.tasksPath/KAIZEN/`) hacia `paths.tasksPath/ACTIVE/`:
  - Si es un **fichero suelto:** mueve solo ese `.md` a `ACTIVE/`.
  - Si es una **carpeta-tarea:** mueve **toda la carpeta** a `ACTIVE/<mismo-nombre>/` sin alterar su contenido interno.
- **Sincronización inmediata:** **git-save-snapshot** con la reubicación; **git-sync-remote** (push) en la rama actual. Esto bloquea el TODO.

### 3. Ejecución (Execution)

Inicia y continúa las instrucciones definidas en el proceso correspondiente, por defecto el proceso `feature` (definido en `SddIA/process/feature/spec.md`).

- Si la unidad activa es una **carpeta-tarea** que ya contiene `spec.md`, `plan.md`, `implementation.md`, etc., **lee y sigue** esa documentación como fuente principal antes de duplicar trabajo; genera o actualiza artefactos solo donde falten.
- Si la tarea es un **fichero único** o no existe aún paquete completo en `paths.featurePath`, aplica el proceso feature estándar: generar la documentación (objectives, spec, clarify, plan, implementation, execution, validacion) en `paths.featurePath/<nombre_feature>` cuando corresponda.

### 4. Finalización y Archivo (Finalization)

Transición a estado `DONE` tras el cumplimiento del proceso.

- Mueve la unidad de tarea desde `paths.tasksPath/ACTIVE/` a `paths.tasksPath/DONE/` (mismo criterio: un solo `.md` o **carpeta completa**).
- Actualiza el log de evolución del producto (`paths.evolutionPath` / `paths.evolutionLogFile` según Cúmulo) con un resumen de la intervención, enlazando al archivo o a la carpeta en `DONE/`.
- **git-sync-remote** y **git-create-pr** cuando el cierre requiera PR, inyectando resumen de artefactos de la tarea en el cuerpo del Pull Request.
- Genera la documentación de finalización del proceso feature (`finalize.md`) cuando aplique.

## Estructura de carpetas requerida

Para el correcto funcionamiento de este proceso, el repositorio debe mantener la siguiente jerarquía bajo `paths.tasksPath`:

- Raíz de `paths.tasksPath` → Tareas pendientes: **ficheros `.md` individuales** y/o **subcarpetas-tarea** (cada una con uno o varios `.md` del ciclo SDdIA).
- `paths.tasksPath/KAIZEN/` → Cola Kaizen: mismas formas (sueltos o subcarpetas).
- `paths.tasksPath/ACTIVE/` → Tareas en ejecución (fichero suelto o carpeta bajo `ACTIVE/`).
- `paths.tasksPath/CLARIFY/` → Tareas en ejecución que necesitan aclaración por parte del usuario.
- `paths.tasksPath/DONE/` → Histórico de éxito (archivos y carpetas archivadas).

## Particularidades del proceso

- Trabajar de la forma más autónoma posible, con el fin de obtener la ejecución de la tarea sin supervisión del usuario. En caso de no ser posible este resultado, mover a la ruta de documentos a clarificar.

## Historial de versión del spec

- **2.0.0:** Suite táctica Git (git-workspace-recon, git-branch-manager, git-save-snapshot, git-sync-remote, git-tactical-retreat, git-create-pr); fase 0 de reconocimiento; cierre con sync/PR; obsolescencia iniciar-rama/finalizar-git en este spec.
- **1.3.1:** Frontmatter alineado con process-contract (contract_ref, principles_ref, persist_ref, phases, paths, process_interface_compliance, related_skills); introducción estándar con ubicación e interfaz de proceso.
- **1.3.0:** Soporte explícito de **carpetas-tarea** además de ficheros `.md` sueltos; activación y archivo mueven carpeta completa; triaje unificado y carpetas reservadas nombradas.
- **1.2.0:** Versión anterior (solo ficheros sueltos en la raíz para la bandeja principal).
