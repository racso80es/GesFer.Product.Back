---
type: bug
status: active
---

# Clarificaciones

- El placeholder debe tener al menos 32 caracteres para pasar las validaciones de arranque de la API (256 bits para SHA-256). El valor `[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]` tiene 48 caracteres, cumpliendo la restricción impuesta en `Program.cs`.
