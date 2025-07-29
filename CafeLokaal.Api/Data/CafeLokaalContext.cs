using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Data;

public class CafeLokaalContext : DbContext
{

    public CafeLokaalContext(DbContextOptions<CafeLokaalContext> options) 
        : base(options) { }

    public DbSet<CafeModel> Cafes { get; set; }
    public DbSet<CafeOrderModel> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure owned types
        modelBuilder.Entity<CafeModel>()
            .OwnsOne(c => c.PrimaryContact);

        modelBuilder.Entity<CafeOrderModel>()
            .OwnsOne(o => o.OrderStates, orderStates =>
            {
                orderStates.OwnsOne(s => s.OrderReceived);
                orderStates.OwnsOne(s => s.OrderPrepared);
                orderStates.OwnsOne(s => s.OrderServed);
            });
    }
}
