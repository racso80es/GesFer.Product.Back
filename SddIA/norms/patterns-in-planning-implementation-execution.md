# Norma: Aplicación de patrones en planificación, implementación y ejecución

**Fuente:** SddIA/norms. Contrato de patrones: SddIA/patterns/patterns-contract.json. Rutas: Cúmulo paths.patternsPath.

## Objetivo

Definir **dónde y cómo** se aplican los patrones de diseño (paths.patternsPath) en las tres fases del ciclo de feature que tocan diseño y código: **planning**, **implementation** y **execution**.

---

## 1. Fase de planificación (planning)

**Acción:** planning. **Salida:** `{SpecName}_PLAN.md` (hoja de ruta técnica ejecutable).

### Dónde aplicar patrones

| Momento en el flujo | Aplicación de patrones |
|---------------------|-------------------------|
| **Análisis de requisitos** | Identificar decisiones arquitectónicas que correspondan a un patrón existente en paths.patternsPath (ej. capa de persistencia → Repository; integración con legacy → ACL; comandos/eventos → CQRS o Domain Events; resiliencia → Failover/Outbox). |
| **Generación del plan** | Incluir en el PLAN referencias a patrones cuando una tarea o fase consista en *aplicar* o *respetar* un patrón. Formato sugerido: `[PATTERN-<uuid>]` o sección "Patrones aplicados" con lista de pattern id (UUID) y título. |
| **Fases / tareas técnicas** | Si el plan descompone en fases (ej. "Capas", "Handlers", "Persistencia"), asociar cada fase a los patrones que la rigen (ej. fase Persistencia → Repository, DAO; fase Mensajería → Outbox, Domain Events). |

### Criterios

- **Consultar paths.patternsPath:** El agente o proceso que genera el plan (documentación manual; comandos vía invoke-command, paths.skillCapsules["invoke-command"]) debe poder resolver la lista de patrones (spec.json por carpeta UUID) para sugerir o validar referencias.
- **interested_agents:** Los patrones declaran `interested_agents` (architect, tekton-developer, etc.); en planificación el **Arquitecto** selecciona qué patrones aplican al roadmap; **Tekton** los usa en implementación/ejecución.
- **No inventar patrones en el plan:** Las referencias en el PLAN deben ser a patrones existentes en paths.patternsPath (UUID con spec.md con frontmatter YAML conforme a patterns-contract).

### Ejemplo (fragmento PLAN)

```markdown
## Patrones aplicados
- e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a03: Repository (capas de acceso a datos)
- e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a01: Arquitectura Hexagonal (puertos en dominio, adaptadores en infra)

## Fase 2 – Handlers y aplicación
- [PATTERN-e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a06] Nuevos command handlers siguiendo patrón Command.
```

---

## 2. Fase de implementación (implementation)

**Acción:** implementation. **Salida:** documento IMPL (touchpoints: Id, Acción, Ruta, Ubicación, Propuesta, Dependencias).

### Dónde aplicar patrones

| Momento en el flujo | Aplicación de patrones |
|---------------------|-------------------------|
| **Análisis del plan** | Al derivar touchpoints de cada tarea del PLAN, si la tarea referencia un patrón (por UUID o etiqueta [PATTERN-<uuid>]), el ítem de implementación debe incluir el **pattern_id** (UUID) asociado. |
| **Estructura del documento IMPL** | Cada ítem puede tener un campo opcional **Patrón** (o `pattern_id`) con el UUID del patrón que rige ese cambio. Así el ejecutor y el revisor saben qué patrón debe cumplirse en ese punto. |
| **Resolución de rutas** | Los patrones pueden imponer restricciones de ubicación (ej. Hexagonal: puertos en dominio, adaptadores en infra); la acción implementation puede validar que las rutas propuestas sean coherentes con el patrón referenciado (consultando spec.md con frontmatter del patrón). |

### Criterios

- **Trazabilidad PLAN → IMPL:** Si el PLAN incluye "Patrones aplicados" o etiquetas [PATTERN-<uuid>], los ítems del IMPL que materializan esas tareas deben llevar el pattern_id correspondiente.
- **Unificación:** El documento de implementación sigue siendo una sola fuente de verdad; el campo patrón es *metadato* del ítem para guiar ejecución y revisión.
- **Contrato patterns-contract:** Cualquier pattern_id usado en IMPL debe existir en paths.patternsPath/<uuid>/ con spec.md (frontmatter YAML) válido.

### Ejemplo (ítem IMPL con patrón)

```markdown
### 2.4 – Crear: Repository de ArticleFamily
- **Id:** 2.4
- **Patrón:** e98e4d2a-1c3f-4e56-9a2b-8f7d6c5b4a03 (Repository)
- **Acción:** Crear
- **Ruta:** `src/Product/Back/domain/Persistence/IArticleFamilyRepository.cs`
- **Ubicación:** Nuevo archivo interfaz.
- **Propuesta:** Definir interfaz con GetById, List(Specification) según spec del patrón Repository.
```

---

## 3. Fase de ejecución (execution)

**Acción:** execution. **Entrada:** implementation.json o IMPL. **Salida:** execution.json (registro por ítem: id, ruta, acción, estado, mensaje).

### Dónde aplicar patrones

| Momento en el flujo | Aplicación de patrones |
|---------------------|-------------------------|
| **Carga de ítems** | Si un ítem tiene pattern_id, el ejecutor (Tekton) debe tener acceso a paths.patternsPath/<uuid>/ (spec.md o spec.json) para conocer las reglas del patrón al aplicar la propuesta. |
| **Por cada ítem** | Al aplicar la propuesta (Crear/Modificar/Eliminar), respetar las restricciones del patrón cuando el ítem esté etiquetado con pattern_id (ej. no poner lógica de infra en carpeta de dominio si el patrón es Hexagonal). |
| **Registro execution.json** | Incluir en cada entrada opcionalmente el pattern_id que regía el ítem, para trazabilidad y para que validate/QA comprueben conformidad con el patrón. |

### Criterios

- **Orden y dependencias:** Siguen siendo los del IMPL; el patrón no cambia el orden, pero sí la *forma* de implementar (respetar estructura y convenciones del patrón).
- **interested_agents:** El agente que ejecuta (Tekton) está en interested_agents de la mayoría de patrones de diseño; debe aplicar el código conforme al spec del patrón cuando pattern_id esté presente.
- **Validación posterior:** La acción validate (o QA Judge) puede usar execution.json + pattern_id para comprobar que el código generado cumple el patrón (review manual o reglas derivadas del spec del patrón).

---

## 4. Resumen por fase

| Fase | Uso de patrones |
|------|-----------------|
| **Planning** | Selección de patrones aplicables; referencias en el PLAN (patrones aplicados, etiquetas por tarea/fase). Consulta paths.patternsPath para listado y spec. |
| **Implementation** | Asociar cada ítem IMPL a un pattern_id cuando el cambio instancie un patrón; opcionalmente validar rutas/estructura frente al patrón. |
| **Execution** | Aplicar cada ítem respetando el spec del patrón si tiene pattern_id; registrar pattern_id en execution.json para trazabilidad. |

---

## 5. Contrato y rutas

- **Contrato:** SddIA/patterns/patterns-contract.json (security_model: Karma2Token; required_files: spec.md, spec.json por patrón).
- **Ruta canónica:** Cúmulo paths.patternsPath (paths.patternsPath/<uuid>/ para cada patrón).
- **Acciones que referencian patrones:** planning, implementation, execution (campo opcional patterns_ref: paths.patternsPath en sus spec.json).

Referencia cruzada: principios en paths.principlesPath (agents.principles) guían *cómo* pensar el diseño; patrones en paths.patternsPath guían *qué* estructuras y convenciones aplicar en plan, IMPL y código.
