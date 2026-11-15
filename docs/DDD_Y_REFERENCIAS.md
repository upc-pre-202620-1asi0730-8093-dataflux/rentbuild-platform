# DDD y correspondencia con Learning Center

Se usaron como referencia los archivos aportados por el usuario:

- maquigest-platform-develop.zip (Java/Spring Boot): SHA-256 `41c2d6bb4b6e02f351d16b7b08d6761e6e7cec02846c3753b8b4ca7d537029a0`.
- learning-center-platform-main.zip (.NET 10/C# 14): SHA-256 `4e4f9d22bcc112fdb159232dfaa532a48b4d8c4c514a0a0d8bd5aecb29ccc1bc`.
- Vue: entrega anterior preparada contra main `7a86d6d352eba49b70909480bc369a9827396acf` de `upc-pre-202620-1asi0730-8093-dataflux/rentbuild-webapp`.

| Estructura del profesor | Adaptación RentBuild |
| --- | --- |
| Acme.Center.Platform | RentBuild.Platform |
| Iam, Profiles, Publishing, Shared | Iam, Profiles, Inventory, Subscriptions, Rentals, Maintenance, Shared |
| Domain/Model/Aggregates y ValueObjects | User, CompanyProfile, Equipment, RentalRequest, Rental, UserSubscription, MaintenanceRecord, Incident; EmailAddress, RentalRate, RentalPeriod, CompanyAddress |
| Domain/Repositories | Contratos propios del contexto; ninguna dependencia EF |
| Application/CommandServices y QueryServices | Interfaces y implementaciones separadas para los seis contextos |
| Application/Internal/OutboundServices | Puertos de acceso a inventario, suscripciones, mantenimiento, identidad y perfiles |
| Interfaces/Acl y adaptadores | Facades públicas y traducción entre proyecciones de contextos |
| Infrastructure/Persistence/EntityFrameworkCore | Repositorios EF y AppDbContext; SQLite local y proveedor MySQL |
| Interfaces/Rest/Resources y Transform | Recursos HTTP y ensambladores; no se devuelven hashes de contraseña ni agregados completos |
| Result y ProblemDetails | Resultado de comandos y errores REST estructurados con código y estado |
| AggregateRoot, eventos y unidad de trabajo | Eventos acumulados por el agregado, persistidos en la misma transacción y registrados tras commit |

DDD se comprueba con reglas ejecutables, no solo con carpetas. Las pruebas de arquitectura rechazan ASP.NET/EF y referencias a otros contextos en Domain; también rechazan infraestructura o controladores en Application. Las pruebas de API verifican transacciones de aprobación y devolución, permisos y concurrencia.

Las decisiones operativas de esta adaptación son explícitas: los servicios de aplicación coordinan comandos directamente, sin depender de Cortex.Mediator; la unidad de trabajo maneja la transacción y los eventos; SQLite permite un arranque comprobable sin servidor de base de datos. MySQL queda disponible mediante configuración. El bootstrap de esquema se explica en README y no equivale a la estrategia de migraciones del ejemplo.

No se incluyen Publishing, tutoriales ni datos del ejemplo porque no pertenecen al dominio RentBuild. No se crea autoría ni historia Git de los profesores o integrantes; la división de commits se entrega como parches para revisión y confirmación por sus autores reales.
