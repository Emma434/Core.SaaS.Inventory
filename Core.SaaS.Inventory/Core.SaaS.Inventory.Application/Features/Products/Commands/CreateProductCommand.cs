using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System;

namespace Core.SaaS.Inventory.Application.Features.Products.Commands
{
    // IRequest<Guid> indica que cuando este comando termine, devolverá el ID del nuevo producto
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
