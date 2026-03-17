---
category: Seguridad de Aplicaciones
id: a1b2c3d4-0000-4000-a000-000000000003
interested_agents:
- architect
- security-engineer
- tekton-developer
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Autenticación
- JWT
- Tokens
- Identidad
---

# JSON Web Tokens (JWT) y su Integridad

## Descripción
Los JWT permiten la comunicación de información entre cliente y servidor sin necesidad de guardar el estado de la sesión en base de datos. El token contiene un *payload* con datos (como el ID del usuario).

## Riesgo de Integridad
El principal riesgo es que un usuario intercepte el token y modifique su contenido (ej. cambiando su ID para suplantar a otro).

## Mecanismo de Seguridad
El servidor firma el JWT en su creación utilizando un secreto o clave privada; si el *payload* es manipulado, el servidor detectará que la firma criptográfica ya no es válida y rechazará la petición [5, 6].
