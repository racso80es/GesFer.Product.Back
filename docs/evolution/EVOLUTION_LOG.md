# Evolution Log

Registro de cambios arquitectónicos y features del proyecto.

---

## [2026-04-27] feat/add-xml-docs-postal-code-and-state-controllers
**Feature:** Agregar documentación XML a controladores
**Description:** Se agregó documentación XML faltante a los constructores de PostalCodeController y StateController, así como al método Create de PostalCodeController.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_27_add_xml_docs_postal_code_and_state_controllers.md]
## [2026-04-25] feat/kaizen-clean-usings-controllers
**Feature:** Limpieza de directivas using no utilizadas
**Description:** Se eliminaron directivas using innecesarias (ej. System.Security.Claims) en DashboardController.cs para mejorar la salud del código según la tarea Kaizen.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_25_clean_usings_controllers.md]
## [2026-04-24] fix/correccion-auditorias-2026-04-24
**Feature:** Refactorización de permisos en AuthService
**Description:** Se optimizó el método GetUserPermissionsAsync eliminando los bucles foreach e inicializando el HashSet usando colecciones nativas y UnionWith.
**Reference:** [docs/features/correccion-auditorias-2026-04-24]
**Status:** DONE

---

## [2026-04-24] feat/kaizen-2026-04-24-tax-types-controller-xml-docs
**Feature:** Documentación XML y ResponseTypes en Controladores
**Description:** Se añadieron etiquetas XML faltantes y atributos ProducesResponseType, en especial el Status401Unauthorized en los controladores TaxTypes, City y State, según los requerimientos de la tarea Kaizen.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_24_tax_types_controller_xml_docs.md]
**Status:** DONE

---

## [2026-04-23] fix/analyze-sddia-evolution-log-bug
**Feature:** Corrección de persistencia errónea en logs de sistema
**Description:** Análisis y corrección del error donde agentes IA escribían logs de producto en la bitácora evolutiva de SddIA. Se incluyeron directrices explícitas para separación de naturalezas y mecanismo de validación pre-escritura en `sddia-evolution-sync.md`.
**Reference:** [docs/TASKS/DONE/20260423-bug_log_SddIA.md]
**Status:** DONE

---

## [2026-04-22] feat/audit-documentar-contexto-user
**Feature:** Auditoría y Documentación de Contexto User
**Description:** Extracción de la especificación técnica completa de la entidad User y generación del documento formal `docs/DocumentacionUsuarios.md` estructurado en vectores clave como esquema, semillas, handlers y legacy mapping, sin modificaciones de código fuente.
**Reference:** [docs/TASKS/DONE/documentar_contexto_user.md]
**Status:** DONE

---

## [2026-04-21] fix/hardcoded-jwt-secret-14196445710699879921
**Feature:** Validación de secreto JWT anti-placeholder en arranque
**Description:** Validated JWT secret keys to prevent repositories' versioned placeholders from being used in runtime, adding UserSecretsId and enforcing use of User Secrets or ENV vars.
**Reference:** [docs/TASKS/DONE/20260421-Refactor-validacion-secreto-jwt-no-placeholder.md]
**Status:** DONE

---

## [2023-10-25] fix/cors-vulnerability
**Feature:** Fix overly permissive CORS policy
**Description:** Corregida vulnerabilidad CORS (AllowAnyOrigin) configurando `AllowedOrigins` explícitamente en `appsettings.json` según el entorno de despliegue y renombrando la política a `GesFerCorsPolicy`. Añadidas pruebas de integración.
**Reference:** [docs/bugs/fix-cors-vulnerability/objectives.md]
**Status:** DONE

---

## [2026-04-18] feat/kaizen-customer-xml-docs
**Feature:** Validate CustomerController XML docs
**Description:** Validated that XML Docs and ProducesResponseType attributes correctly exist in CustomerController.cs. Added an inline comment for tracking.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_18_Customer_Xml_Docs.md]
**Status:** DONE
## [2026-04-18] feat/sddia-process-validate-pull-requests
**Feature:** Proceso SddIA validate-pull-requests (validación integral de PRs, S+ Grade)
**Description:** Nuevo proceso en `SddIA/process/validate-pull-requests/`, índices y difusión (#Process, AGENTS, normas PR, Cursor). Documentación de tarea en `docs/features/sddia-process-validate-pull-requests/`.
**Reference:** [docs/features/sddia-process-validate-pull-requests/objectives.md]
**Status:** PR en curso
## [2026-04-16] feat/kaizen-remove-redundant-deletedat-user
**Feature:** Remove redundant DeletedAt checks in User handlers
**Description:** Removed explicit DeletedAt checks as EF Core global filters handle them.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_16_remove_redundant_deletedat_checks_user.md]
**Status:** DONE

---

## [2026-04-13] feat/kaizen-2026-04-13-remove-redundant-deletedat-supplier
**Feature:** Remove redundant DeletedAt checks in Supplier handlers
**Description:** Removed redundant explicitly coded `DeletedAt == null` checks from EF Core queries in Supplier command handlers, relying on EF Core Global Query Filters.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_13_remove_redundant_deletedat_checks_supplier.md]
**Status:** DONE

---

## [2026-04-12] feat/kaizen-dashboard-produces
**Feature:** Add ProducesResponseType to DashboardController
**Description:** Added `[ProducesResponseType]` attributes to `GetStats` in `DashboardController.cs` to improve Swagger OpenAPI discoverability.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_12_dashboard_controller_produces.md]
## [2026-04-10] feat/kaizen-swagger-docs
**Feature:** Enable XML Documentation in Swagger UI
**Description:** Enabled XML documentation generation in `GesFer.Api.csproj` and configured SwaggerGen to include comments.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_10_swagger_docs.md]
**Status:** DONE

---

## [2026-04-10] fix/bug-log-sddia
**Feature:** Corregir confusión en logs de evolución
**Description:** Se añadieron restricciones explícitas en las normas de SddIA y reglas de IDE para asegurar que la evolución fuera de SddIA se registre en EVOLUTION_LOG.md y no en el log interno de SddIA.
**Reference:** [docs/TASKS/DONE/bug_log_SddIA.md]
**Status:** DONE

---

## [2026-04-08] feat/kaizen-remove-todos-sddia2
**Feature:** Remove TODOs from SddIA
**Description:** Replaced explicit TODO strings with [ACTION-REQUIRED] to satisfy Clean Code rules.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_08_remove_todos.md]
**Status:** DONE

---

## [2024-04-06] fix/xunit2013-warnings
**Feature:** Fix xUnit2013 warnings in Performance Tests
**Description:** Replaced Assert.Equal(0, mockAdminApiClient.Invocations.Count) with Assert.Empty() in GetAllUsersCommandHandlerPerformanceTests.cs.
**Reference:** [docs/TASKS/DONE/Kaizen_2024_04_06_fix-xunit2013-warnings.md]
**Status:** DONE

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

## [2026-04-08] feat/refactorization-correccion-auditoria-2026-04-08
**Feature:** Ejecución de auditoría diaria estructural
**Description:** Generación de reporte `docs/audits/AUDITORIA_2026_04_08.md`. Validación de código, infraestructura y reglas async superada al 100%. No se registraron issues pendientes ni deuda técnica.
**Reference:** [docs/features/correccion-auditoria-2026-04-08/objectives.md]
**Status:** DONE

## [2026-04-13] feat/remove-redundant-deletedat-checks-article-families
**Feature:** Remove redundant DeletedAt checks in ArticleFamilies handlers
**Description:** Removed redundant explicitly coded `DeletedAt == null` checks, relying on EF Core Global Query Filters.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_13_remove_redundant_deletedat_checks_article_families.md]
**Status:** DONE

## [2026-04-17] jules-7135794475779878235-04d0b836
- **Feature:** Corrección de Auditoría 2026-04-17
- **Description:** Ejecución del proceso correccion-auditorias sin hallazgos (100% de salud).
- **Reference:** docs/features/correccion-auditoria-2026-04-17/objectives.md
- **Status:** Done

## [2026-04-18] feat/kaizen-audit-2026-04-18
**Feature:** Auditoría del Proyecto (2026-04-18)
**Description:** Ejecución de auditoría diaria estructural sin hallazgos de deuda técnica.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_18_audit-project.md]
**Status:** DONE
## [2026-04-18] feat/kaizen-auth-controller-xml-docs
**Feature:** Add missing XML docs to AuthController
**Description:** Added XML summary to AuthController constructor to comply with XML documentation standards.
**Reference:** [docs/TASKS/DONE/Kaizen_2026_04_18_auth_controller_xml_docs.md]
