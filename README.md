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
