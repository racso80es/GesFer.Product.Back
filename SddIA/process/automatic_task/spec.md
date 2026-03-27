---
name: Automatic Task
process_id: automatic_task
related_actions:
  - triage
  - activation
  - execution
  - finalization
spec_version: 1.0.0
---
# Proceso: Automatic Task

Este documento define el procedimiento para que una unidad de ejecución SDDIA procese una tarea del backlog de forma autónoma. Asegura la integridad del repositorio y la visibilidad del progreso.

## Fases del Proceso

### 1. Identificación y Triaje (Triage)
Localiza en `docs/TASKS/` el archivo `.md` con la prioridad más alta, el que el usuario indique o el de fecha más antigua.
- Verifica que cumple con un Análisis suficiente para poder realizar la tarea.
- Si la tarea no tiene un ID único (ej. T-26-001), asígnale uno basado en la fecha actual en el nombre del fichero o en su contenido.
- Comprueba que la tarea no está ya en ejecución (no existe en `docs/TASKS/ACTIVE/`).
- **Acción Alternativa (Kaizen):** Si no hay tareas disponibles en `docs/TASKS/`, analiza el proyecto en busca de posibles acciones de mejora continua (Kaizen). Elige una de estas acciones y regístrala como una nueva tarea en un archivo `.md` dentro del directorio `docs/TASKS/` (por ejemplo, `docs/TASKS/Kaizen_YYYY_MM_DD.md`), procediendo a ejecutarla.

### 2. Activación y Bloqueo (Activation)
Transición a estado `ACTIVE` para evitar colisiones con otras IAs (Jules/Cursor).
- Crea una nueva rama `feat/<nombre_feature>` o `fix/<nombre_fix>`.
- Mueve el archivo de la tarea de `docs/TASKS/` a `docs/TASKS/ACTIVE/`.
- **Sincronización Inmediata**: Realiza el primer commit con la reubicación del archivo de la tarea a su nueva ubicación `ACTIVE/` y haz push a origin en la rama actual. Esto bloquea el TODO.

### 3. Ejecución (Execution)
Inicia y continúa las instrucciones definidas en el proceso correspondiente, por defecto el proceso `feature` (definido en `SddIA/process/feature/spec.md`).
- Esto implica generar la documentación de la tarea (objectives, spec, clarify, plan, implementation, execution, validacion) en `paths.featurePath/<nombre_feature>`.

### 4. Finalización y Archivo (Finalization)
Transición a estado `DONE` tras el cumplimiento del proceso.
- Mueve el archivo de la tarea de `docs/TASKS/ACTIVE/` a `docs/TASKS/DONE/`.
- Actualiza `docs/EVOLUTION_LOG.md` con un resumen de la intervención, enlazando al archivo en `DONE/`.
- Genera la documentación de finalización del proceso feature (`finalize.md`).

## Estructura de Carpetas Requerida
Para el correcto funcionamiento de este proceso, el repositorio debe mantener la siguiente jerarquía:
- `docs/TASKS/` -> Tareas pendientes (ficheros individuales).
- `docs/TASKS/ACTIVE/` -> Tareas en ejecución en la rama actual.
- `docs/TASKS/CLARIFY/` -> Tareas en ejecución en la rama actual que necesitan aclaración por parte de usuario.
- `docs/TASKS/DONE/` -> Histórico de éxito.

## Particularidades del proceso
- Trabajar de la forma más autónoma posible, con el fin de obtener la éjecución de la tarea sin supervisión del usuario. En caso de no ser posible este resultado, mover a ruta de documentos a clarificar.
