---
category: Seguridad en Código
id: a1b2c3d4-0000-4000-a000-000000000018
interested_agents:
- security-engineer
- tekton-developer
- auditor
metadata:
  difficulty: Intermediate
  status: Published
tags:
- GitHub
- Git
- Spoofing
- Verificación
---

# Modo Vigilante (Vigilant Mode) en GitHub

## Descripción
Relacionado con la firma GPG de Git, las plataformas cloud permiten habilitar el "Modo Vigilante" (Vigilant Mode) en los repositorios [27].

## Mecanismo de Defensa
Esta configuración etiqueta explícitamente y resalta como "Unverified" (No verificado) todos aquellos commits que no posean una firma criptográfica válida o cuya firma provenga de una fuente de dudosa procedencia. Esta barrera visual actúa como escudo protector ante *commits* inyectados por bots o malwares [27].
