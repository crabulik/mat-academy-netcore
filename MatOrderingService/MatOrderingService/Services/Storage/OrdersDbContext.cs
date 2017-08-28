using MatOrderingService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Services.Storage
{
    public class OrdersDbContext: DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options): base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MapOrders(modelBuilder.Entity<Order>());
            MapOrderItems(modelBuilder.Entity<OrderItem>());
        }

        protected void MapOrders(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.Property(p => p.Id)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue(OrderStatus.New);

            builder.Property(p => p.CreatorId)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasMany(p => p.OrderItems)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .HasPrincipalKey(p => p.Id);

            builder.Property(p => p.IsDeleted)
                .IsRequired();

            builder.Property(p => p.CreateDate)
                .IsRequired()
                .ForSqlServerHasDefaultValueSql("GETDATE()");

            builder.HasKey(p => p.Id);
        }

        protected void MapOrderItems(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.Property(p => p.Id)
                .IsRequired();

            builder.Property(p => p.Value)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(p => p.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(p => p.OrderId)
                .HasPrincipalKey(p => p.Id);

            builder.HasKey(p => p.Id);
        }
    }
}
