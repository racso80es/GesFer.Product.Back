---
type: validation
---
# Validación
Los tests se estabilizaron parcialmente respecto al uso concurrente de HttpClient, pero fallan en la etapa de autenticación porque `admin123` recibe 401 dado que el `CompanyId` ahora debe provenir y validarse correctamente con la API de Back. Esto se ha marcado como TODO.