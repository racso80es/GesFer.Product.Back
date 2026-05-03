---
feature_id: create-skill-git-close-cycle
artifact: implementation
status: draft
---

# Implementación — touchpoints

| Área | Archivo / elemento |
|------|---------------------|
| Rust | `scripts/skills-rs/src/bin/git_close_cycle.rs` |
| Build | `scripts/skills-rs/Cargo.toml` `[[bin]] git_close_cycle` |
| SSOT rutas | `SddIA/agents/cumulo.paths.json` → `skillCapsules.git-close-cycle` |
| Definición skill | `SddIA/skills/git-close-cycle/spec.md`, `spec.json` |
| Cápsula | `scripts/skills/git-close-cycle/*` |
| Índice skills | `scripts/skills/index.json` |
| Tekton | `scripts/skills/run-capsule-from-tekton-request.ps1` |
| Distribución exe | `scripts/skills-rs/install.ps1` |
| Acción | `SddIA/actions/finalize-process/spec.md` (frontmatter + cuerpo) |
| Normas | `SddIA/norms/interaction-triggers.md` |
| Catálogo | `SddIA/skills/README.md` |
| Proceso | `SddIA/process/feature/spec.md` `related_skills` |

## Contrato JSON

Entrada alineada con skills existentes: `targetBranch`, `mainBranch?`, `workingDirectory?`.
