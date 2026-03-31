---
title: Spec
created: 2024-03-28
---

# Especificación

Las estructuras `JsonInput` de los binarios Rust `iniciar_rama`, `invoke_command`, `merge_to_master_cleanup`, y `push_and_create_pr` dentro de `scripts/skills-rs` definen un campo opcional `skill_id` (vía `#[serde(rename = "skillId")]`) que no se utiliza en el código fuente, generando advertencias de *dead code* durante la compilación.

Para tener una compilación completamente libre de warnings (cero-warning policy) y reducir el ruido en CI/CD, estos campos sin uso deben ser eliminados de las definiciones del struct `JsonInput`.
