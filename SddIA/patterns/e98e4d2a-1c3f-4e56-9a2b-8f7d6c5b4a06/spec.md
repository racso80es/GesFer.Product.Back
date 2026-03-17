---
category: Patrones de Diseño
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a06
interested_agents:
- tekton-developer
- architect
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Comportamiento
- Acciones
- Mensajería
---

# Patrón Command

El patrón Command encapsula dentro de un único objeto (usualmente un DTO) todos los datos e información escalar requerida para solicitar que se ejecute una acción en el sistema [24], [25]. Su principal virtud es que permite separar el qué se solicita hacer de quién y cómo se ejecuta realmente, facilitando el transporte seguro de intenciones a lo largo de las fronteras de las capas de software [24], [25].
