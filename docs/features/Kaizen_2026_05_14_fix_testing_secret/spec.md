---
id: "KAIZEN-2026-05-14-spec"
---
# Specification
Update `src/IntegrationTests/appsettings.Testing.json` so that `JwtSettings:SecretKey` is set to `[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]`. Verify integration tests pass since `IntegrationTestWebAppFactory.cs` injects the test secret at runtime.
