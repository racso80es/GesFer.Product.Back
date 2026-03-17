---
category: Patrones de Diseño
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a04
interested_agents:
- tekton-developer
- architect
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Acceso a Datos
- Bases de Datos
- CRUD
---

# Data Access Object (DAO)

El patrón DAO (Data Access Object) se enfoca en abstraer y encapsular el acceso directo a la estructura de datos, mapeando operaciones CRUD a las tablas de la base de datos [14], [15]. A diferencia del patrón Repository, el DAO está fuertemente acoplado y guiado por el esquema de persistencia físico, sin modelar necesariamente reglas de negocio complejas ni comportamientos ricos del dominio [16], [17].
