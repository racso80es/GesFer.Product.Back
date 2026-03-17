---
category: Arquitectura de Software
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c83
metadata:
  difficulty: Intermediate
  status: Published
principle_id: manejo-de-errores-tipado-either-result
tags:
- Errores
- Either
- Result
- Programación Funcional
---

# Manejo de Errores Tipado (Either / Result)

**principle_id:** `manejo-de-errores-tipado-either-result`

## Resumen

En un esfuerzo por no encubrir flujos anómalos o emplear excepciones de forma genérica para dirigir el flujo, se pueden promover estructuras funcionales conocidas como Either o Result. La ventaja fundamental de utilizar estos envoltorios es la capacidad de tipar contractualmente las probabilidades de éxito o fallo de forma explícita en las cabeceras de los métodos, forzando con ello al compilador a advertir al desarrollador para que implemente la captura de todas las ramificaciones.

## Objetivo

Hacer que los errores esperables formen parte del contrato del método (tipo de retorno), en lugar de depender de excepciones no declaradas o de códigos de error opacos, mejorando la exhaustividad del manejo de errores.

## Aplicación para Arquitecto

- Decidir en qué capas usar Result/Either (dominio, aplicación) y en qué fronteras traducir a excepciones o códigos HTTP si aplica.
- Definir tipos de error de dominio (ej. ValidationError, NotFoundError) que puedan ser devueltos en el lado izquierdo de Result y documentar su uso en las interfaces.

## Aplicación para Tekton

- En métodos que pueden fallar de forma esperable (validación, búsqueda sin resultado), devolver Result&lt;T, E&gt; o Either&lt;L, R&gt; en lugar de lanzar excepción; el llamador debe manejar ambos casos.
- Evitar usar excepciones para flujo de control; reservar excepciones para fallos realmente excepcionales (errores de infraestructura, invariantes violadas en tiempo de ejecución).

## Referencias

[31], [32] — Either, Result, error handling tipado.

---
*Definición en paths.principlesPath/manejo-de-errores-tipado-either-result/ (contrato paths.principlesPath/principles-contract.md).*
