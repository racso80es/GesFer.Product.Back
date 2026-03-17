---
category: Arquitectura de Software
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c85
metadata:
  difficulty: Intermediate
  status: Published
principle_id: peligro-del-modulo-shared
tags:
- Clean Architecture
- Shared Kernel
- Cohesión
- Acoplamiento
---

# El Peligro del Módulo Shared

**principle_id:** `peligro-del-modulo-shared`

## Resumen

Aglutinar indiscriminadamente entidades o lógicas en una carpeta compartida (Shared Kernel) constituye un riesgo notable que deteriora la cohesión del código base si no se custodia bajo reglas inflexibles. El mandato dicta que un módulo transversal debe contener exclusivamente elementos de infraestructura comunes o conceptos intrínsecos de dominio puro (como el tipado UUID genérico), pero nunca deberá hospedar casos de uso o reglas de aplicación que deriven en un alto acoplamiento interdepartamental.

## Objetivo

Limitar el contenido de los módulos compartidos a lo estrictamente común y estable (utilidades, tipos base, contratos de infraestructura), evitando que se conviertan en un cajón de sastre que acople todos los bounded contexts o equipos.

## Aplicación para Arquitecto

- Definir una política explícita para lo que puede vivir en Shared: solo tipos primitivos de dominio (UUID, Money), utilidades de infraestructura (logging, fechas), y contratos estables; no entidades de negocio ni casos de uso.
- Revisar periódicamente el contenido del shared y extraer a contextos concretos todo lo que sea específico de un dominio o flujo.

## Aplicación para Tekton

- No añadir al shared nuevas dependencias de negocio ni clases que contengan reglas de un solo contexto; si dos contextos necesitan algo similar, valorar duplicación controlada o un contrato mínimo (interfaces, DTOs) en lugar de implementaciones compartidas.
- Mantener el shared libre de referencias a capas de aplicación o infraestructura específicas de un módulo.

## Referencias

[36], [37], [38] — Shared Kernel, Clean Architecture, cohesión.

---
*Definición en paths.principlesPath/peligro-del-modulo-shared/ (contrato paths.principlesPath/principles-contract.md).*
