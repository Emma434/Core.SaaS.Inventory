using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Core.SaaS.Inventory.Domain.Entities;
using Core.SaaS.Inventory.Application.Interfaces;

namespace Core.SaaS.Inventory.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public CreateProductCommandHandler(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Identificamos de quién es la petición
            var tenantId = _tenantProvider.GetTenantId();

            // 2. Usamos el dominio para crear el objeto
            var product = new Product(tenantId, request.Name, request.Sku, request.Price);

            // 3. Guardamos en la base de datos abstracta
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
