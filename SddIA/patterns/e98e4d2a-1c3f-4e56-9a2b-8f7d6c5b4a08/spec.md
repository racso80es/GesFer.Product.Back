---
category: Arquitectura de Software
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a08
interested_agents:
- architect
- infrastructure-architect
metadata:
  difficulty: Advanced
  status: Published
tags:
- Lectura/Escritura
- Modelos
- Desacoplamiento
---

# CQRS (Command Query Responsibility Segregation)

CQRS es un patrón arquitectónico avanzado que establece la separación explícita de los modelos de operaciones de escritura o mutación (Commands) de las operaciones exclusivas de lectura (Queries) [27], [28]. Facilita la escalabilidad y optimización independiente de ambos canales, implicando a menudo la creación de proyecciones o modelos de lectura (Read Models) adaptados exactamente al formato que requieren las vistas de los usuarios [29], [30].
