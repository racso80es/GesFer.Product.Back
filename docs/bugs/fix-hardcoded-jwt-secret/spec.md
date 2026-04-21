---
type: bug
status: active
---

# Especificaciones

- Reemplazar `JwtSettings:SecretKey` en `appsettings.json`, `appsettings.Development.json` y `appsettings.Development.json.example` por el placeholder `[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]`.
- Reemplazar `InternalSecret` en `appsettings.Development.json` y `appsettings.Development.json.example` por el placeholder `[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]`.
- Eliminar toda clave dura (hardcoded) del control de versiones.
