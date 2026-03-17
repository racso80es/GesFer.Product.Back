---
common_workflows:
  start_feature: Iniciar-Rama.bat o .ps1 -BranchType feat -BranchName <slug>
  start_fix: Iniciar-Rama.bat o .ps1 -BranchType fix -BranchName <slug>
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
implementation_path_ref: paths.skillCapsules.iniciar-rama
owner: tekton-developer
parameters:
  BranchName:
    description: Slug del nombre (ej. mi-feature, correccion-timeout)
    required: true
    type: string
  BranchType:
    description: 'Tipo de rama: feat o fix'
    enum:
    - feat
    - fix
    required: true
    type: string
  MainBranch:
    description: Rama troncal; por defecto se detecta
    enum:
    - master
    - main
    required: false
    type: string
  SkipPull:
    default: false
    description: No ejecutar pull en la troncal
    required: false
    type: boolean
phases:
- description: Fetch, checkout a master/main, pull, checkout -b feat/<slug> o fix/<slug>.
  id: create_branch
  name: Crear rama desde troncal actualizada
  steps:
  - Normalizar slug del nombre de rama
  - 'Si rama existe: checkout y merge origin/troncal; si no: fetch, checkout troncal, pull, checkout -b nueva rama'
  - Comprobar git status y git branch -vv
related_agents:
- tekton-developer
- architect
- bug-fix-specialist
related_artefacts:
- paths.processPath/feature/
- paths.processPath/bug-fix/
rules:
- Nunca trabajar en master/main; inicio de acción deja el repo en feat/ o fix/
- La nueva rama se crea desde la troncal actualizada con origin
skill_id: iniciar-rama
spec_version: 1.0.0
status: Active
---

# Skill: Iniciar Rama

**skill_id:** `iniciar-rama`

## Objetivo

Encargarse del **inicio de una acción**: crear una rama nueva adecuada (feat/ o fix/) actualizada con master/main y posicionar el repositorio en ella. Consumible por la fase 0 del proceso **feature**, por el proceso **bug-fix** y por cualquier acción o agente que deba comenzar una tarea en una rama de trabajo. Ley GIT: no trabajar en master; todo el trabajo en feat/ o fix/.

## Alcance

- **Entrada:** Tipo de tarea (feat o fix) y nombre/slug de la rama (ej. mi-feature, correccion-timeout).
- **Salida:** Repositorio en la rama `feat/<slug>` o `fix/<slug>`, creada desde la rama troncal actualizada con origin (master o main).

## Especificación (Spec)

### Entradas

| Entrada | Tipo | Descripción |
| :--- | :--- | :--- |
| BranchType | enum | `feat` (funcionalidad) o `fix` (corrección). |
| BranchName | string | Slug del nombre de la rama (ej. auditoria-scripts, admin-back-login). Se formará feat/BranchName o fix/BranchName. |
| MainBranch | string | (Opcional) Rama troncal: `master` o `main`. Por defecto se detecta automáticamente. |
| SkipPull | bool | (Opcional) Si true, no ejecuta pull en la troncal (útil cuando ya está actualizada). |

### Salidas

| Salida | Descripción |
| :--- | :--- |
| Rama creada | Rama local `feat/<slug>` o `fix/<slug>` creada desde la troncal actualizada. |
| Working tree | Repositorio en esa rama, listo para commits y trabajo. |

### Flujo de ejecución

1. Normalizar el nombre de rama (slug sin espacios ni barras).
2. Si la rama ya existe: checkout a ella y actualizar con origin/master (merge); salir.
3. Si no existe:
   - `git fetch origin`
   - `git checkout master` (o main)
   - `git pull origin master` (o main), salvo SkipPull
   - `git checkout -b feat/<slug>` (o fix/<slug>)
4. Comprobar estado: `git status`, `git branch -vv`.

### Reglas (Ley GIT)

- **Nunca trabajar en master/main:** El inicio de una acción debe dejar el repo en una rama feat/ o fix/.
- **Troncal actualizada:** La nueva rama se crea desde la troncal ya actualizada con origin para evitar desvíos innecesarios.

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:** `scripts/skills/iniciar-rama/bin/iniciar_rama.exe`  
**Fuente Rust:** `scripts/skills-rs/src/iniciar_rama.rs`  

**Estándar:** Solo se generan ejecutables `.exe`. No se deben crear archivos `.ps1`.

### Integración con la cápsula

**Cápsula:** paths.skillCapsules["iniciar-rama"] (Cúmulo).  
**Launcher:** `Iniciar-Rama.bat` en la cápsula; invoca `bin/iniciar_rama.exe`.

Desde la raíz del repo (ejemplo con launcher en cápsula):

```powershell
.\scripts\skills\iniciar-rama\Iniciar-Rama.bat feat mi-feature
```

### Invocación directa del ejecutable

```powershell
& "scripts/skills/iniciar-rama/bin/iniciar_rama.exe" --branch-type feat --branch-name "mi-feature"
```

### Consumidores

- **Proceso feature (fase 0):** Preparar entorno = crear rama feat/<nombre_feature> desde master actualizado. Ver paths.processPath/feature/.
- **Proceso bug-fix:** Crear rama fix/<nombre_fix> desde master. Ver paths.processPath/bug-fix/.
- **Agentes:** Tekton Developer, Arquitecto, Bug Fix Specialist (al iniciar una tarea).

---
*Especificación del skill Iniciar Rama. Definición en paths.skillsDefinitionPath/iniciar-rama/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
