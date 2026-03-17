---
commands:
  build_admin: Set-Location src/Admin/Front; npm run build
  build_product: Set-Location src/Product/Front; npm run build
contract_ref: paths.skillsDefinitionPath/skills-contract.md (Cúmulo)
owner: tekton-developer
related_agents:
- tekton-developer
- frontend-architect
rules:
- El script 'build' en package.json de Product y Admin debe apuntar a node scripts/next-build-fallback.cjs.
- 'Ejecutar desde la raíz del proyecto: Set-Location <cwd>; luego node scripts/next-build-fallback.cjs o npm run build.'
- 'Si MODULE_NOT_FOUND next/dist/bin/next persiste, reinstalar: Remove-Item -Recurse node_modules; npm install.'
scripts:
- cwd: src/Product/Front
  id: next-build-fallback-product
  name_short: Build Product
  npm_script: npm run build
- cwd: src/Admin/Front
  id: next-build-fallback-admin
  name_short: Build Admin
  npm_script: npm run build
skill_id: frontend-build
spec_version: 1.0.0
status: Active
---

# Skill: Frontend Build (Next.js)

**skill_id:** `frontend-build`

## Objetivo

Scripts y comandos para compilar los frontends Product y Admin sin depender del binario next (dist/bin/next). Usar cuando el build del frontend falle con MODULE_NOT_FOUND.

## Reglas

- El script 'build' en package.json de Product y Admin debe apuntar a node scripts/next-build-fallback.cjs.
- Ejecutar desde la raíz del proyecto: Set-Location <cwd>; luego node scripts/next-build-fallback.cjs o npm run build.
- Si MODULE_NOT_FOUND next/dist/bin/next persiste, reinstalar: Remove-Item -Recurse node_modules; npm install.

## Scripts

- next-build-fallback-product: Build Product (src/Product/Front)
- next-build-fallback-admin: Build Admin (src/Admin/Front)

## Comandos

- build_product: Set-Location src/Product/Front; npm run build
- build_admin: Set-Location src/Admin/Front; npm run build

---
*Definición en paths.skillsDefinitionPath/frontend-build/ (contrato paths.skillsDefinitionPath/skills-contract.md).*
