---
category: Patrones de Diseño
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a07
interested_agents:
- tekton-developer
- architect
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Comportamiento
- Desacoplamiento
- Enrutamiento
---

# Patrón Mediator

El patrón Mediator centraliza la lógica de interacción y rutea la comunicación entre distintos objetos de un sistema para promover un bajo acoplamiento [24]. Frecuentemente se combina de forma directa con el patrón Command para construir un Command Bus, donde el mediador recibe una solicitud de acción y se encarga de redirigirla de manera determinista hacia su único manejador (Handler) asociado [26], [25].
