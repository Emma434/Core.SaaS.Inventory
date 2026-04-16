# Core.SaaS.Inventory
### Sistema de Gestión de Inventario Multi-tenant para Locales Comerciales

**Descripción:**
SaaS de gestión de inventario y ventas multi-tenant para locales comerciales. Desarrollado con .NET 8, C# y SQL Server, enfocado en el aislamiento de datos (Data Isolation).

**Tecnologías Clave:**
Backend: .NET 8 Web API (C#) 
Persistencia: Entity Framework Core & SQL Server
Seguridad: JWT (JSON Web Tokens)
Arquitectura: Clean Architecture con separación de capas (Domain, Application, Infrastructure, API)

**Diferenciador Técnico:**
Implementación de arquitectura **Multi-tenancy** mediante el uso de `TenantId` a nivel de base de datos para garantizar que la información de cada local comercial sea estrictamente privada



1. Resumen Ejecutivo (El Valor de Negocio)

Definición: API REST construida en .NET 8 diseñada para ser el núcleo (Core) de un producto SaaS (Software as a Service) de gestión de inventario.
Problema resuelto: Garantiza el aislamiento estricto de datos entre múltiples clientes (Tenants) que comparten la misma base de datos e infraestructura, eliminando el riesgo de filtración cruzada de información.

2. Arquitectura Base (Clean Architecture)

Estrategia: Desacoplamiento total de responsabilidades mediante anillos concéntricos.
Domain: Contiene las reglas puras del negocio y las entidades base (Product, Tenant, BaseEntity) protegidas por encapsulamiento estricto (sin setters públicos).
Application: Orquesta los casos de uso. Define las interfaces de contratos (ITenantProvider, IApplicationDbContext) sin depender de ninguna tecnología externa.
Infrastructure: Implementa los contratos de datos usando Entity Framework Core y SQL Server.
API (Presentation): Punto de entrada ligero que maneja las peticiones HTTP, inyección de dependencias y configuración de Swagger.

3. El Corazón del SaaS: Aislamiento Multi-tenant

Inversión de Control: Uso de ITenantProvider para extraer el TenantId directamente del Token JWT en cada petición HTTP, manteniendo el Dominio agnóstico al contexto web.
Seguridad a Nivel de Datos: Implementación de Global Query Filters en el ApplicationDbContext de Entity Framework Core. Esto inyecta automáticamente la cláusula WHERE TenantId = X en absolutamente todas las consultas de lectura y escritura, haciendo físicamente imposible que un desarrollador olvide filtrar los datos por cliente a nivel de base de datos.

4. Patrones de Diseño y Lógica de Dominio

Problema resuelto: Eliminación de los "Controladores Gordos" acoplados a la base de datos.
Implementación: Separación estricta entre operaciones de mutación de estado (Commands) y operaciones de lectura (Queries) mediante la librería MediatR.
Beneficio: Los controladores de la API (ProductsController) actúan únicamente como pasarelas de mensajes (Dispatchers), delegando la ejecución a Handlers aislados en la capa de Aplicación, mejorando drásticamente la mantenibilidad y testeabilidad del código.

Inventario Inmutable (Kardex con Event Sourcing Simplificado)
Problema resuelto: La pérdida de historial de transacciones al usar comandos UPDATE tradicionales para modificar el stock.
Implementación: El stock de las entidades es de solo lectura (private set) y se gestiona exclusivamente mediante un historial inmutable de movimientos de entrada y salida (ProductMovements).
Resolución de Identidad: Se dominó el comportamiento del Change Tracker de Entity Framework Core utilizando ValueGeneratedNever() en la configuración fluida (Fluent API) para garantizar la concurrencia exacta de las transacciones y evitar colisiones de identidad en la base de datos durante las inserciones masivas.

5. Seguridad y Pruebas

Autenticación: Implementación de validación de tokens JWT (JSON Web Tokens) exigiendo el claim tenant_id.
Simulación de Entorno (Mocking): Desarrollo de un endpoint especializado (Auth/mock-token/{tenantId}) para forjar y firmar tokens criptográficamente válidos en entorno de desarrollo, permitiendo validar la arquitectura y el aislamiento en Swagger sin depender de un proveedor de identidad externo.
