using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Data;
public class CafeLokaalDBContext(DbContextOptions<CafeLokaalDBContext> options) : DbContext(options)
{
    public DbSet<CafeOrderModel> CafeOrderModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure CafeOrderModel entity
        modelBuilder.Entity<CafeOrderModel>(entity =>
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