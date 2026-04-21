# Spec
1. Añadir validación explícita tras leer la configuración: rechazar secretos que contengan el marcador `[INJECTED_VIA_ENV_OR_SECRET_MANAGER`.
2. En `Development`, exigir `UserSecretsId` o variables de entorno.
