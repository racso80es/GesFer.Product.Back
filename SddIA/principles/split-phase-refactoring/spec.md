---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c72
metadata:
  difficulty: Intermediate
  status: Published
principle_id: split-phase-refactoring
tags:
- Refactoring
- Algoritmos
- Legibilidad
---

# Split Phase Refactoring

**principle_id:** `split-phase-refactoring`

## Resumen

Esta técnica de refactorización se aplica para simplificar métodos o funciones muy extensas y fuertemente acopladas dividiéndolas en fases claras y secuenciales. Por ejemplo, un script monolítico puede estructurarse primero en una fase de obtención y parseo de datos, luego en una fase donde se aplica estrictamente la lógica de negocio, y finalmente en una fase dedicada a formatear o serializar la salida.

## Objetivo

Separar responsabilidades distintas (I/O, negocio, presentación) en fases bien delimitadas, mejorando la legibilidad, el testing y la evolución independiente de cada fase.

## Aplicación para Arquitecto

- Diseñar flujos de aplicación o pipelines en fases explícitas (lectura/parseo → dominio → persistencia/salida) y documentar los contratos entre fases.
- Evitar que un mismo componente mezcle acceso a datos, reglas de negocio y formateo de respuesta.

## Aplicación para Tekton

- Al encontrar métodos largos que hacen varias cosas, aplicar Split Phase: extraer cada fase a funciones o clases con nombres que describan la fase (ej. `ParseInput`, `ApplyBusinessRules`, `FormatOutput`).
- Mantener una única dirección del flujo entre fases y pasar datos mediante estructuras o DTOs claros, no mediante efectos secundarios ocultos.

## Referencias

[5], [6], [7] — Refactoring: Split Phase (Martin Fowler y fuentes relacionadas).

---
*Definición en paths.principlesPath/split-phase-refactoring/ (contrato paths.principlesPath/principles-contract.md).*
