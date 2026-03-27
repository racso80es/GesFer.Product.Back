---
feature_name: sddia-evolution-sync-norma
created: 2026-03-27
plan_ref: docs/features/sddia-evolution-sync-norma/plan.md
spec_ref: docs/features/sddia-evolution-sync-norma/spec.md
items:
  - id: IMPL-P1
    phase: P1
    action: Verificar / completar contrato y Cúmulo
    dependencies: []
  - id: IMPL-P2
    phase: P2
    action: Rust register + validate + watch + install
    dependencies: [IMPL-P1]
  - id: IMPL-P3
    phase: P3
    action: GitHub Actions + README + PR template
    dependencies: [IMPL-P2]
  - id: IMPL-P4
    phase: P4
    action: Norma, constitución, .cursor/rules
    dependencies: [IMPL-P1]
  - id: IMPL-P5
    phase: P5
    action: validacion.md + execution.md
    dependencies: [IMPL-P2, IMPL-P3, IMPL-P4]
---

# Implementación — sddia-evolution-sync-norma

Documento de **touchpoints** para la fase **execution** (Tekton). Las rutas de persistencia normativas se resuelven vía **Cúmulo** (`paths.*`); aquí se listan rutas concretas como ubicación de edición.

**Orden sugerido (del plan):** P1 → P2.1+P2.4 → P2.2 → P2.5 → P4.4 (primer register) → P2.3 → P3 → P4.1–P4.3 → P5.

---

## P1 — Contrato y SSOT

| ID | Ubicación | Qué hacer |
| :--- | :--- | :--- |
| P1.1 | `SddIA/evolution/evolution_contract.md` | Ya en v1.1 con GUID y `tipo_operacion`. Revisar coherencia con `spec.md` 1.2-plan antes de merge. |
| P1.2 | `SddIA/agents/cumulo.paths.json` | Confirmar `sddiaEvolutionPath`, `sddiaEvolutionLogFile`, `sddiaEvolutionContractFile`. |
| P1.2b | `SddIA/norms/paths-via-cumulo.md` | Listar las tres claves `sddia*` si falta entrada explícita. |
| P1.3 | `docs/features/sddia-evolution-sync-norma/spec.md` | Mantener alineado con contrato v1.1 (sin drift). |

---

## P2 — Binarios Rust (`scripts/skills-rs`)

### Clasificación

- **Recomendación:** tratar **register** y **validate** como **skills** ejecutables (mismo patrón que `verify_pr_protocol`: stdin/stdout JSON, TTY opcional).
- **Watcher:** tercer binario `sddia_evolution_watch` o subcomando; puede vivir en el mismo crate con `[[bin]]` adicional.

### Archivos y crates

| ID | Archivo / artefacto | Propuesta |
| :--- | :--- | :--- |
| P2.1 | `scripts/skills-rs/Cargo.toml` | Añadir `[[bin]]` para `sddia_evolution_register`, `sddia_evolution_validate`, `sddia_evolution_watch`. Dependencia `uuid` (v4), `notify` (watcher). |
| P2.1b | `scripts/skills-rs/src/bin/sddia_evolution_register.rs` | Leer `gesfer_capsule` request: campos camelCase alineados al contrato YAML (autor, descripcionBreve, tipoOperacion, cambiosRealizados, rutasEliminadas, commitReferenciaPrevio opcionales). Generar UUID v4; escribir `SddIA/evolution/{uuid}.md` (frontmatter + cuerpo); insertar fila en `Evolution_log.md`; calcular SHA-256 del YAML canónico para `hash_integridad`. Repo root: detectar con `CARGO_MANIFEST_DIR` + `../../` o variable env. |
| P2.2 | `scripts/skills-rs/src/bin/sddia_evolution_validate.rs` | Args o JSON: `baseRef`, `headRef` (default `HEAD`). `git diff --name-only base..head`; si algún path bajo `SddIA/` (excl. `SddIA/evolution/*.md` según reglas), comprobar que exista línea nueva en índice o fichero detalle nuevo creado en el rango — regla mínima del plan; refinar en código. Salida JSON capsule en modo agente; exit ≠ 0 si falla. |
| P2.3 | `scripts/skills-rs/src/bin/sddia_evolution_watch.rs` | `notify` en `./SddIA` con debounce ~1.5s; ignorar `.git`, temp. Log a stderr o JSON línea: “cambio detectado → ejecutar register con…”. Sin escritura automática obligatoria en v1 si no hay metadatos. |
| P2.5 | `scripts/skills-rs/install.ps1` | Copiar los tres `.exe` a cápsula bajo `scripts/skills/sddia-evolution/` (crear carpeta) o nombre acordado; actualizar `scripts/skills/index.json` si el repo lo exige. |

### Definición SddIA (skill)

| ID | Ubicación | Qué hacer |
| :--- | :--- | :--- |
| P2.s | `SddIA/skills/sddia-evolution-register/spec.md` | skill_id, parámetros request, ejemplo envelope JSON. |
| P2.s2 | `scripts/skills/sddia-evolution/manifest.json` | `executable`: `sddia_evolution_register.exe` (y documentar validate/watch en mismo directorio o manifests hermanos). |
| P2.s3 | `SddIA/agents/cumulo.paths.json` | Añadir `skillCapsules.sddia-evolution-register` → `./scripts/skills/sddia-evolution/` (o rutas separadas por binario si se prefiere una cápsula por skill). |

**Nota:** Si se prefiere **una sola cápsula** `sddia-evolution` con tres `.exe`, un solo `skillCapsules` apunta a la carpeta y los manifests documentan cada binario.

---

## P3 — `.github`

| ID | Ubicación | Qué hacer |
| :--- | :--- | :--- |
| P3.1 | `.github/workflows/pr-validation.yml` | Tras pasos existentes o job condicional: si diff incluye `SddIA/**`, `cargo build` bin `sddia_evolution_validate` y ejecutar con `GITHUB_BASE_REF` / merge base del PR. Ubuntu: usar `git` del runner; pasar refs `origin/${{ github.base_ref }}` y `HEAD`. |
| P3.2 | `.github/README.md` | Sección “SddIA evolution”: enlace a norma, obligación para Jules/PRs. |
| P3.3 | `.github/PULL_REQUEST_TEMPLATE.md` | Checkbox: “Si el PR modifica `./SddIA/`, registro evolution aplicado (GUID + índice).” |

---

## P4 — Norma, constitución, Cursor

| ID | Ubicación | Qué hacer |
| :--- | :--- | :--- |
| P4.1 | `SddIA/norms/sddia-evolution-sync.md` | Sustituir cualquier mención a `SSDD-LOG-*` como id; fijar GUID; watcher local + CI. |
| P4.2 | `SddIA/constitution.json` | Nueva ley (p. ej. `L8_SDDIA_EVOLUTION`) o extensión: trazabilidad obligatoria con GUID. |
| P4.2b | `SddIA/CONSTITUTION.md` | Sección breve remitiendo a norma y `paths.sddiaEvolutionPath`. |
| P4.3 | `.cursor/rules/sddia-evolution-sync.mdc` | **Hecho en repo:** regla con `globs: SddIA/**/*`, enlaces SSOT, sin YAML duplicado; actualizar si cambia la norma (sddia-difusion). |
| P4.4 | `SddIA/evolution/Evolution_log.md` | Tras primer `register` OK: sustituir fila “pendiente” por GUID real + fecha + descripción. |

---

## P5 — Cierre documental

| ID | Ubicación | Qué hacer |
| :--- | :--- | :--- |
| P5.1 | `docs/features/sddia-evolution-sync-norma/validacion.md` | Checks listados en spec §9 + pruebas locales validate/register. |
| P5.2 | `docs/features/sddia-evolution-sync-norma/execution.md` | Registrar commits, archivos tocados, incidencias (post-ejecución). |

---

## Dependencias externas

- **Git:** disponible en PATH (local y en `ubuntu-latest` para validate en CI).
- **Rust:** stable; mismo toolchain que el resto de `skills-rs`.

---

## Criterio de “implementation.md” cumplido

Tekton puede ejecutar **execution.md** ítem a ítem siguiendo las tablas anteriores sin reinterpretar el plan: cada **ID** es trazable a un archivo o binario concreto.
