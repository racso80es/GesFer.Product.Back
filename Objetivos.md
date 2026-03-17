# Objetivos del proyecto GesFer.Admin.Back

## Descripción del contexto

Este repositorio corresponde al **backend (API)** del ecosistema GesFer, que ha sido **aislado** como proyecto independiente. Anteriormente formaba parte de un monorepo o solución que incluía otras piezas (frontend, otros servicios, etc.). En su estado actual, el proyecto contiene únicamente la API de administración y sus dependencias directas.

## Alcance actual

- **API REST** para gestión (administración) del dominio GesFer.
- Stack: **.NET 8**, ASP.NET Core, autenticación **JWT**, **Entity Framework Core**, **Serilog**, **Swagger/OpenAPI**.
- Estructura en capas: **Api** → **Application** → **Infrastructure** → **Domain**, con tests unitarios e integración y scripts de soporte (base de datos, hashes).

## Objetivos del proyecto (documentación viva)

1. **Mantener una única fuente de verdad**  
   La documentación en este repositorio debe referirse solo al contexto actual: backend/API aislado, sin asumir otras piezas del ecosistema salvo que se indique explícitamente.

2. **README y Objetivos al día**  
   El README debe describir qué es este proyecto, cómo ejecutarlo y cómo contribuir, sin referencias obsoletas a monorepos o componentes que ya no forman parte del repo.

3. **Claridad para nuevos desarrolladores**  
   Cualquier persona que clone el repo debe entender de inmediato que es el backend/API de GesFer Admin y qué puede hacer con él (ejecutar, testear, desplegar).

4. **Alineación con el protocolo del proyecto**  
   Respetar las Leyes Universales y el protocolo multi-agente definidos en `AGENTS.md` (entorno Windows/PowerShell, sin commits a `master`, compilación verificada, etc.).

---

*Este documento se actualizará cuando cambien los objetivos o el alcance del proyecto.*
