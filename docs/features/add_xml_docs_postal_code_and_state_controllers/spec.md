---
title: Especificación - Agregar documentación XML a controladores
description: Especificación técnica para los comentarios XML
date: 2026-04-27
---
# Especificación

## Alcance
- Archivo `src/Api/Controllers/PostalCodeController.cs`
- Archivo `src/Api/Controllers/StateController.cs`

## Detalles
Se debe agregar la etiqueta `<summary>` y, si corresponde, `<param>` a:
- Constructor `PostalCodeController()`
- Método `Create(CreatePostalCodeDto dto)` en `PostalCodeController`
- Constructor `StateController()`
