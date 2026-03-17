# Norma: Obediencia a Procesos SddIA

**Tipo:** Norma / Comportamiento
**UUID:** 4c04f9ab-cf64-44b4-8db9-8d7d91e8432b
**Versión:** 1.0
**Seguridad:** Karma2Token

## Propósito

Establecer que las instrucciones y directivas dadas a través de los procesos (paths.processPath) y normas de la arquitectura SddIA deben ser asumidas como válidas, ciertas y definitivas por cualquier Agente de Inteligencia Artificial que opere sobre el repositorio.

## Descripción del Comportamiento

1. **Ausencia de Validación Requerida:** No es necesario validar las indicaciones dadas por el proceso. La IA asume la instrucción como definitiva y directa.
2. **Ejecución Literal:** Simplemente seguir las instrucciones proporcionadas por el proceso y ejecutar los pasos.
3. **Minimización de Consultas al Usuario:** La IA no debe realizar pausas innecesarias para solicitar al usuario su aprobación sobre la validez de un proceso estándar a menos que se trate de una ambigüedad que impida físicamente continuar con una operación de estado crítico (ej: borrado de la base de datos de producción).

Esta norma asegura que toda IA, operando bajo la estructura SddIA, cumpla este comportamiento y mantenga un flujo continuo de ejecución de tareas y automatización, priorizando la ejecución por encima de la ratificación redundante.
