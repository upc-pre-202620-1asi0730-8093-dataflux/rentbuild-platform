# Contratos REST y cambios de integración

Todos los recursos de negocio usan `/api/v1` y JWT Bearer. Las colecciones mantienen los arrays y nombres camelCase que consume Vue.

| Contexto | Operaciones |
| --- | --- |
| IAM | POST authentication/sign-up, sign-in, password-recovery, reset-password; GET authentication/me |
| Profiles | GET profiles, profiles/{id}; POST profiles; PUT profiles/{id} |
| Inventory | GET equipment, equipment/{id}, equipment-categories; POST equipment; PATCH equipment/{id}; DELETE equipment/{id}; GET equipment/{id}/availability; POST equipment/{id}/availability-blocks; PATCH equipment/{id}/operational-status |
| Subscriptions | GET subscription-plans, user-subscriptions, user-subscriptions/{id}; POST user-subscriptions; PUT user-subscriptions/{id}; PATCH user-subscriptions/{id}/cancel |
| Rentals | GET/POST rental-requests, GET rental-requests/{id}, PUT rental-requests/{id}, PATCH rental-requests/{id}/period; GET rentals y rentals/{id}; GET/POST deliveries y equipment-returns |
| Maintenance | GET/POST maintenances, GET/PATCH maintenances/{id}, GET maintenances/{id}/activities, GET maintenances/equipment/{id}/history; GET/POST incidents, GET/PATCH incidents/{id}, POST incidents/equipment/{id}/reactivate |

POST rentals devuelve conflicto: un alquiler se crea únicamente al aprobar una solicitud. PUT rentals/{id} solo acepta el recurso idéntico como compatibilidad; para cambios de ciclo se usan deliveries o equipment-returns.

## Transacciones de negocio

- Registro: usuario y perfil se guardan juntos; un perfil inválido revierte la creación del usuario.
- Aprobación: valida propietario, plan, incidencias, disponibilidad y solapamientos; guarda decisión, reserva y alquiler en una transacción. Se serializan comandos en una instancia y se usa aislamiento Serializable en la base. Una prueba concurrente confirma una aprobación y rechaza la otra para el mismo período.
- Entrega: cambia alquiler e inventario junto con el comprobante.
- Devolución: completa el alquiler, libera exactamente su reserva, actualiza el equipo y crea mantenimiento de inspección si corresponde, dentro de una transacción.
- Mantenimiento: completar usa el mismo ID y crea una actividad de seguimiento, con técnico y observaciones. Los trabajos programados no avanzan antes de su fecha.
- Reactivación: requiere inspección, ausencia de incidentes bloqueantes, trabajos pendientes y alquileres activos. Resolver un incidente no reactiva automáticamente el equipo.
- Suscripción: las fechas se calculan en el servidor; cambiar plan conserva período e ID, cancelar revoca vigencia.

## Adaptación necesaria en Vue

En modo backend real (`VITE_USE_FAKE_API=false`) la aprobación llama solo a PUT rental-requests/{id}. Las entregas y devoluciones llaman a su endpoint y vuelven a leer el alquiler; Vue deja de crear un alquiler adicional o modificar directamente el estado del equipo. El modo mock conserva la demostración anterior.

La entrega incluye la vista de cambio de contraseña a la que apunta el correo de recuperación. La configuración de CORS admite 127.0.0.1:5173 y localhost:5173; el servidor de prueba registra su puerto 5190 de forma independiente.

Los perfiles ajenos se proyectan como directorio de empresas, ocultando correo, teléfono y dirección; las modificaciones siguen verificando propietario. Los endpoints de historial de equipo son solo del propietario. Las consultas de restricciones por equipo ocultan el texto de incidentes a empresas constructoras.

## Alcance de verificación

28 pruebas .NET (dominio, arquitectura y API); 114 pruebas Vue; 3 escenarios de navegador de la demostración anterior y 1 escenario de extremo a extremo contra .NET/SQLite. El esquema MySQL se generó con el proveedor configurado, sin servidor MySQL. El envío SMTP se reemplazó por un emisor de prueba; faltan credenciales para probar un proveedor real.
