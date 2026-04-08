---
title: Missing spec.json in SddIA processes
created: 2024-04-02
---
# Refactorización: Añadir archivos spec.json faltantes en los procesos de SddIA

Varios procesos en `SddIA/process/` carecen de su archivo `spec.json` correspondiente.
Esto viola el contrato de procesos. Se debe crear un `spec.json` mínimo para cada uno de los siguientes procesos:
audit-tool, feature, automatic_task, create-tool, create-template, refactorization, correccion-auditorias, bug-fix.