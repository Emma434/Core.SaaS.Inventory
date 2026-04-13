using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SaaS.Inventory.Domain.Common;

namespace Core.SaaS.Inventory.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; }
        public string SubscriptionPlan { get; private set; }
        public bool IsActive { get; private set; }

        private Tenant() { }

        public Tenant(string name, string subscriptionPlan)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del Tenant es obligatorio.");

            Id = Guid.NewGuid();
            Name = name;
            SubscriptionPlan = subscriptionPlan;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
