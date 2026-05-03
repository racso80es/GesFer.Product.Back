---
title: "Spec"
---
# Spec
Se requiere modificar `GetCustomerByIdCommand`, `UpdateCustomerCommand` y `DeleteCustomerCommand` para aceptar `Guid CompanyId`. Los manejadores deben incluir el filtro `c.CompanyId == command.CompanyId` en las consultas de base de datos.
