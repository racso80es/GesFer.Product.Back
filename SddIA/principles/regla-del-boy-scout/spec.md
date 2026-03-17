---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c70
metadata:
  difficulty: Beginner
  status: Published
principle_id: regla-del-boy-scout
tags:
- Clean Code
- Refactoring
- Buenas Prácticas
- Deuda Técnica
---

# Regla del Boy Scout

**principle_id:** `regla-del-boy-scout`

## Resumen

La regla del Boy Scout establece el principio fundamental de dejar el código en un estado mejor del que nos lo encontramos inicialmente. En el mundo de la programación y el diseño de software, esto significa aplicar refactorizaciones continuas a medida que interactuamos con el código, mejorando la legibilidad, extrayendo métodos o limpiando deuda técnica progresivamente al añadir nuevas funcionalidades.

## Objetivo

Mantener y mejorar la calidad del código base de forma incremental, sin necesidad de grandes iniciativas de refactorización. Cada intervención en un archivo o módulo debe dejar ese fragmento más claro, más testeable o más alineado con los principios del proyecto.

## Aplicación para Arquitecto

- Incluir en las guías de diseño la expectativa de que cualquier cambio en un módulo puede aprovecharse para reducir acoplamiento, mejorar nombres o extraer responsabilidades.
- Definir umbrales de deuda técnica aceptables y priorizar su reducción en las zonas que se toquen con más frecuencia.

## Aplicación para Tekton

- Al implementar una feature o fix, dedicar un margen acotado a mejorar el código que se ha tenido que leer o modificar (nombres, métodos largos, constantes mágicas).
- No dejar código peor del que se encontró: si se añade un parche, valorar si conviene un refactor mínimo antes o después.

## Referencias

[1] Clean Code y prácticas de refactoring continuo.

---
*Definición en paths.principlesPath/regla-del-boy-scout/ (contrato paths.principlesPath/principles-contract.md).*
