using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Core.SaaS.Inventory.Domain.Entities;

namespace Core.SaaS.Inventory.Application.Features.Products.Commands
{
    // Devuelve un booleano para confirmar el éxito
    public class AdjustStockCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
