---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c73
metadata:
  difficulty: Beginner
  status: Published
principle_id: clausulas-de-guarda-early-return
tags:
- Clean Code
- Legibilidad
- Control de Flujo
---

# Cláusulas de Guarda (Early Return)

**principle_id:** `clausulas-de-guarda-early-return`

## Resumen

Las cláusulas de guarda permiten simplificar el control de flujo evitando el uso de condicionales profundamente anidados. Esta técnica invita a realizar comprobaciones de error o estados inválidos al principio del método para interrumpir la ejecución tempranamente devolviendo un error o lanzando una excepción. Su uso mejora drásticamente la legibilidad porque mantiene el "happy path" del algoritmo en el primer nivel de indentación.

## Objetivo

Reducir la complejidad ciclomática y la anidación, haciendo que el flujo principal del método sea lineal y fácil de seguir, y que los casos excepcionales queden explícitos al inicio.

## Aplicación para Arquitecto

- Establecer en las guías de código que los métodos públicos validen precondiciones al inicio y devuelvan o lancen de forma temprana en caso de fallo.
- Combinar con manejo de errores tipado (Either/Result) cuando el lenguaje o el proyecto lo soporten.

## Aplicación para Tekton

- Al escribir un método: primero comprobar argumentos nulos, permisos, estado inválido; si no se cumple, return o throw. A continuación, implementar solo el camino feliz con la mínima indentación.
- Evitar estructuras del tipo `if (ok) { ... mucha lógica ... } else { ... }`; sustituir por `if (!ok) return ...;` y luego la lógica principal.

## Referencias

[8], [9], [10] — Guard clauses, early return, flujo lineal.

---
*Definición en paths.principlesPath/clausulas-de-guarda-early-return/ (contrato paths.principlesPath/principles-contract.md).*
