using Core.SaaS.Inventory.Application.Interfaces;
using Core.SaaS.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SaaS.Inventory.Application.Features.Products.Commands
{
    public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public AdjustStockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscamos el producto. Al usar FindAsync, EF Core ya lo empieza a vigilar automáticamente.
            var product = await _context.Products
                .Include(p => p.Movements)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
            {
                throw new Exception($"El producto con ID {request.ProductId} no existe.");
            }

            // 2. Modificamos el objeto en memoria. 
            // Esto altera el Stock y AGREGA un movimiento a la lista interna del producto.
            if (request.Type == MovementType.In)
            {
                product.AddStock(request.Quantity, request.Reason);
            }
            else if (request.Type == MovementType.Out)
            {
                product.RemoveStock(request.Quantity, request.Reason);
            }

            // 3. Guardamos. 
            // EF Core es inteligente: verá que el Stock cambió (hará un UPDATE) 
            // y verá que hay un movimiento nuevo en la lista (hará un INSERT).
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
