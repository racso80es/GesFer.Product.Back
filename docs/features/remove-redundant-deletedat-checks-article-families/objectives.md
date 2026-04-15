---
type: objective
---
# Objetivo
Eliminar validaciones redundantes de `DeletedAt == null` en los Handlers y Servicios (especificamente en ArticleFamilies handlers), ya que Entity Framework Core Global Query Filters gestiona automáticamente el soft delete.
