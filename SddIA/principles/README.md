# Principios técnicos SddIA

Este directorio es **paths.principlesPath** (Cúmulo, `SddIA/agents/cumulo.json`). Contiene principios técnicos que deben ser tenidos en cuenta por **Arquitecto** y **Tekton** al realizar tareas de diseño e implementación.

## Estructura

| Ubicación | Contenido |
|-----------|-----------|
| **SddIA/principles/** (este directorio) | Contrato global: `principles-contract.json`, `principles-contract.md`. Por principio: subcarpeta **&lt;principle-id&gt;/** con `spec.md` y `spec.json`. |

Solo definición; no hay implementación ejecutable. Rutas canónicas vía Cúmulo (**paths.principlesPath**).

## Contrato

Cada principio debe tener en su carpeta **spec.md** (contenido legible, aplicación para arquitectos/tekton) y **spec.json** (id, principle_id, title, category, tags, metadata). Ver `principles-contract.md` y `principles-contract.json`.

## Listado de principios (paths.principlesPath/<principle-id>/)

| principle_id | Título | Categoría |
| :--- | :--- | :--- |
| regla-del-boy-scout | Regla del Boy Scout | Clean Code |
| preparatory-refactoring | Preparatory Refactoring | Clean Code |
| split-phase-refactoring | Split Phase Refactoring | Clean Code |
| clausulas-de-guarda-early-return | Cláusulas de Guarda (Early Return) | Clean Code |
| evitar-magic-numbers | Evitar Magic Numbers | Clean Code |
| single-responsibility-principle-srp | Single Responsibility Principle (SRP) | Principios SOLID |
| open-closed-principle-ocp | Open/Closed Principle (OCP) | Principios SOLID |
| dependency-inversion-principle-dip | Dependency Inversion Principle (DIP) | Principios SOLID |
| bounded-contexts | Bounded Contexts | Domain-Driven Design |
| value-objects | Value Objects | Domain-Driven Design |
| aggregate-roots | Aggregate Roots | Domain-Driven Design |
| eventos-de-dominio-domain-events | Eventos de Dominio (Domain Events) | Domain-Driven Design |
| ley-de-demeter-tell-dont-ask | Ley de Demeter (Tell, Don't Ask) | Clean Code |
| manejo-de-errores-tipado-either-result | Manejo de Errores Tipado (Either / Result) | Arquitectura de Software |
| read-models-y-proyecciones | Read Models y Proyecciones | Arquitectura de Software |
| peligro-del-modulo-shared | El Peligro del Módulo Shared | Arquitectura de Software |
| identificadores-uuids-desde-el-cliente | Identificadores UUIDs desde el Cliente | Arquitectura de Software |
| caso-de-uso-como-unidad-de-testing | Caso de Uso como Unidad de Testing | Testing |
| test-first-vs-tdd | Test-First vs TDD | Testing |
| base-de-datos-como-cola-de-mensajeria | Base de Datos como Cola de Mensajería | Sistemas Distribuidos |
| nomenclatura | Norma de Nomenclatura (Cúmulo) | Normas SddIA (bloqueante para PR) |

## Referencias

- Cúmulo: `SddIA/agents/cumulo.json` → **paths.principlesPath**.
