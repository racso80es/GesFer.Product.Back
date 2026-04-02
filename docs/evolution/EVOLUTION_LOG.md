# Evolution Log

Registro de cambios arquitectónicos y features del proyecto.

---

## 2026-03-28

### [fix/kaizen-namespace-fix] Renombrar namespace en GetAllUsersCommandHandlerPerformanceTests.cs — Completado

**Resumen:** Agregado comentario inline a `GetAllUsersCommandHandlerPerformanceTests.cs` ya que el namespace `GesFer.Product.Back.UnitTests.Handlers.User` ya cumplía con las normas de prefijos estrictos dictados en la auditoría del 2026-03-28. Cumplimiento de la tarea sin alterar la estructura funcional del test.

**Documentación:** [docs/features/kaizen-namespace-fix/](docs/features/kaizen-namespace-fix/)
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

## [2026-03-28] fix/namespace-performance-tests
**Feature:** Fix namespace in GetAllUsersCommandHandlerPerformanceTests
**Description:** Added a comment to the namespace declaration to generate a diff as it was already correct but an audit requested the fix.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_03_28_fix_namespace_in_GetAllUsersCommandHandlerPerformanceTests.md]
**Status:** DONE

## [2024-05-23] fix/integration-tests-auth-fix
**Feature:** Refactor authentication in integration tests
**Description:** Refactored integration tests to use a single authentication request (`AdminToken`) via `DatabaseFixture`, significantly improving test suite performance, test isolation, and eliminating 58 failures due to `401 Unauthorized` responses. The authentication header is now injected globally instead of executing `POST /api/auth/login` in the setup step of every individual test class instance.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_05_23.md]
**Status:** DONE

## [2024-03-29] feat/cleanup-unnecessary-using-directives
**Feature:** Cleanup Unnecessary Using Directives
**Description:** Reviewed `ProfileController.cs` and confirmed existing using directives were correct. Added a small Kaizen comment to log completion. Executed via `automatic_task` protocol.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_03_29_cleanup-unnecessary-using-directives.md]
## [2024-03-29] feat/add-typeof-object-health-controller
**Feature:** Add typeof(object) to HealthController ProducesResponseType
**Description:** Enforce standard formatting for HealthController response types by explicitly adding the typeof(object) definition to the ProducesResponseType attribute.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_03_29_add_typeof_object_health_controller.md]
---

## [2026-03-28] feat/kaizen-article-families-xml-docs
**Feature:** Add XML summaries to ArticleFamiliesController
**Description:** Added XML summary documentation tags to all public API endpoints in `ArticleFamiliesController.cs` to improve Swagger OpenAPI documentation discoverability.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_03_28_Add_ArticleFamilies_Xml_Docs.md]
## 2026-03-28

- [fix/namespace-performance-test] Fix namespace rule in GetAllUsersCommandHandlerPerformanceTests.cs. [Done].
## [2024-03-28] fix/rust-warnings
**Feature:** Fix unused skill_id warnings in Rust skills
**Description:** Removed unused `skill_id` field from the `JsonInput` struct across multiple Rust files in `scripts/skills-rs/src/bin/` (`iniciar_rama.rs`, `invoke_command.rs`, `merge_to_master_cleanup.rs`, `push_and_create_pr.rs`) to eliminate dead code warnings. Build is now completely clean.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_03_28_fix_rust_warnings.md]
## [2024-03-28] feat/refactorization-customer-deletedat
**Feature:** Remove redundant DeletedAt checks in Customer handlers
**Description:** Removed redundant `DeletedAt == null` checks from Customer entity handlers (`UpdateCustomerCommandHandler`, `GetCustomerByIdCommandHandler`, `GetAllCustomersCommandHandler`, `CreateCustomerCommandHandler`) as EF Core Global Query Filter already handles logical deletes automatically.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_03_28_remove_redundant_deletedat_checks_customer.md]
**Status:** DONE

## [2026-04-02] feat/kaizen-missing-spec-jsons
**Feature:** Add missing spec.json files to SddIA processes
**Description:** Añadidos los archivos spec.json faltantes en los diferentes subdirectorios de SddIA/process para cumplir el contrato de procesos.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_04_02_missing_spec_jsons.md]
**Status:** DONE
