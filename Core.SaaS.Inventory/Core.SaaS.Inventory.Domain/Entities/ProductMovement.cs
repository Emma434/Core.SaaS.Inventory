using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SaaS.Inventory.Domain.Entities
{
    // Usamos un enum para restringir los tipos de movimiento
    public enum MovementType
    {
        In,
        Out
    }

    public class ProductMovement
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public MovementType Type { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public DateTime Date { get; private set; }

        // El movimiento debe conocer a qué Tenant pertenece
        public Guid TenantId { get; private set; }

        protected ProductMovement() { } // EF Core necesita esto

        // Constructor blindado
        public ProductMovement(Guid tenantId, Guid productId, MovementType type, int quantity, string reason)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");

            Id = Guid.NewGuid();
            TenantId = tenantId;
            ProductId = productId;
            Type = type;
            Quantity = quantity;
            Reason = reason;
            Date = DateTime.UtcNow;
        }
    }
}