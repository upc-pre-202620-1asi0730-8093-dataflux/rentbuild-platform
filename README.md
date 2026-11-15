# RentBuild Platform

Base inicial para los cinco integrantes. ASP.NET Core / .NET 10, arquitectura por contextos con Domain, Application, Infrastructure e Interfaces.

## Clonar y ejecutar

```powershell
git clone -b develop https://github.com/upc-pre-202620-1asi0730-8093-dataflux/rentbuild-platform.git
cd rentbuild-platform
dotnet restore
dotnet build
dotnet test
dotnet run --project RentBuild.Platform
```

API de arranque: http://127.0.0.1:5080/health. Documentación: http://127.0.0.1:5080/scalar/v1.

La base arranca sin completar funcionalidades de los demás integrantes. No contiene cuentas, catálogos ni lógica comercial terminada. SQLite está disponible para desarrollo y MySQL mediante Database__Provider y ConnectionStrings__DefaultConnection. No se suben bases, contraseñas ni claves locales.

## Ramas

- main: permanece sin archivos.
- develop: base común que todos clonan.
- feature/maintenance: la misma base más el avance del integrante 4.

Cada compañero crea su rama desde develop. Las funcionalidades se integran mediante pull requests hacia develop, sin publicar directamente en main. Los nombres de ramas no necesitan el prefijo ddd.

| Integrante | Contextos | Rama sugerida |
| --- | --- | --- |
| 1 | IAM y Profiles | feature/iam-profiles |
| 2 | Inventory y Subscriptions | feature/inventory-subscriptions |
| 3 | Rentals | feature/rentals |
| 4 | Maintenance | feature/maintenance |
| 5 | Integración, persistencia y pruebas | feature/integration |

## Incorporar un contexto

1. Añadir sus agregados, comandos, consultas, repositorios, recursos y controladores dentro del contexto.
2. Crear una clase pública sin parámetros que implemente IPlatformModule en Infrastructure/Configuration.
3. Registrar servicios con Register y configurar entidades y relaciones con ConfigureModel. Los módulos se descubren dentro del ensamblado de la API.
4. Proveer las facades públicas de su contexto. Los contratos iniciales de Inventory, Rentals y Subscriptions permiten que Maintenance compile; los proveedores provisionales devuelven 501 CONTEXT_PENDING, sin simular resultados de negocio. Registrar la implementación real con AddScoped para sustituir el proveedor provisional.
5. Añadir las pruebas del caso de uso y ejecutar dotnet build y dotnet test antes del pull request.

El repositorio común no contiene implementaciones de otros integrantes. La autenticación JWT está preparada; el módulo IAM debe incorporar la emisión y la comprobación del estado de cuenta y revocación de tokens. No se desactiva la autorización de los controladores.

InitializeSchema usa EnsureCreated solo como arranque de desarrollo con módulos presentes. Al integrar nuevos módulos sobre una base que ya tiene tablas, se deben preparar migraciones EF y conservar sus datos; EnsureCreated no actualiza un esquema existente. Las consultas agregadas con colecciones deben configurar AutoInclude o sobrescribir Query en su repositorio.

La estructura toma como referencia el inicio de RQLS26/buildline-platform (88ad0afdf0ce2c68fc5b8f6bf8455513ac429ecd). Se adapta la configuración y los nombres a RentBuild; las implementaciones posteriores del ejemplo no se importan.
