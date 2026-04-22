Actúa como un Arquitecto de Software ejecutando una tarea de auditoría asíncrona. Tu objetivo es analizar en profundidad el código base actual de este proyecto (api-product) y extraer la especificación técnica completa de la entidad "User".

Esta información es el paso previo para migrar la responsabilidad de la entidad a un microservicio de Administración. 

Tu entregable no es un resumen conversacional, sino la generación de un archivo de documentación formal (ej. `especificacion_entidad_user_legacy.md`) que debe ser guardado en el directorio de documentación del proyecto. 

El documento generado debe estructurarse con los siguientes vectores obligatorios:

1. Estructura de Datos (Tablas y Modelos):
   - Definición exacta del esquema de base de datos actual para el usuario (campos, tipos de datos, constraints, índices, valores por defecto).
   - Relaciones existentes con otras entidades dentro de api-product.

2. Semillas (Seeds):
   - Estructura y datos exactos utilizados en los scripts de seeding actuales para la entidad User (usuarios maestros, administradores por defecto, etc.).

3. Lógica de Negocio y Casos de Uso (CRUD + Extras):
   - Reglas de negocio aplicadas actualmente en la creación, modificación, eliminación y consulta de usuarios.
   - Validaciones específicas, encriptación, o eventos desencadenados.

4. Interfaces y DTOs:
   - Código o definición estructural de todos los Data Transfer Objects (DTOs), interfaces o tipos asociados a los flujos de entrada y salida de User.

5. Endpoints (Contratos de API):
   - Mapeo completo de las rutas expuestas actualmente.
   - Para cada endpoint: Método HTTP, Ruta, Parámetros requeridos, DTO de entrada y DTO de respuesta. (Fundamental para mantener la compatibilidad futura).

6. Código Legacy / Acoplamiento Oculto:
   - Listado de dependencias, servicios o utilidades dentro de api-product que estén fuertemente acoplados a la gestión actual del usuario y que requieran refactorización en la fase de limpieza.

Ejecuta el análisis y guarda el documento en ./docs/DocumentacionUsuarios.md
