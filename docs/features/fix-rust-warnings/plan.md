---
title: Plan
created: 2024-03-28
---

# Plan

1. Localizar los binarios afectados en `scripts/skills-rs/src/bin/`:
    * `iniciar_rama.rs`
    * `invoke_command.rs`
    * `merge_to_master_cleanup.rs`
    * `push_and_create_pr.rs`
2. Remover el campo `skill_id` (y la decoración `#[serde(rename = "skillId")]`) de cada struct `JsonInput`.
3. Compilar el proyecto con `cargo build --bins` y confirmar la desaparición de los warnings de *dead code*.
