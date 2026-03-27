## Descripción

<!-- Breve descripción del cambio. -->

## Proceso y documentación

- **Rama:** <!-- feat/... o fix/... (nunca master) -->
- **Proceso:** <!-- feature | bug-fix | refactorization | create-tool -->
- **Documentación de la tarea:** <!-- Enlace a la carpeta en docs/features/<nombre> o docs/bugs/<nombre> según Cúmulo (paths.featurePath / paths.fixPath) -->

Las normas de trabajo (leyes GIT, procesos, rutas) están en **AGENTS.md** y **SddIA/** (única fuente de verdad). Ver [SddIA/norms/touchpoints-ia.md](../SddIA/norms/touchpoints-ia.md).

## Checklist

- [ ] El trabajo está en rama `feat/` o `fix/`, no en `master`.
- [ ] Documentación de la tarea en la carpeta correspondiente (objectives, spec, validación si aplica).
- [ ] Build y tests pasan (Ley COMPILACIÓN).
- [ ] Si el PR modifica `./SddIA/`, registro evolution aplicado (detalle `SddIA/evolution/<uuid>.md` + índice), salvo lo excluido por la norma — ver [SddIA/norms/sddia-evolution-sync.md](../SddIA/norms/sddia-evolution-sync.md).
