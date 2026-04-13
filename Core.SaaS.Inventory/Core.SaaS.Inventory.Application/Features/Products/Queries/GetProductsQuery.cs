using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Core.SaaS.Inventory.Application.Features.Products.Queries
{
    // El formato de salida (lo que verá el cliente de la API)
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    // La consulta: "Tráeme una lista de ProductDto". 
    // No tiene propiedades porque queremos "todos" los productos de este cliente.
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
    }
}
