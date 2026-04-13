using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SaaS.Inventory.Domain.Common;

namespace Core.SaaS.Inventory.Domain.Entities
{
    public class Product : BaseEntity, IMustHaveTenant
    {
        public Guid TenantId { get; private set; }
        public string Name { get; private set; }
        public string SKU { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        private Product() { }

        public Product(Guid tenantId, string name, string sku, decimal price)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("El producto debe pertenecer a un Tenant válido.");

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            Id = Guid.NewGuid();
            TenantId = tenantId;
            Name = name;
            SKU = sku;
            Price = price;
            Stock = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
            Stock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
            if (Stock - quantity < 0) throw new InvalidOperationException("Stock insuficiente.");

            Stock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
