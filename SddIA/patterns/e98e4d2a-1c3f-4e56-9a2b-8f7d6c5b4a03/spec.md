---
category: Patrones de Diseño
id: e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a03
interested_agents:
- tekton-developer
- architect
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Acceso a Datos
- Persistencia
- Dominio
---

# Patrón Repository

El patrón Repository centraliza el almacenamiento de datos empleando un lenguaje propio del dominio y de negocio [8], [9]. Actúa conceptualmente como una colección de objetos en memoria, aislando a la aplicación de los detalles técnicos concretos de la persistencia subyacente, ya sea SQL, Redis o memoria [10], [11]. Favorece enormemente la testabilidad del sistema porque permite invertir el control e inyectar implementaciones en memoria durante la ejecución de pruebas unitarias [12], [13].
