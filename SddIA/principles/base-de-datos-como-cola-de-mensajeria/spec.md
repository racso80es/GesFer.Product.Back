---
category: Sistemas Distribuidos
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c89
metadata:
  difficulty: Advanced
  status: Published
principle_id: base-de-datos-como-cola-de-mensajeria
tags:
- Event-Driven
- Message Brokers
- PostgreSQL
- Outbox Pattern
---

# Base de Datos como Cola de Mensajería

**principle_id:** `base-de-datos-como-cola-de-mensajeria`

## Resumen

En arquitecturas más livianas o etapas incipientes de desarrollo donde acoger la robustez de RabbitMQ o Kafka resultaría en sobreingeniería de infraestructura, puede resultar útil tratar a la propia base de datos como sistema de intermediario de mensajes. Dedicando una tabla explícitamente para acumular los eventos de dominio transaccionales, se permite a trabajadores secundarios leerlos paulatinamente y borrarlos, liberando así al caso de uso principal del acoplamiento a las tareas derivadas y aportando resiliencia.

## Objetivo

Desacoplar la ejecución de efectos secundarios (envío de emails, proyecciones, integraciones) del flujo transaccional principal usando la base de datos como cola (patrón Outbox o tabla de eventos), sin introducir aún un bus de mensajería externo.

## Aplicación para Arquitecto

- Diseñar una tabla Outbox o de eventos pendientes, escrita en la misma transacción que el caso de uso; workers o jobs que lean en batch y procesen (o reenvíen a un bus cuando se escale).
- Documentar la garantía de entrega (at-least-once) y la idempotencia de los consumidores; estrategia de borrado o marcado como procesado.

## Aplicación para Tekton

- En el mismo commit transaccional que persiste el agregado, insertar filas en la tabla Outbox con el evento de dominio serializado; el caso de uso no llama directamente a notificaciones ni a proyecciones.
- Implementar un proceso (scheduled job o worker) que lea filas no procesadas, invoque a los handlers y marque o elimine las filas; manejar reintentos y fallos sin perder mensajes.

## Referencias

[49], [50] — Outbox pattern, database as queue, event-driven.

---
*Definición en paths.principlesPath/base-de-datos-como-cola-de-mensajeria/ (contrato paths.principlesPath/principles-contract.md).*
