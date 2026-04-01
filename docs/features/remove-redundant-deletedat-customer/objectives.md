---
title: Objectives
---
# Objetivos
Eliminar los checks redundantes de `DeletedAt == null` en los manejadores de entidades de Customer, dado que el EF Core Global Query Filter ya se encarga de excluir los registros eliminados de forma lógica.
