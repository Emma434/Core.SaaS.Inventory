# Core.SaaS.Inventory
### Sistema de Gestión de Inventario Multi-tenant para Locales Comerciales

**Descripción:**
SaaS de gestión de inventario y ventas multi-tenant para locales comerciales. Desarrollado con .NET 8, C# y SQL Server, enfocado en el aislamiento de datos (Data Isolation).

**Tecnologías Clave:**
* [cite_start]**Backend:** .NET 8 Web API (C#) [cite: 205]
* [cite_start]**Persistencia:** Entity Framework Core & SQL Server [cite: 205]
* [cite_start]**Seguridad:** JWT (JSON Web Tokens) [cite: 174]
* [cite_start]**Arquitectura:** Clean Architecture con separación de capas (Domain, Application, Infrastructure, API) [cite: 171, 206]

**Diferenciador Técnico:**
[cite_start]Implementación de arquitectura **Multi-tenancy** mediante el uso de `TenantId` a nivel de base de datos para garantizar que la información de cada local comercial sea estrictamente privada[cite: 207, 242].



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

4. Patrones de Diseño Aplicados (CQRS con MediatR)

Problema resuelto: Eliminación de los "Controladores Gordos" acoplados a la base de datos.


Implementación: Separación estricta entre operaciones de mutación de estado (Commands) y operaciones de lectura (Queries) mediante la librería MediatR.


Beneficio: Los controladores de la API (ProductsController) actúan únicamente como pasarelas de mensajes (Dispatchers), delegando la ejecución a Handlers aislados en la capa de Aplicación, mejorando drásticamente la mantenibilidad y testeabilidad del código.

5. Seguridad y Pruebas
Autenticación: Implementación de validación de tokens JWT (JSON Web Tokens) exigiendo el claim tenant_id.


Simulación de Entorno (Mocking): Desarrollo de un endpoint especializado (Auth/mock-token/{tenantId}) para forjar y firmar tokens criptográficamente válidos en entorno de desarrollo, permitiendo validar la arquitectura y el aislamiento en Swagger sin depender de un proveedor de identidad externo.
