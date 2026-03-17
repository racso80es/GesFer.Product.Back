---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c82
metadata:
  difficulty: Intermediate
  status: Published
principle_id: ley-de-demeter-tell-dont-ask
tags:
- POO
- Encapsulación
- Acoplamiento
---

# Ley de Demeter (Tell, Don't Ask)

**principle_id:** `ley-de-demeter-tell-dont-ask`

## Resumen

La Ley de Demeter, materializada también en la filosofía "Tell, Don't Ask", fomenta no establecer diálogos con objetos desconocidos dentro de nuestra arquitectura orientada a objetos. En lugar de valernos de múltiples getters para extraer datos de los modelos y procesar comportamientos en servicios anémicos externos, este principio exige trasladar y encasillar la responsabilidad del comportamiento orgánicamente hacia dentro de los propios modelos de dominio.

## Objetivo

Reducir acoplamiento y mejorar encapsulación: los objetos exponen comportamiento (métodos que hacen algo) en lugar de solo datos (getters encadenados), y el cliente "dice" qué quiere que haga el objeto en lugar de "preguntar" y decidir por él.

## Aplicación para Arquitecto

- Favorecer un diseño rico en dominio: entidades y agregados con métodos que expresen acciones de negocio (ej. pedido.Cerrar(), usuario.CambiarEmail()), no solo DTOs con getters.
- Evitar que los servicios de aplicación orquesten lógica que debería vivir en el dominio (cálculos, validaciones de invariantes) extrayendo datos con getters; en su lugar, invocar métodos de dominio que devuelvan resultados o efectos.

## Aplicación para Tekton

- No encadenar llamadas del tipo objeto.GetA().GetB().GetC(); en su lugar, exponer en el objeto raíz un método que realice la operación (Tell) o devuelva un resultado ya calculado.
- Mover lógica que hoy está en servicios y que solo usa datos de una entidad hacia esa entidad o su agregado, dejando al servicio la orquestación de alto nivel.

## Referencias

[29], [30] — Ley de Demeter, Tell Don't Ask, diseño rico.

---
*Definición en paths.principlesPath/ley-de-demeter-tell-dont-ask/ (contrato paths.principlesPath/principles-contract.md).*
