---
date: "2026-04-23"
title: "Analyze and resolve SddIA Evolution Log bug"
---
# Spec
Bug report: SddIA/evolution/Evolution_log.md is modified incorrectly when changes happen outside of ./SddIA. General changes must be logged in docs/evolution/EVOLUTION_LOG.md.

Changes to make: Update Jules/AI instructions in `SddIA/agents/cumulo.instructions.json`, and ensure there are clear explicit distinctions in `SddIA/norms/sddia-evolution-sync.md`.
Also check for other references that might cause the AI to mistakenly use SddIA evolution for general product changes.
