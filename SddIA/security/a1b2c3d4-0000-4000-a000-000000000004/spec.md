---
category: Seguridad de Aplicaciones
id: a1b2c3d4-0000-4000-a000-000000000004
interested_agents:
- architect
- security-engineer
- tekton-developer
metadata:
  difficulty: Advanced
  status: Published
tags:
- Criptografía
- Clave Pública
- Clave Privada
- Autenticación descentralizada
---

# Criptografía Asimétrica en JWT

## Descripción
Los JWT pueden firmarse usando algoritmos asimétricos como RS256 [7]. Esto implica generar un par de claves: una privada y una pública [7].

## Arquitectura Segura
El servidor central (que tiene la clave privada) es el único capaz de emitir y firmar los tokens de forma segura [8]. Por otro lado, la clave pública se puede distribuir a otros microservicios o sistemas descentralizados.

## Beneficios
Esto permite que cualquier subsistema pueda verificar matemáticamente la legitimidad del token sin necesidad de comunicarse con el servidor original y sin poner en riesgo la capacidad de crear nuevos tokens [9].
