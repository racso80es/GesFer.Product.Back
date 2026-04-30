---
title: Objectives
---
# Objectives
Refactor all instances of `[ProducesResponseType(typeof(T), statusCode)]` in `src/Api/Controllers/*.cs` to use the modern generic syntax `[ProducesResponseType<T>(statusCode)]`.
