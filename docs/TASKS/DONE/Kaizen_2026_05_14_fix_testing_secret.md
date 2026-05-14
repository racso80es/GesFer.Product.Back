---
id: "KAIZEN-2026-05-14"
title: "Fix Zero-Trust Violation in appsettings.Testing.json"
date: "2026-05-14"
status: "PENDING"
---
# Fix Zero-Trust Violation in appsettings.Testing.json

## Objective
Remove the hardcoded `JwtSettings:SecretKey` from `src/IntegrationTests/appsettings.Testing.json` and replace it with the standard placeholder `[INJECTED_VIA_ENV_OR_SECRET_MANAGER_MIN_32_CHARS]` to comply with the Zero-Trust configuration policy. The actual secret is correctly injected via environment variables during test initialization in `IntegrationTestWebAppFactory.cs`, so removing it from the JSON file will not break tests.
