---
feature_name: fix-namespaces-remove-shared
created: '2026-03-19'
process: feature
branch: feature/fix-namespaces-remove-shared-14188559524536972040
---

# Objetivos

## Objetivo
Resolver conflictos de merge con `main` y completar el refactor de namespaces en la rama `feature/fix-namespaces-remove-shared-*`, eliminando dependencias de Shared y estandarizando a `GesFer.Product.Back.*`.

## Alcance
- Rama actual: `feature/fix-namespaces-remove-shared-14188559524536972040`
- 7 archivos en conflicto con `origin/main`
- Referencias residuales a namespaces antiguos en código
- Cambios de Shared → Common (SharedSecret → InternalSecret, ConfigureSharedEntities → ConfigureCommonEntities)

## Ley aplicada
- **Ley GIT:** No commits en master; trabajo en rama feat/fix con documentación en paths.featurePath.
- **Ley COMPILACIÓN:** Código roto inaceptable; verificar localmente tras cambios.
