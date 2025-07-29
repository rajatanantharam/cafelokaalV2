using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Data;

public class CafeLokaalDBContext : DbContext
{

    public CafeLokaalDBContext(DbContextOptions<CafeLokaalDBContext> options) 
        : base(options) { }

    public DbSet<CafeOrderModel> CafeOrderModels { get; set; }
}