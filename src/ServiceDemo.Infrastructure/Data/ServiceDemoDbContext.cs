using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceDemo.Domain.Entities;

namespace ServiceDemo.Infrastructure.Data
{
    public class ServiceDemoDbContext : DbContext
    {
        public ServiceDemoDbContext(DbContextOptions<ServiceDemoDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Stock).IsRequired();
            });
        }
    }
}