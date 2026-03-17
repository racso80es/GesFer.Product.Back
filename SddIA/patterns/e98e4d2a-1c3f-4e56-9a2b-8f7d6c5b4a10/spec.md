---
category: Sistemas Distribuidos
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a10
interested_agents:
- architect
- infrastructure-architect
- performance-engineer
metadata:
  difficulty: Advanced
  status: Published
tags:
- Resiliencia
- Mensajería
- Transaccionalidad
---

# Outbox Pattern

El Outbox Pattern es una estrategia de resiliencia utilizada en sistemas distribuidos que asegura la publicación fiable de eventos al mundo exterior [35]. Funciona insertando el evento de dominio en una tabla auxiliar de la misma base de datos y dentro de la misma transacción en la que se modifica la entidad principal [35]. Posteriormente, un trabajador asíncrono o sistema externo lee esa tabla y consolida la publicación hacia el broker de mensajería [35].
