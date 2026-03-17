---
category: Testing
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c88
metadata:
  difficulty: Advanced
  status: Published
principle_id: test-first-vs-tdd
tags:
- TDD
- Test-First
- Metodologías
- Diseño de Software
---

# Test-First vs TDD

**principle_id:** `test-first-vs-tdd`

## Resumen

Escribir el código de test previo a formalizar su código de producción (estrategia conocida como Test-First) aporta foco al desarrollador al evitar la sobreingeniería, diseñando mentalmente las respuestas en función del cliente que consumirá dicho bloque. Sin embargo, Test-Driven Development (TDD) asimila esa metodología e incorpora por encima un flujo constante e iterativo de validación, escritura y limpieza (Red-Green-Refactor) que extrae un diseño del software mucho más resiliente.

## Objetivo

Diferenciar "escribir el test antes que el código" (Test-First) del ciclo completo TDD (Red-Green-Refactor) y favorecer TDD cuando se busque diseño emergente y resistencia al cambio mediante pruebas automatizadas.

## Aplicación para Arquitecto

- Promover TDD en zonas de alta criticidad o de diseño inestable; aceptar Test-First como mínimo en features nuevas cuando no se aplique TDD estricto.
- Incluir en las guías de desarrollo la expectativa de tests antes o en paralelo al código de producción, y el ciclo de refactor tras verde.

## Aplicación para Tekton

- Practicar el ciclo TDD: escribir un test que falle (Red), implementar el mínimo para que pase (Green), refactorizar código y tests (Refactor). Repetir en pasos pequeños.
- Si no se usa TDD estricto, al menos escribir el test del caso de uso o del componente antes de implementar la lógica (Test-First), para fijar el contrato esperado.

## Referencias

[45], [46], [47], [48] — TDD, Test-First, Red-Green-Refactor.

---
*Definición en paths.principlesPath/test-first-vs-tdd/ (contrato paths.principlesPath/principles-contract.md).*
