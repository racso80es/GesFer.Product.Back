---
category: Domain-Driven Design
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c81
metadata:
  difficulty: Advanced
  status: Published
principle_id: eventos-de-dominio-domain-events
tags:
- DDD
- Arquitectura Orientada a Eventos
- Desacoplamiento
---

# Eventos de Dominio (Domain Events)

**principle_id:** `eventos-de-dominio-domain-events`

## Resumen

Los Eventos de Dominio expresan en un formato de datos inmutable que una acción significativa del negocio ya se ha resuelto de manera efectiva en el sistema (por ejemplo, cuando un usuario se ha registrado). Al expedir estos eventos a través de un bus de comunicación, logramos desacoplar el flujo neurálgico de la lógica respecto a cualquier tarea secundaria; esta práctica desbloquea transversalmente el cumplimiento de principios arquitectónicos como SOLID.

## Objetivo

Comunicar hechos ya ocurridos en el dominio sin acoplar quien los produce con quien reacciona (notificaciones, proyecciones, integraciones), permitiendo extensión y evolución independiente.

## Aplicación para Arquitecto

- Definir un catálogo de eventos de dominio (nombre, payload, versión) y el contrato de publicación (in-process, outbox, bus externo).
- Diseñar handlers que reaccionen a eventos para efectos secundarios (envío de email, actualización de proyecciones, auditoría), manteniendo el caso de uso principal libre de esas responsabilidades.

## Aplicación para Tekton

- Tras aplicar un cambio que cumple una regla de negocio, publicar un evento de dominio (ej. UsuarioRegistrado) con los datos mínimos necesarios; no incluir lógica de negocio en el evento, solo el hecho y su contexto.
- Implementar handlers como suscriptores independientes, con idempotencia y manejo de fallos; no bloquear el flujo principal si un handler falla (resiliencia y reintentos según estrategia del proyecto).

## Referencias

[25], [26], [27], [28] — Domain Events, event-driven, desacoplamiento.

---
*Definición en paths.principlesPath/eventos-de-dominio-domain-events/ (contrato paths.principlesPath/principles-contract.md).*
