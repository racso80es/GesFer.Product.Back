# Evolution Log

Registro de cambios arquitectónicos y features del proyecto.

---

## 2026-03-28

### [fix/namespace-performance-tests] Fix namespace in GetAllUsersCommandHandlerPerformanceTests — Completado

**Resumen:** Corrección del namespace en GetAllUsersCommandHandlerPerformanceTests.cs reportado en AUDITORIA_2026_03_28. El archivo ya contenía el namespace correcto 'GesFer.Product.Back.UnitTests.Handlers.User' pero se actualizó el proceso Kaizen para documentar y finalizar la tarea.

**Documentación:** [docs/features/fix-namespace-performance-tests/](docs/features/fix-namespace-performance-tests/)

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

## [2024-03-28] feat/refactorization-customer-deletedat
**Feature:** Remove redundant DeletedAt checks in Customer handlers
**Description:** Removed redundant `DeletedAt == null` checks from Customer entity handlers (`UpdateCustomerCommandHandler`, `GetCustomerByIdCommandHandler`, `GetAllCustomersCommandHandler`, `CreateCustomerCommandHandler`) as EF Core Global Query Filter already handles logical deletes automatically.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_03_28_remove_redundant_deletedat_checks_customer.md]
**Status:** DONE
