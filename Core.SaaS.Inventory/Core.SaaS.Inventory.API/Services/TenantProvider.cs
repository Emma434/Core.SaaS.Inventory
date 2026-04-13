using System;
using Microsoft.AspNetCore.Http;
using Core.SaaS.Inventory.Application.Interfaces;

namespace Core.SaaS.Inventory.API.Services
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetTenantId()
        {
            // Extraemos el claim "tenant_id" que vendrá en el Token JWT
            var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrWhiteSpace(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
            {
                throw new UnauthorizedAccessException("Acceso denegado: TenantId ausente o inválido en el token.");
            }

            return tenantId;
        }
    }
}
