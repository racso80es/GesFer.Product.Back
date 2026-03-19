---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
purpose: Clarificación de los 4 puntos del SPEC para decisión del usuario
spec_ref: docs/features/fix-namespaces-remove-shared/spec.md
---

# Clarificaciones — Consulta con usuario

Este documento recoge las **preguntas de clarificación** sobre los 4 puntos del SPEC. Se requiere respuesta del usuario para proceder con planning e implementación.

---

## Punto 1: Resolución de conflictos de merge

**Contexto:** Hay 7 archivos en conflicto. En todos, la versión de `main` tiene el refactor completo (`GesFer.Product.Back.*`); la rama actual tiene usings antiguos en algunos bloques.

**Pregunta:** ¿Confirmas que en la resolución de conflictos debemos **tomar siempre la versión de main** en los bloques de usings (adoptar `GesFer.Product.Back.*` y descartar `GesFer.Infrastructure.*` / `GesFer.Domain.*`)?

- [x] Sí, adoptar versión de main en usings
- [ ] No, prefiero otra estrategia: _______________

---

## Punto 2: Completar refactor de referencias residuales

**Contexto:** Quedan referencias literales a namespaces antiguos en código (p. ej. `GesFer.Infrastructure.Services.MasterDataSeeder`, `GesFer.Domain.Services.SensitiveDataSanitizer`).

**Pregunta:** ¿El alcance incluye **todos** los archivos de código C# (src, tests, IntegrationTests), o hay alguna excepción (p. ej. scripts, Dockerfiles, .csproj) que debamos dejar fuera de este refactor?

- [x] Incluir todo el código C# (src, tests, IntegrationTests)
- [ ] Excluir: _______________
- [x] Incluir también .csproj, Dockerfile, scripts

---

## Punto 3: Eliminar usings duplicados

**Contexto:** `SetupService.cs` y `CustomerConfiguration.cs` tienen `using GesFer.Product.Back.Domain.Entities;` duplicado.

**Pregunta:** ¿Confirmas que debemos eliminar **todos** los usings duplicados detectados, sin excepción?

- [x] Sí, eliminar todos los duplicados
- [ ] No, mantener en: _______________

---

## Punto 4: Validación post-cambios

**Contexto:** Tras resolver conflictos y aplicar cambios, se ejecutará build y tests.

**Pregunta:** ¿Qué nivel de validación requieres antes de considerar la tarea cerrada?

- [x] Solo `dotnet build` + tests existentes (por ahora)
- [ ] Incluir también: _______________ (ej. linter, análisis estático, despliegue en entorno de prueba)
- [ ] Otro: _______________

---

## Respuestas del usuario

*(Decisiones incorporadas al plan.)*

| Punto | Decisión | Notas |
|-------|----------|-------|
| 1 | Sí | Adoptar versión de main en usings |
| 2 | Todo incluido | Código C#, .csproj, Dockerfile, scripts |
| 3 | Sí | Eliminar todos los usings duplicados |
| 4 | Build + tests existentes | Por ahora; añadido al checklist como el resto |

---

*Documento generado por acción clarify. Ref: SddIA/actions/clarify/spec.md*
