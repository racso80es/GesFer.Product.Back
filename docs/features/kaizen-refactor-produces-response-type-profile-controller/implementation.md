---
date: "2026-05-03"
---
# Implementación
Se utilizó el siguiente script de bash para realizar los reemplazos:
```bash
for FILE in $FILES; do
    sed -i 's/\[ProducesResponseType(StatusCodes.Status404NotFound)\]/\[ProducesResponseType<object>(StatusCodes.Status404NotFound)\]/g' "$FILE"
    sed -i 's/\[ProducesResponseType(StatusCodes.Status400BadRequest)\]/\[ProducesResponseType<object>(StatusCodes.Status400BadRequest)\]/g' "$FILE"
    sed -i 's/\[ProducesResponseType(StatusCodes.Status401Unauthorized)\]/\[ProducesResponseType<object>(StatusCodes.Status401Unauthorized)\]/g' "$FILE"
    sed -i 's/\[ProducesResponseType(StatusCodes.Status500InternalServerError)\]/\[ProducesResponseType<object>(StatusCodes.Status500InternalServerError)\]/g' "$FILE"
done
```
