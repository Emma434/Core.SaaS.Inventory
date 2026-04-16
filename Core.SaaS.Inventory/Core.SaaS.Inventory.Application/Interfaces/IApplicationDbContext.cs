using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.SaaS.Inventory.Domain.Entities;

namespace Core.SaaS.Inventory.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Tenant> Tenants { get; set; }
        DbSet<Product> Products { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<ProductMovement> ProductMovements { get; set; }
    }
}
