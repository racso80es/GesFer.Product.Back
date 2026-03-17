---
category: DevSecOps
id: a1b2c3d4-0000-4000-a000-000000000010
interested_agents:
- security-engineer
- tekton-developer
- infrastructure-architect
metadata:
  difficulty: Advanced
  status: Published
tags:
- Supply Chain
- Dependencias
- Malware
- npm
---

# Ataques de Supply Chain en Dependencias (npm)

## Descripción
Un vector de ataque que está ganando gran popularidad es el envenenamiento de la cadena de suministro de software.

## Método de Ataque
Los atacantes explotan dependencias transitivas u open source falsificando paquetes (ej. usando nombres de dominios caídos o estafas tipo typosquatting).

## Ejemplo
Un ejemplo notable es la ejecución de malware encubierto usando componentes de node como `child_process` para ejecutar comandos a nivel de sistema e infectar las máquinas de los desarrolladores [15].
