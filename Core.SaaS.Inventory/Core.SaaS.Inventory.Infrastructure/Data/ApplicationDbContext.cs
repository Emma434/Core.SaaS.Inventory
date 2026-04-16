using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SaaS.Inventory.Application.Interfaces;
using Core.SaaS.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.SaaS.Inventory.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantProvider _tenantProvider;
        private Guid _currentTenantId;

        // Inyectamos las opciones de EF y tu puente (ITenantProvider)
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;

            // Lógica defensiva: Cuando EF Core corre migraciones (en tiempo de diseño), 
            // no hay un contexto HTTP ni un Token. Por lo tanto, capturamos el error 
            // y asignamos Guid.Empty para permitir que la base de datos se genere.
            try
            {
                _currentTenantId = _tenantProvider.GetTenantId();
            }
            catch
            {
                _currentTenantId = Guid.Empty;
            }
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMovement> ProductMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Filtros Globales de Multi-tenancy
            modelBuilder.Entity<Tenant>().HasQueryFilter(e => e.Id == _currentTenantId);
            modelBuilder.Entity<Product>().HasQueryFilter(e => e.TenantId == _currentTenantId);
            modelBuilder.Entity<ProductMovement>().HasQueryFilter(e => e.TenantId == _currentTenantId);

            // 2. Mapeo explícito de la relación 1 a Muchos y la lista privada
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Movements)
                .WithOne()
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<Product>()
                .Navigation(p => p.Movements)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 3. LA CURA AL ERROR DE CONCURRENCIA
            // Obligamos a EF Core a aceptar nuestros Guids generados en el Dominio como INSERTS nuevos.
            modelBuilder.Entity<ProductMovement>()
                .Property(m => m.Id)
                .ValueGeneratedNever();
        }
    }
}
