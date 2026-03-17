---
category: Seguridad en Código
id: a1b2c3d4-0000-4000-a000-000000000017
interested_agents:
- security-engineer
- tekton-developer
- auditor
metadata:
  difficulty: Intermediate
  status: Published
tags:
- Git
- GPG
- Firma
- Identidad
---

# Firmas de Commits con GPG

## Descripción
Git permite que un atacante suplante el nombre y correo de cualquier persona al hacer un commit. Para prevenir inyecciones de código haciéndose pasar por desarrolladores legítimos (Suplantación de Identidad en Git), se requiere el uso de claves GPG (GNU Privacy Guard) [25].

## Verificación de Identidad
Al asociar una clave GPG a tu identidad de desarrollador y configurar Git para requerir la firma, plataformas como GitHub certifican matemáticamente la autenticidad real de quien sube la propuesta de código [26].
