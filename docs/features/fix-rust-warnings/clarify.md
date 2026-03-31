---
title: Clarify
created: 2024-03-28
---

# Aclaraciones

La eliminación del campo `skill_id` en las estructuras `JsonInput` de los binarios Rust NO es un cambio destructivo respecto al JSON entrante, ya que el deserializador `serde_json` ignorará dicho campo si no está presente en la definición de la estructura. El funcionamiento de las tools y skills no se verá afectado.
