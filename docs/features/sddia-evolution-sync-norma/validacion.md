---
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
status: completada
---

# Validación — sddia-evolution-sync-norma

## Checks

- [x] `paths.sddiaEvolutionPath`, `paths.sddiaEvolutionLogFile`, `paths.sddiaEvolutionContractFile` en Cúmulo; documentado en `paths-via-cumulo.md`.
- [x] `SddIA/evolution/evolution_contract.md` y `Evolution_log.md` presentes; primer registro vía `sddia_evolution_register`.
- [x] Norma `sddia-evolution-sync.md`; ley `L8_SDDIA_EVOLUTION` en `constitution.json`; sección en `CONSTITUTION.md`.
- [x] Binarios Rust: register, validate, watch; `install.ps1` copia a `scripts/skills/sddia-evolution/`.
- [x] Workflow PR: paso `sddia_evolution_validate` cuando el diff toca `SddIA/`; README y plantilla PR actualizados.
- [x] `.cursor/rules/sddia-evolution-sync.mdc` con globs `SddIA/**/*`.

## Evidencia local

- `cargo build --release` en `scripts/skills-rs` sin errores.
- `sddia_evolution_validate --base origin/main --head HEAD` con cambios solo en detalle UUID: omitido / OK según diff.
