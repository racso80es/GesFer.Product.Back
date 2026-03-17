---
category: Domain-Driven Design
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c79
metadata:
  difficulty: Intermediate
  status: Published
principle_id: value-objects
tags:
- DDD
- Inmutabilidad
- Validación
- Diseño de Tipos
---

# Value Objects

**principle_id:** `value-objects`

## Resumen

Los Value Objects u objetos de valor son componentes conceptuales inmutables y sin identificador cuyo propósito es encapsular datos primitivos junto con las lógicas asociadas a ellos. Su rasgo más valioso es que albergan sus propias restricciones de validación dentro del propio constructor, asegurando invariablemente la integridad de negocio: si el programa es capaz de hacer una instancia de ese Value Object, entonces el valor es veraz y seguro.

## Objetivo

Elevar primitivos (strings, números) a tipos de dominio con significado y reglas de validez, eliminando valores inválidos en todo el flujo y centralizando la validación en un único lugar.

## Aplicación para Arquitecto

- Definir Value Objects para conceptos de negocio que hoy son primitivos (Email, NIF, Importe, FechaConZona, etc.) y documentar sus invariantes.
- Ubicar los Value Objects en la capa de dominio y asegurar que no dependan de infraestructura; la validación debe ser pura y basada en reglas de negocio.

## Aplicación para Tekton

- Crear tipos inmutables que encapsulen el valor y validen en el constructor; no permitir setters ni mutación. En C#, usar structs o clases con init-only o readonly.
- Rechazar en el constructor cualquier valor que no cumpla las reglas (ej. email vacío, importe negativo); lanzar excepción de dominio o devolver Result/Either en lugar de permitir estado inválido.

## Referencias

[21], [22] — DDD, Value Objects, validación en constructor.

---
*Definición en paths.principlesPath/value-objects/ (contrato paths.principlesPath/principles-contract.md).*
