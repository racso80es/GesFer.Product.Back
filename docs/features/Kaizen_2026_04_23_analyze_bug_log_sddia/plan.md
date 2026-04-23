---
date: "2026-04-23"
title: "Plan for SddIA Evolution Log bug"
---
# Plan
1. Analyze where AI instructions reference SddIA evolution log instead of the general evolution log.
2. In `SddIA/agents/cumulo.instructions.json`, update `"Map [EVO] -> paths.evolutionPath (paths.evolutionLogFile para EVOLUTION_LOG.md)",` to be extremely explicitly distinct from `SddIA/evolution/Evolution_log.md` and explicitly forbid using `SddIA/evolution` for product features.
3. In `SddIA/norms/sddia-evolution-sync.md`, strengthen the wording to explicitly forbid logging general product evolution in SddIA log.
4. Execute changes and run tests as sanity check.
5. Create SddIA evolution record for these modifications to `SddIA/` folder.
