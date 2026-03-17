---
category: Principios SOLID
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c77
metadata:
  difficulty: Intermediate
  status: Published
principle_id: dependency-inversion-principle-dip
tags:
- SOLID
- Interfaces
- Polimorfismo
- Inversión de Control
---

# Dependency Inversion Principle (DIP)

**principle_id:** `dependency-inversion-principle-dip`

## Resumen

El Principio de Inversión de Dependencias dicta que el código o lógica de negocio central nunca debe depender en detalle de implementaciones concretas, sino de abstracciones. Basándonos en interfaces alojadas en nuestra capa de dominio, empujamos los detalles de infraestructura o persistencia hacia las capas externas; esto promueve un sistema tolerante a cambios e incrementa notablemente la facilidad para testearlo mediante dobles de prueba.

## Objetivo

Invertir la dependencia: dominio y aplicación dependen de abstracciones (interfaces); la infraestructura implementa esas abstracciones. Así el núcleo no conoce bases de datos, APIs externas ni detalles de framework.

## Aplicación para Arquitecto

- Definir interfaces de puertos (repositorios, envío de mensajes, notificaciones) en la capa de aplicación o dominio; implementaciones en infraestructura.
- Asegurar que ningún paquete de dominio o aplicación referencie proyectos de infraestructura; la dependencia debe ser siempre hacia abstracciones propias del dominio/aplicación.

## Aplicación para Tekton

- Inyectar dependencias mediante constructores o contenedor IoC; programar contra interfaces (IUserRepository, IEmailSender) en servicios de aplicación y dominio.
- En tests, sustituir implementaciones reales por mocks o fakes que implementen las mismas interfaces, sin tocar el código de producción.

## Referencias

[18] — DIP, interfaces en dominio, inyección de dependencias.

---
*Definición en paths.principlesPath/dependency-inversion-principle-dip/ (contrato paths.principlesPath/principles-contract.md).*
