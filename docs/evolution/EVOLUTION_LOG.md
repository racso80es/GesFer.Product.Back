# Evolution Log

Registro de cambios arquitectónicos y features del proyecto.

---

## 2026-03-28

### [fix/kaizen-namespace-fix] Renombrar namespace en GetAllUsersCommandHandlerPerformanceTests.cs — Completado

**Resumen:** Agregado comentario inline a `GetAllUsersCommandHandlerPerformanceTests.cs` ya que el namespace `GesFer.Product.Back.UnitTests.Handlers.User` ya cumplía con las normas de prefijos estrictos dictados en la auditoría del 2026-03-28. Cumplimiento de la tarea sin alterar la estructura funcional del test.

**Documentación:** [docs/features/kaizen-namespace-fix/](docs/features/kaizen-namespace-fix/)

---

## 2026-03-19

### [feat/kaizen-skills-tools-rust-json-contract] Kaizen skills/tools Rust + JSON según contratos SddIA — Completado

**Resumen:** Proceso create-skill, contrato JSON skills, implementación Rust completa: iniciar_rama, invoke_command, invoke_commit, merge_to_master_cleanup, push_and_create_pr. Tools: run_tests_local, postman_mcp_validation. Eliminación de fallback .ps1 en .bat.

**Documentación:** [docs/features/kaizen-skills-tools-rust-json-contract/](docs/features/kaizen-skills-tools-rust-json-contract/)

---

### [feature/fix-namespaces-remove-shared] Refactor namespaces GesFer.Product.Back.* — Completado

**Resumen:** Resolución de conflictos con main, refactor de namespaces eliminando dependencias de Shared, estandarización a `GesFer.Product.Back.*`. Incluye namespaces IntegrationTests y actualización de Dockerfile.

**Documentación:** [docs/features/fix-namespaces-remove-shared/](docs/features/fix-namespaces-remove-shared/)

## [2024-05-23] fix/integration-tests-auth-fix
**Feature:** Refactor authentication in integration tests
**Description:** Refactored integration tests to use a single authentication request (`AdminToken`) via `DatabaseFixture`, significantly improving test suite performance, test isolation, and eliminating 58 failures due to `401 Unauthorized` responses. The authentication header is now injected globally instead of executing `POST /api/auth/login` in the setup step of every individual test class instance.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_05_23.md]
**Status:** DONE
