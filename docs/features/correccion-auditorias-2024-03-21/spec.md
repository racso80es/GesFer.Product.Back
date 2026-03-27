---
type: specification
status: completed
---

# Especificación

Actualizar el test `UpdateMyCompany_WithValidToken_ShouldReturn200` para que preserve el nombre de la empresa "Empresa Demo", previniendo que los tokens JWT de otros tests fallan con 401 debido al uso de un `[Collection("DatabaseStep")]` compartido.
