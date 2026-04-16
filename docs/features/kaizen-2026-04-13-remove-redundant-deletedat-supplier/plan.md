---
name: Remove redundant DeletedAt checks in Supplier handlers
created: 2026-04-13
type: feature
---
# Plan
Modify the 5 handler files using `replace_with_git_merge_diff` to remove `DeletedAt == null` checks, then verify code, run tests, and finalize task.