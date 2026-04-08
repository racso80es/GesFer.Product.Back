---
title: "Remover verificaciones redundantes de DeletedAt == null"
created: 2026-04-03
priority: high
---
# Objetivo
Eliminar validaciones redundantes de `DeletedAt == null` en los Handlers y Servicios (especificamente en AuthService y GetAllPostalCodesCommandHandler), ya que Entity Framework Core Global Query Filters gestiona automáticamente el soft delete, de acuerdo a la documentación y las reglas del sistema.
