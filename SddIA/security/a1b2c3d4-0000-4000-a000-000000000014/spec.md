---
category: Arquitectura de Software
id: a1b2c3d4-0000-4000-a000-000000000014
interested_agents:
- architect
- infrastructure-architect
- tekton-developer
- security-engineer
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Gestión de Secretos
- Bitwarden
- Vault
- Seguridad
---

# Gestión de Secretos con Vaults

## Descripción
Para evitar el problema de almacenar contraseñas en texto plano o ficheros `.env`, la mejor práctica de la industria dicta el uso de gestores de contraseñas y herramientas tipo Vault (HashiCorp Vault, Bitwarden, 1Password, etc.) [20, 21].

## Implementación
Estas plataformas inyectan las credenciales necesarias dinámicamente en tiempo de ejecución de la aplicación o en la pipeline de despliegue, asegurando que las claves del entorno de producción nunca lleguen a rozar la base de código ni los equipos de los desarrolladores [21].
