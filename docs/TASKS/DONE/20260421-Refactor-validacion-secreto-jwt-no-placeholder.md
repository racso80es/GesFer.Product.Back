---
created: 2026-04-21
type: Kaizen
title: Validación de secreto JWT anti-placeholder en arranque
status: pending
origin_pr: origin/fix/hardcoded-jwt-secret-14196445710699879921
detecting_agent: security-engineer
---

# Tarea Automatizada: Refactorización de validación de secreto JWT (anti-placeholder)

**Origen:** PR `origin/fix/hardcoded-jwt-secret-14196445710699879921` (rama: `fix/hardcoded-jwt-secret-14196445710699879921`)

**Agente detector:** security-engineer

## Contexto

Se identificó que la validación actual en `Program.cs` solo comprueba longitud mínima del `JwtSettings:SecretKey`, permitiendo arranque con un literal de plantilla presente en el repositorio.

## Objetivo del Refactor (S+ Grade)

Endurecer el arranque para que **ningún** valor de firma JWT sea aceptado si coincide con valores de plantilla conocidos o si carece de procedencia segura según entorno (Development vs Production), sin romper el flujo legítimo con User Secrets o variables de entorno.

## Instrucciones para Tekton/Jules

1. Añadir validación explícita tras leer la configuración: rechazar secretos que contengan el marcador `[INJECTED_VIA_ENV_OR_SECRET_MANAGER` o la lista negra de placeholders acordada.
2. En `Development`, documentar y, si aplica la política del repo, exigir `UserSecretsId` o variables `JwtSettings__SecretKey` / `InternalSecret` para no depender del JSON versionado.
3. Añadir prueba de integración o test de arranque que falle si se detecta placeholder en la configuración efectiva (según política del repo).
