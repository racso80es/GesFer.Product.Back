---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c71
metadata:
  difficulty: Intermediate
  status: Published
principle_id: preparatory-refactoring
tags:
- Refactoring
- Clean Code
- Diseño Evolutivo
---

# Preparatory Refactoring

**principle_id:** `preparatory-refactoring`

## Resumen

El Preparatory Refactoring consiste en adaptar y mejorar el diseño del código existente antes de introducir una nueva funcionalidad o feature. Este enfoque se basa en "hacer hueco" para que la implementación de la nueva funcionalidad sea mucho más sencilla y natural. Al refactorizar previamente, se reduce la complejidad del código y se minimiza significativamente el margen de error humano.

## Objetivo

Facilitar la incorporación de nuevas capacidades sin deformar el diseño: primero se adapta el código actual para que el cambio encaje de forma coherente, y después se implementa la funcionalidad en ese espacio ya preparado.

## Aplicación para Arquitecto

- En el plan de implementación, identificar fases de "refactor preparatorio" antes de tocar la lógica de negocio nueva.
- Recomendar puntos de extensión (interfaces, métodos virtuales, eventos) donde la nueva feature pueda integrarse sin invadir responsabilidades ajenas.

## Aplicación para Tekton

- Antes de escribir la lógica nueva, ejecutar un refactor acotado: extraer interfaces, mover métodos, introducir puntos de enganche. Luego implementar la feature sobre esa base.
- Evitar mezclar en el mismo commit refactor grande y lógica nueva; separar en commits distintos para facilitar revisión y rollback.

## Referencias

[2], [3], [4] — Diseño evolutivo y refactoring previo a features.

---
*Definición en paths.principlesPath/preparatory-refactoring/ (contrato paths.principlesPath/principles-contract.md).*
