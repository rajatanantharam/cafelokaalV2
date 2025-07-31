using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Data;
public class CafeLokaalContext(DbContextOptions<CafeLokaalContext> options) : DbContext(options)
{
    public DbSet<CafeOrder> CafeOrders { get; set; }
    public DbSet<UserAccess> UserAccess { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure CafeOrderModel entity
        modelBuilder.Entity<CafeOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.OrganizationId).IsRequired();
            entity.Property(e => e.OrganizationName).IsRequired();
            entity.Property(e => e.ProcessTime).IsRequired();
            entity.Property(e => e.ProcessDate).IsRequired();

            // Configure OrderStep as a conversion to/from string
            entity.Property(e => e.OrderStep)
                  .HasConversion(
                      v => v.Value,
                      v => ParseOrderStep(v))
                  .IsRequired();
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.OrderId);
        });

        // Configure UserAccess entity
        modelBuilder.Entity<UserAccess>(entity =>
        {
            entity.HasKey(e => new { e.Email, e.OrganizationName });
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OrganizationName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SubscriptionId).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.OrganizationName);
        });
    }

    private static OrderStep ParseOrderStep(string orderStepValue)
    {
        return orderStepValue switch
        {
            "OrderReceived" => OrderStep.OrderReceived,
            "OrderPrepared" => OrderStep.OrderPrepared,
            "OrderServed" => OrderStep.OrderServed,
            _ => OrderStep.Unknown
        };
    }
}