---
title: "Plan"
---
# Plan
1. Actualizar comandos (`GetCustomerByIdCommand`, `UpdateCustomerCommand`, `DeleteCustomerCommand`) añadiendo `CompanyId`.
2. Modificar el controlador `CustomerController` para pasar `this.GetCompanyId()` a los comandos.
3. Actualizar manejadores para validar `c.CompanyId == command.CompanyId`.
