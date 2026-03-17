---
category: Domain-Driven Design
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c80
metadata:
  difficulty: Advanced
  status: Published
principle_id: aggregate-roots
tags:
- DDD
- Agregados
- Transacciones
- Integridad
---

# Aggregate Roots

**principle_id:** `aggregate-roots`

## Resumen

Un Agregado es un límite conceptual que engloba un conjunto de Entidades y Value Objects a los que se trata de manera unificada a efectos de cambios y transaccionalidad. A la cabecera de esta estructura se le nombra Aggregate Root, figurando como el punto de entrada exclusivo desde el cual cualquier clase exterior puede interactuar con el estado interno del agregado, lo que sirve para salvaguardar invariantes de negocio con éxito.

## Objetivo

Garantizar la consistencia de un conjunto de objetos de dominio dentro de una transacción, exponiendo un único punto de entrada (la raíz del agregado) que protege las invariantes y evita modificaciones inconsistentes desde fuera.

## Aplicación para Arquitecto

- Definir agregados por invariante de negocio: todo lo que debe ser consistente en una transacción pertenece al mismo agregado; la raíz es la única referencia expuesta al exterior.
- Diseñar agregados pequeños cuando sea posible para reducir contención y complejidad; evitar agregados que abarquen todo el contexto.

## Aplicación para Tekton

- No permitir que servicios o repositorios accedan directamente a entidades hijas del agregado; todas las operaciones deben pasar por métodos del Aggregate Root que actualicen el estado interno y disparen eventos si aplica.
- Persistir el agregado completo en una transacción; no exponer colecciones internas como listas modificables desde fuera.

## Referencias

[23], [24] — DDD, Agregados, Aggregate Root, transacciones.

---
*Definición en paths.principlesPath/aggregate-roots/ (contrato paths.principlesPath/principles-contract.md).*
