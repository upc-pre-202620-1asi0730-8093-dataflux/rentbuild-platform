# Maintenance

Integrante 4: mantenimiento, incidencias e historial.

Esta rama contiene los 24 archivos del avance original del integrante 4, más el registro del módulo y sus pruebas. Incluye programación y cierre de trabajos, incidencias, historial, repositorios y adaptadores de integración.

MaintenanceModule registra los servicios y configura las tablas mediante IPlatformModule. Cada contexto mantiene sus reglas en Domain, sus casos de uso en Application y HTTP en Interfaces.

La rama compila y arranca con la base común. IAM todavía debe integrar la emisión de tokens y la validación del estado de cuenta. Las operaciones que necesitan Inventory, Rentals o Subscriptions no simulan datos: sus puertos provisionales responden 501 CONTEXT_PENDING hasta integrar las implementaciones de los compañeros. Los controladores conservan su autorización.
