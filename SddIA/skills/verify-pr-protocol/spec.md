---
dependencies:
- dotnet
- pwsh
- cargo
implementation_path: scripts/skills-rs/src/bin/verify_pr_protocol.rs
inputs: []
language: rust
outputs:
- description: 0 for success, non-zero for failure.
  name: Exit Code
  type: integer
security_context: Karma2Token
skill_id: verify-pr-protocol
---

# Skill: verify-pr-protocol

## Propósito
Esta skill ejecuta de manera atómica todos los pasos requeridos por el **Protocolo de Aceptación de PR** (ver `SddIA/norms/pr-acceptance-protocol.md`).

## Responsabilidad
- Validar la nomenclatura de la rama actual (invocando `scripts/validate-nomenclatura.ps1`).
- Compilar la solución (.NET).
- Ejecutar todos los tests de la solución.
- Retornar **éxito (0)** solo si TODOS los pasos son exitosos.

## Implementación
- **Lenguaje:** Rust.
- **Binario:** `verify_pr_protocol`.
- **Ruta Fuente:** `scripts/skills-rs/src/bin/verify_pr_protocol.rs`.

## Uso
```bash
# Desde la raíz del repositorio
cargo run --manifest-path scripts/skills-rs/Cargo.toml --bin verify_pr_protocol
```

## Salidas
- **Standard Output:** Logs detallados de cada fase (1/3, 2/3, 3/3).
- **Exit Code:** `0` (Éxito), `1` (Fallo).
