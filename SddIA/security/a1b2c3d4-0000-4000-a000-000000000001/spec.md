---
category: Ciberseguridad
id: a1b2c3d4-0000-4000-a000-000000000001
interested_agents:
- security-engineer
- architect
- tekton-developer
- auditor
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Seguridad Web
- Base de Datos
- Vulnerabilidades
- SQLi
---

# SQL Injection (Inyección SQL)

## Descripción
La inyección SQL es un ataque clásico y altamente peligroso en aplicaciones web. Ocurre cuando no se sanean correctamente los parámetros introducidos por el usuario (como en la URL o en un formulario) antes de concatenarlos en una consulta a la base de datos [1].

## Impacto
Un usuario malintencionado puede cerrar las comillas de un campo e introducir sentencias adicionales como `OR 1=1`, logrando que la consulta devuelva toda la tabla. Esto puede permitir la extracción de información sensible como contraseñas o comprometer todo el sistema [2].

## Mitigación
*   Utilizar consultas parametrizadas (Prepared Statements).
*   Validar y sanitizar todas las entradas de usuario.
*   Utilizar ORMs que manejen la seguridad por defecto.
