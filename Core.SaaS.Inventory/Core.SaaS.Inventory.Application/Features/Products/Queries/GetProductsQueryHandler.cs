using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Core.SaaS.Inventory.Application.Interfaces;

namespace Core.SaaS.Inventory.Application.Features.Products.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // MAGIA DE ARQUITECTURA: 
            // Fíjate que NO estamos haciendo un .Where(p => p.TenantId == tenantId).
            // Entity Framework lo inyectará automáticamente gracias a tu ApplicationDbContext.

            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.SKU,
                    Price = p.Price,
                    Stock = p.Stock
                })
                .ToListAsync(cancellationToken);
        }
    }
} 
