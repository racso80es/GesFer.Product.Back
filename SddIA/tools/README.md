# Herramientas (SddIA/tools) — Definición vs implementación

Este directorio es **paths.toolsDefinitionPath** (Cúmulo, `SddIA/agents/cumulo.json`). Separa la **capa de definición (SddIA)** de la **capa de implementación (scripts)** para desacoplar la arquitectura IA de los artefactos técnicos.

## Estructura

| Ubicación | Contenido | Propósito |
|-----------|-----------|-----------|
| **SddIA/tools/** (este directorio) | Contrato global: `tools-contract.json`, `tools-contract.md`. Por herramienta: subcarpeta **&lt;tool-id&gt;/** con `spec.md` y `spec.json`. | **Definición:** qué hace la herramienta, contrato, entradas/salidas, fases. Consumido por agentes y procesos. |
| **scripts/tools/** | Índice `index.json`, wrappers .bat y, por herramienta, cápsula **&lt;tool-id&gt;/** (manifest, .ps1, .bat, config, doc, bin/). | **Implementación:** código, configuración y ejecutables. Ruta canónica: **paths.toolCapsules[&lt;tool-id&gt;]** (Cúmulo). |

La **raíz del path de implementación** la indica Cúmulo (**paths.toolsPath**, **paths.toolCapsules**). No se usan rutas literales en la definición.

## Definición por herramienta (SddIA/tools/&lt;tool-id&gt;/)

Cada herramienta debe tener en este directorio una carpeta con:

- **spec.md** — Especificación legible: objetivo, entradas, salidas, fases, cumplimiento del contrato.
- **spec.json** — Especificación machine-readable. Debe incluir **implementation_path_ref**: referencia a la ruta de implementación en Cúmulo (ej. `paths.toolCapsules.<tool-id>`), de modo que la ruta efectiva se resuelva desde `SddIA/agents/cumulo.json` y no se dupliquen rutas.

Ejemplo en **spec.json**:

```json
{
  "toolId": "invoke-mysql-seeds",
  "implementation_path_ref": "paths.toolCapsules.invoke-mysql-seeds",
  "description": "...",
  "contract_ref": "SddIA/tools/tools-contract.json"
}
```

Así, los consumidores (agentes, procesos) leen la definición en SddIA y obtienen la ruta de implementación consultando Cúmulo con la clave indicada en `implementation_path_ref`.

## Referencias

- Contrato global: `tools-contract.json`, `tools-contract.md` (en este directorio).
- Cúmulo: `SddIA/agents/cumulo.json` → **paths.toolsDefinitionPath**, **paths.toolsPath**, **paths.toolCapsules**.
- Índice de implementaciones: **paths.toolsIndexPath** (paths.toolsIndexPath, Cúmulo).
