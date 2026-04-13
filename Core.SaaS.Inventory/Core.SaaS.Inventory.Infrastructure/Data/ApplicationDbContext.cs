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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API: Mantenemos el Dominio limpio de atributos de base de datos
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SubscriptionPlan).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                // EL CORAZÓN DEL SaaS: Filtro Global de Consultas
                // Todo SELECT, UPDATE o DELETE de Product tendrá un "WHERE TenantId = _currentTenantId" automático
                entity.HasQueryFilter(e => e.TenantId == _currentTenantId);
            });
        }
    }
}
