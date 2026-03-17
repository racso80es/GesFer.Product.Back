---
category: Arquitectura de Software
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c86
metadata:
  difficulty: Intermediate
  status: Published
principle_id: identificadores-uuids-desde-el-cliente
tags:
- Identificadores
- UUID
- Frontend
- APIs
---

# Identificadores UUIDs desde el Cliente

**principle_id:** `identificadores-uuids-desde-el-cliente`

## Resumen

Acoplar la expedición de identificadores numéricos a secuencias autoincrementales controladas por motores de base de datos interfiere gravemente con el ciclo de programación asíncrona y la validación en tests aislados. Por el contrario, potenciar que la generación de identificadores universales (UUIDs) recaiga previamente en el lado del cliente o Frontend simplifica enormemente la topología porque permite la comunicación offline, operaciones nativas en masa y simplificación del valor retornado.

## Objetivo

Permitir que las entidades puedan ser identificadas antes de persistirse, facilitando creación en lote, operaciones offline, y pruebas sin depender del orden de inserción o del motor de base de datos.

## Aplicación para Arquitecto

- Decidir en qué entidades el ID es generado por el cliente (UUID) y documentar el contrato de la API (el cliente envía el ID en el cuerpo o en la URL al crear el recurso).
- Evaluar impacto en idempotencia y duplicados: aceptar el mismo UUID en reintentos puede usarse como patrón de idempotencia.

## Aplicación para Tekton

- En APIs de creación, aceptar un identificador opcional o obligatorio (UUID) en el request; si no se envía, el servidor puede generar uno, pero preferir generación en cliente cuando el flujo lo permita.
- En tests, generar UUIDs deterministas o fijos para no depender de secuencias de base de datos y poder afirmar sobre IDs conocidos.

## Referencias

[39], [40], [41] — UUID client-side, APIs REST, idempotencia.

---
*Definición en paths.principlesPath/identificadores-uuids-desde-el-cliente/ (contrato paths.principlesPath/principles-contract.md).*
