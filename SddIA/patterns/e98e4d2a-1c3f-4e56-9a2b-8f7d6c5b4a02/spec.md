---
category: Arquitectura de Software
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a02
interested_agents:
- architect
- infrastructure-architect
metadata:
  difficulty: Advanced
  status: Published
tags:
- ACL
- Integración
- Legacy
---

# Anti-Corruption Layer (Capa Anticorrupción)

La Capa Anticorrupción (ACL) actúa como una barrera traductora entre el dominio puro y la infraestructura o sistemas legacy [5], [6]. Su objetivo principal es evitar que las estructuras de las bases de datos antiguas o las respuestas de APIs externas contaminen la lógica de negocio de la nueva aplicación [7]. Al usar este patrón, las capas internas interactúan solo con la ACL, la cual se encarga internamente de transformar los datos primitivos y mapearlos a entidades válidas del dominio [6], [7].
