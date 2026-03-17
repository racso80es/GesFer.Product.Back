---
category: Testing
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c87
metadata:
  difficulty: Intermediate
  status: Published
principle_id: caso-de-uso-como-unidad-de-testing
tags:
- Unit Testing
- Casos de Uso
- Mantenibilidad
- Clean Architecture
---

# Caso de Uso como Unidad de Testing

**principle_id:** `caso-de-uso-como-unidad-de-testing`

## Resumen

Restringir dogmáticamente las pruebas unitarias a una relación de "un test estricto por cada clase del dominio" acostumbra a producir suites de prueba muy inestables y rígidas ante refactorizaciones internas. Dentro del marco de arquitecturas orientadas a casos de uso (Clean Architecture), la recomendación radica en trazar el límite de la "unidad" en el propio Servicio de Aplicación (Caso de uso completo); inyectando reemplazos técnicos y verificando el comportamiento orgánico de las reglas que colaboran entre sí.

## Objetivo

Equilibrar cobertura y mantenibilidad: probar el comportamiento observable del caso de uso (entrada → salida/efectos) con dobles de infraestructura, en lugar de fijar tests a implementaciones internas de cada clase de dominio que cambian con frecuencia.

## Aplicación para Arquitecto

- Definir la frontera de tests unitarios en el nivel de caso de uso (servicio de aplicación): entrada, expectativas sobre resultado y sobre colaboradores mockeados (repositorio, notificaciones).
- Documentar qué se considera "unidad" en el proyecto: un caso de uso con dependencias sustituidas, no cada entidad por separado.

## Aplicación para Tekton

- Escribir tests que instancien el servicio de aplicación (caso de uso), inyecten mocks o fakes de IRepository, IEventBus, etc., y afirmen sobre el resultado y las interacciones esperadas (ej. que se llamó a Save con la entidad correcta).
- Refactorizar el dominio con libertad; los tests del caso de uso no deben romperse por cambios internos de implementación mientras el comportamiento observable se mantenga.

## Referencias

[42], [43], [44] — Unit testing, caso de uso, Clean Architecture.

---
*Definición en paths.principlesPath/caso-de-uso-como-unidad-de-testing/ (contrato paths.principlesPath/principles-contract.md).*
