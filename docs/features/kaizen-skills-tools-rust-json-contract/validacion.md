---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
execution_ref: docs/features/kaizen-skills-tools-rust-json-contract/execution.md
checks:
  - id: 6.1
    status: passed
    description: Verificar .exe en cada cápsula
  - id: 6.2
    status: passed
    description: Verificar salida JSON (skillId/toolId, exitCode, success, message, feedback, timestamp)
---

# Validación pre-PR: Kaizen skills/tools Rust + JSON

## 6.1 Verificación de .exe

| Cápsula | .exe | Estado |
|---------|------|--------|
| iniciar-rama | bin/iniciar_rama.exe | OK |
| invoke-command | bin/invoke_command.exe | OK |
| invoke-commit | bin/invoke_commit.exe | OK |
| finalizar-git | bin/merge_to_master_cleanup.exe, bin/push_and_create_pr.exe | OK |
| run-tests-local | run_tests_local.exe | OK |
| postman-mcp-validation | postman_mcp_validation.exe | OK |
| prepare-full-env | prepare_full_env.exe | OK |
| invoke-mysql-seeds | invoke_mysql_seeds.exe | OK |
| start-api | start_api.exe | OK |

## 6.2 Verificación de salida JSON

**invoke_command** (--command "echo ok" --output-json):
- Campos: skillId, exitCode, success, timestamp, message, feedback[], data, duration_ms
- Resultado: OK

**run_tests_local** (--test-scope unit --output-json):
- Campos: toolId, exitCode, success, timestamp, message, feedback[], data, duration_ms
- Resultado: OK (tests unit ejecutados correctamente)

## Conclusión

Validación pre-PR superada. Listo para push y PR.
