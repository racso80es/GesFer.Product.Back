# Principio Constitucional: Cimiento Arquitectónico

## Propósito
Establecer la estructura física y lógica fundamental del proyecto, asegurando escalabilidad, mantenibilidad y preparación para futuras integraciones.

## Definición
Este principio dicta la organización del código en capas claras y la adopción de patrones de diseño robustos como la Inyección de Dependencias.

## Directrices
1. **Jerarquía Física:** El código debe residir en `src/`, con proyectos claramente separados (Api, Application, Domain, Infrastructure).
2. **Inyección de Dependencias:** Todo componente debe ser desacoplado mediante un contenedor IoC (InversifyJS).
3. **Contratos:** Las interacciones externas deben definirse primero mediante interfaces (contratos), permitiendo implementaciones intercambiables (ej. IOTA/Blockchain).

## Estado Actual
- **En GesFer.Admin.Back:** Ubicación en `src/` (proyectos .NET). No aplica Interface/Desktop (repo backend).
- **DI:** Implementado con InversifyJS.
- **Contratos:** Interfaces en Domain/Application; convención del proyecto.
