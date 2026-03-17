---
category: DevSecOps
id: a1b2c3d4-0000-4000-a000-000000000012
interested_agents:
- security-engineer
- tekton-developer
- infrastructure-architect
metadata:
  difficulty: Advanced
  status: Published
tags:
- CI/CD
- GitHub Actions
- Bash Injection
- Vulnerabilidades
---

# Bash Injection en GitHub Actions

## Descripción
Los pipelines de CI/CD pueden ser gravemente comprometidos si no se audita su configuración.

## Vector de Ataque
Un vector conocido de ataque implica aprovechar el evento de trigger `pull_request_target` en GitHub Actions [17]. Este evento lanza la acción con permisos elevados (permitiendo el acceso a los secretos de GitHub y variables de entorno reales).

## Consecuencia
Si se combina con una validación laxa, un atacante puede abrir una Pull Request que ejecute un *Bash Injection*, robando tokens de infraestructura o alterando repositorios enteros [16, 17].
