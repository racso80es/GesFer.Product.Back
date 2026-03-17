---
category: Principios SOLID
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c75
metadata:
  difficulty: Intermediate
  status: Published
principle_id: single-responsibility-principle-srp
tags:
- SOLID
- Cohesión
- Acoplamiento
- Arquitectura
---

# Single Responsibility Principle (SRP)

**principle_id:** `single-responsibility-principle-srp`

## Resumen

El Principio de Responsabilidad Única sostiene que en el diseño de software debemos agrupar y cohesionar las piezas que cambian por los mismos motivos y, al mismo tiempo, separar las que cambian por razones completamente distintas. Aplicado a casos de uso, este principio dicta que la clase encargada de llevar a cabo una acción fundamental (por ejemplo, registrar un usuario) debe ocuparse únicamente de ello y no aglomerar lógicas secundarias derivadas como enviar correos electrónicos.

## Objetivo

Lograr módulos y clases con una única razón de cambio, reduciendo el impacto de las modificaciones y facilitando pruebas y evolución independiente.

## Aplicación para Arquitecto

- Delimitar responsabilidades por capa y por caso de uso: orquestación en aplicación, reglas en dominio, efectos secundarios (email, eventos) en infraestructura o handlers.
- Revisar que los servicios de aplicación no mezclen coordinación del flujo con envío de notificaciones, persistencia secundaria o logging de negocio; delegar estos en colaboradores inyectados.

## Aplicación para Tekton

- Al crear o modificar un caso de uso, mantener una única responsabilidad: orquestar una acción de negocio. Notificaciones, auditoría y persistencia secundaria deben ser dependencias (interfaces) inyectadas y ejecutadas de forma explícita o por eventos.
- Si una clase crece con muchos "y además hace...", extraer esas responsabilidades a servicios o suscriptores de eventos.

## Referencias

[13], [14], [15] — SOLID, SRP, cohesión y casos de uso.

---
*Definición en paths.principlesPath/single-responsibility-principle-srp/ (contrato paths.principlesPath/principles-contract.md).*
