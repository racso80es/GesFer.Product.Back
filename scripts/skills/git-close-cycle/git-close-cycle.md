# git-close-cycle

Skill ejecutable: JSON por stdin, `--input-path` o `GESFER_CAPSULE_REQUEST`.

## Ejemplo

```json
{
  "targetBranch": "feat/mi-tarea",
  "workingDirectory": "C:\\Proyectos\\GesFer.Product.Back"
}
```

Opcional: `mainBranch` para fijar la troncal sin detección automática.

## Tekton

Escribir la petición en `.tekton_request.json` en la raíz del repo e invocar:

`pwsh scripts/skills/run-capsule-from-tekton-request.ps1 -Skill git-close-cycle`
