using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Data;

public class CafeLokaalContext : DbContext
{
    private readonly string _tenantId;

    public CafeLokaalContext(DbContextOptions<CafeLokaalContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        // In a real application, get the tenant ID from the authenticated user's claims
        _tenantId = httpContextAccessor.HttpContext?.User.FindFirst("tid")?.Value ?? string.Empty;
    }

    public DbSet<CafeModel> Cafes { get; set; }
    public DbSet<CafeOrderModel> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Multi-tenant filter on Cafes
        modelBuilder.Entity<CafeModel>()
            .HasQueryFilter(c => c.TenantId == _tenantId);

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
