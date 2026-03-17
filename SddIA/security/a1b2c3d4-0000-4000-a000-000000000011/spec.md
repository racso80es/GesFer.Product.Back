---
category: DevSecOps
id: a1b2c3d4-0000-4000-a000-000000000011
interested_agents:
- security-engineer
- tekton-developer
metadata:
  difficulty: Advanced
  status: Published
tags:
- npm
- postinstall
- Scripts
- Exfiltración
---

# Ejecución de malware vía scripts postinstall

## Descripción
Dentro de los ecosistemas de gestión de paquetes (como npm), existe un gran peligro asociado a los scripts automatizados como `postinstall` [16].

## Riesgo
Si un atacante logra comprometer una librería e inserta código malicioso, este script se ejecutará automática e invisiblemente al hacer un `npm install`.

## Impacto
Este método ha sido utilizado para abrir puertas traseras, extraer variables de entorno (como NPM_TOKEN) y exfiltrarlas hacia un webhook externo controlado por el hacker [16].
