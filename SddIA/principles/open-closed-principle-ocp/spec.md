---
category: Principios SOLID
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c76
metadata:
  difficulty: Advanced
  status: Published
principle_id: open-closed-principle-ocp
tags:
- SOLID
- Extensibilidad
- Eventos
---

# Open/Closed Principle (OCP)

**principle_id:** `open-closed-principle-ocp`

## Resumen

Este principio cardinal de la programación orientada a objetos expone que el software debe ser diseñado de manera que esté abierto a la extensión de nuevas funcionalidades, pero siempre manteniéndose cerrado a la modificación de la base existente. La integración del OCP cobra un enorme valor mediante el uso de eventos de dominio, ya que permite que agreguemos nuevos efectos colaterales registrando simplemente nuevos suscriptores, omitiendo alterar el flujo del código originario.

## Objetivo

Extender el comportamiento del sistema añadiendo nuevo código (nuevas implementaciones, nuevos manejadores de eventos) en lugar de modificar el código estable ya probado.

## Aplicación para Arquitecto

- Diseñar puntos de extensión mediante interfaces, eventos de dominio o estrategias inyectables; evitar ramas `if (tipo X)` en el núcleo del flujo.
- Priorizar arquitecturas orientadas a eventos para efectos secundarios (notificaciones, reporting, integraciones): nuevos suscriptores sin tocar el publicador.

## Aplicación para Tekton

- Al añadir un nuevo comportamiento "cuando pase X", valorar un evento de dominio y un handler en lugar de editar el caso de uso que origina la acción.
- Implementar nuevas variantes de un algoritmo mediante nuevas clases que implementen una interfaz existente, en lugar de extender con `else if` en el código central.

## Referencias

[16], [17] — OCP, extensión por eventos y estrategias.

---
*Definición en paths.principlesPath/open-closed-principle-ocp/ (contrato paths.principlesPath/principles-contract.md).*
