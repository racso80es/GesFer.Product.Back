---
created: 2026-03-29
priority: medium
---
# Kaizen: Remove TODOs from SddIA Templates

## Objective
Remove the explicit `// TODO:` comments in `SddIA/templates/spec-template.md` to adhere to the `dotnet-development` skill rule: `Clean Code: No 'TODO', no commented-out code.` Since this is a template, it should use placeholders like `// [ACTION-REQUIRED]` or `// [PENDING-TASK]` instead of `TODO` which triggers false positives in codebase audits.

## Scope
- Modify `SddIA/templates/spec-template.md`
