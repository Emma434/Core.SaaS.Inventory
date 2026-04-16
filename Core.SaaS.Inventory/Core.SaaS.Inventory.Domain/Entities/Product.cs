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
        //el stock ya no se puede cambiar desde afuera
        public int Stock { get; private set; }

        // Lista inmutable de movimientos
        private readonly List<ProductMovement> _movements = new();
        public IReadOnlyCollection<ProductMovement> Movements => _movements.AsReadOnly();

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
        // --- LÓGICA DE NEGOCIO ---
        public void AddStock(int quantity, string reason)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser positiva.");

            Stock += quantity;
            _movements.Add(new ProductMovement(TenantId, Id, MovementType.In, quantity, reason));
        }

        public void RemoveStock(int quantity, string reason)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser positiva.");
            if (Stock < quantity) throw new InvalidOperationException("Stock insuficiente para esta operación.");

            Stock -= quantity;
            _movements.Add(new ProductMovement(TenantId, Id, MovementType.Out, quantity, reason));
        }
    }
}
