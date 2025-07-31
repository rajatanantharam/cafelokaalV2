using Microsoft.EntityFrameworkCore;

namespace CafeLokaal.Api.Data;

public interface IDBContextResolver
{
    CafeLokaalContext GetCafeTenantDB(string organizationName);
    CafeLokaalContext GetCafeLokaalDB();
}
public class DBContextResolver : IDBContextResolver
{
    private readonly IConfiguration _configuration;
    public DBContextResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CafeLokaalContext GetCafeTenantDB(string organizationName)
    {
        if (string.IsNullOrEmpty(organizationName))
        {
            throw new ArgumentException("Organization name cannot be null or empty", nameof(organizationName));
        }

        var connectionString = _configuration.GetConnectionString(organizationName);
        var optionsBuilder = new DbContextOptionsBuilder<CafeLokaalContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));
        var context = new CafeLokaalContext(optionsBuilder.Options);
        return context;
    }
    
    public CafeLokaalContext GetCafeLokaalDB()
    {
        var connectionString = _configuration.GetConnectionString("CafeLokaal");
        var optionsBuilder = new DbContextOptionsBuilder<CafeLokaalContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));
        var context = new CafeLokaalContext(optionsBuilder.Options);
        return context;
    }
}