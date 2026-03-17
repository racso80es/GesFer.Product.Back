---
category: Clean Code
contract_ref: paths.principlesPath/principles-contract.md
id: c1f7a01b-9a8c-4d3b-8f2e-1e3d4a5b6c74
metadata:
  difficulty: Beginner
  status: Published
principle_id: evitar-magic-numbers
tags:
- Naming
- Clean Code
- Constantes
---

# Evitar Magic Numbers

**principle_id:** `evitar-magic-numbers`

## Resumen

El uso de números mágicos (Magic Numbers) insertados directamente en la lógica del código sin un contexto descriptivo merma en gran medida la comprensión del programa para otros desarrolladores o personas con diferente contexto cultural. La solución principal a este mal olor consiste en extraer dichos valores estáticos hacia constantes con nombres altamente descriptivos que revelen su intención dentro de las reglas de negocio.

## Objetivo

Hacer que todo valor numérico (o literal con significado de negocio) tenga un nombre que explique su rol, mejorando la mantenibilidad y reduciendo errores al cambiar umbrales o constantes en un único lugar.

## Aplicación para Arquitecto

- Definir constantes de dominio (límites, códigos, tiempos) en capas compartidas o en módulos de configuración, con nombres que reflejen el concepto de negocio.
- Documentar el origen y significado de constantes críticas (normativas, límites legales, etc.) en spec o documentación de dominio.

## Aplicación para Tekton

- Extraer a constantes con nombre cualquier número o string literal que represente una regla de negocio (ej. `MaxLoginAttempts`, `DefaultPageSize`, `StatusCodeNotFound`).
- Preferir constantes en el ámbito más próximo al uso (clase o módulo) y evitar constantes globales sin categoría clara.

## Referencias

[11], [12] — Magic numbers, constantes con nombre, expresividad.

---
*Definición en paths.principlesPath/evitar-magic-numbers/ (contrato paths.principlesPath/principles-contract.md).*
