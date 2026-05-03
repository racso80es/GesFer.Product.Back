---
title: "Implementation"
---
# Implementation
Se añade `CompanyId` a los records de comandos. Se inyecta la obtención del `CompanyId` desde los claims en los endpoints del controlador. Los queries de Entity Framework en los manejadores ahora filtran por ambos campos.
