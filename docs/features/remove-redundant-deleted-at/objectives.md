---
type: objective
status: in-progress
---
# Objetivos
- Remover chequeos manuales de `DeletedAt == null` en AuthService y GetAllPostalCodesCommandHandler, delegando la responsabilidad a EF Core Global Query Filters.
