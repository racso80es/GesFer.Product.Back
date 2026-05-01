---
feature_id: create-skill-git-close-cycle
artifact: plan
status: draft
---

# Plan

| Paso | Entregable |
|------|------------|
| P0 | Rama `feat/create-skill-git-close-cycle` vía cápsulas Git (Tekton) |
| P1 | `SddIA/skills/git-close-cycle/spec.md`, `spec.json` |
| P2 | `scripts/skills-rs/src/bin/git_close_cycle.rs` + `Cargo.toml` |
| P3 | Cápsula `scripts/skills/git-close-cycle/` (manifest, bat, doc) |
| P4 | `cumulo.paths.json` → `skillCapsules.git-close-cycle` |
| P5 | `scripts/skills/index.json`, `run-capsule-from-tekton-request.ps1`, `install.ps1` |
| P6 | `SddIA/actions/finalize-process/spec.md` — directriz de orquestación |
| P7 | Difusión: `interaction-triggers.md`, `SddIA/skills/README.md`, proceso `feature` related_skills |
| P8 | Evaluación impacto SddIA → `sddia-evolution-register` + snapshots; sync + PR |
