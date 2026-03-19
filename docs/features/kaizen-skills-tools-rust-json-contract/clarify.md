---
feature_name: kaizen-skills-tools-rust-json-contract
created: '2026-03-19'
purpose: Clarificación con usuario antes de planificación
contract_ref: SddIA/norms/features-documentation-pattern.md
---

# Clarificación: Kaizen skills/tools Rust + JSON

Este documento recoge las **preguntas para el usuario** que deben resolverse antes de pasar a la fase de planificación (plan.md). Las decisiones se registrarán aquí.

---

## Preguntas abiertas

### 1. Contrato JSON para skills

**Contexto:** El contrato de tools (SddIA/tools/tools-contract.md) define explícitamente salida JSON (toolId, exitCode, success, timestamp, message, feedback). El contrato de skills no define un esquema JSON de entrada/salida.

**Pregunta:** ¿Deben las skills ejecutables tener un contrato JSON de entrada/salida formal (similar al de tools), o basta con que invoquen comandos y registren en execution_history.json sin un esquema estandarizado?

**Opciones:**
- **A)** Sí: definir skills-contract con esquema JSON entrada/salida (alineado a tools-contract).
- **B)** No: las skills pueden seguir con parámetros CLI y registro en execution_history; solo garantizar binario Rust.
- **C)** Parcial: salida JSON estandarizada, pero entrada puede ser solo argumentos CLI.

**Decisión del usuario:** **A)** Sí: definir skills-contract con esquema JSON entrada/salida (alineado a tools-contract).

---

### 2. Estrategia de eliminación de fallbacks .ps1

**Contexto:** Los contratos prohíben .ps1 como implementación principal. Actualmente muchas cápsulas tienen .bat que invoca .exe si existe, si no .ps1.

**Pregunta:** ¿Qué estrategia aplicar para los scripts .ps1 existentes?

**Opciones:**
- **A)** Eliminar .ps1 en cuanto exista .exe funcional; el .bat solo invoca .exe (si no hay .exe, error explícito).
- **B)** Mantener .ps1 como fallback documentado hasta que todos los consumidores (IA, CI, humanos) usen .exe.
- **C)** Eliminar .ps1 de forma gradual: por cada skill/tool completado, quitar su .ps1 en el mismo PR.

**Decisión del usuario:** **A)** Eliminar .ps1 en cuanto exista .exe funcional; el .bat solo invoca .exe. Siempre ha de existir .exe.

---

### 3. Prioridad de skills vs tools

**Contexto:** Hay skills y tools con binarios vacíos o inexistentes. El esfuerzo puede ser considerable.

**Pregunta:** ¿Cuál es la prioridad de implementación?

**Opciones:**
- **A)** Skills primero (iniciar-rama, invoke-command, invoke-commit, finalizar-git).
- **B)** Tools primero (run-tests-local, postman-mcp-validation sin binario).
- **C)** En paralelo: un skill y una tool por iteración.
- **D)** Solo los más críticos para el flujo de desarrollo (iniciar-rama, invoke-command, run-tests-local).

**Decisión del usuario:** **C)** En paralelo: skills y tools (ambos en el alcance del Kaizen).

---

### 4. Alcance de scripts fuera de cápsulas

**Contexto:** Existen scripts .ps1 en la raíz de scripts/ (validate-nomenclatura.ps1, Run-E2ELocal.ps1, run-service-with-log.ps1, etc.) que no están en paths.skillCapsules ni paths.toolCapsules.

**Pregunta:** ¿Este Kaizen incluye migrar esos scripts a tools/skills en Rust, o el alcance se limita a las entidades ya definidas en Cúmulo (skillCapsules, toolCapsules)?

**Opciones:**
- **A)** Solo skillCapsules y toolCapsules.
- **B)** Incluir scripts raíz que sean invocados por IA o CI (ej. validate-nomenclatura.ps1).
- **C)** Inventariar y decidir caso a caso en plan.md.

**Decisión del usuario:** **A)** Solo skillCapsules y toolCapsules.

---

### 5. Entrada JSON para skills/tools

**Contexto:** tools-contract define salida JSON. La entrada puede ser CLI (--param) o JSON por stdin/fichero.

**Pregunta:** ¿Las skills y tools deben aceptar entrada por JSON (stdin o --input-path) además de argumentos CLI, para facilitar invocación desde IA/MCP?

**Opciones:**
- **A)** Sí: soporte dual (CLI y JSON input).
- **B)** No: solo argumentos CLI; la IA construye la invocación desde la spec.
- **C)** Solo para tools; las skills pueden quedarse en CLI.

**Decisión del usuario:** **A)** Sí: soporte dual (CLI y JSON input) para invocación desde IA/MCP.

---

## Resumen de decisiones

| # | Tema                    | Decisión |
|---|-------------------------|----------|
| 1 | Contrato JSON skills    | Formal: esquema JSON entrada/salida alineado a tools-contract |
| 2 | Estrategia .ps1         | Eliminar .ps1; siempre ha de existir .exe; .bat solo invoca .exe |
| 3 | Prioridad skills/tools  | Ambos: skills y tools en paralelo |
| 4 | Alcance scripts raíz    | Solo skillCapsules y toolCapsules |
| 5 | Entrada JSON            | Sí: soporte dual CLI + JSON input |

---

*Decisiones registradas. Proceder a fase de planificación (plan.md).*
