using CafeLokaal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeLokaal.Api.Data
{
    public interface IOrderRepository
    {
        Task CreateDummyOrdersAsync(string organizationName);
        Task<IEnumerable<CafeOrder>> GetOrdersAsync(string organizationName);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CreateDummyOrdersAsync(string organizationName)
        {
            using var context = GetCafeLokaalContext(organizationName);

            _logger.LogInformation("Creating dummy orders for organization: {OrganizationName}", organizationName);
            var dummyOrders = new List<CafeOrder>();
            var random = new Random();
            var orderSteps = new[] { OrderStep.OrderReceived, OrderStep.OrderPrepared, OrderStep.OrderServed };
            var organization = new { Id = Guid.NewGuid().ToString(), Name = organizationName };

            for (int i = 1; i <= 20; i++)
            {
                var orderStep = orderSteps[random.Next(orderSteps.Length)];
                var processDate = DateTime.UtcNow.AddDays(-random.Next(30)).AddHours(-random.Next(24));

                dummyOrders.Add(new CafeOrder
                {
                    OrderId = $"ORDER{i:D3}",
                    OrganizationId = organization.Id,
                    OrganizationName = organization.Name,
                    OrderStep = orderStep,
                    ProcessTime = random.Next(5, 45), // 5-45 minutes
                    ProcessDate = processDate
                });
            }

            // Add to database
            await context.CafeOrders.AddRangeAsync(dummyOrders);
            await context.SaveChangesAsync();
            _logger.LogInformation("Successfully created {Count} dummy orders", dummyOrders.Count);
        }

        public async Task<IEnumerable<CafeOrder>> GetOrdersAsync(string organizationName)
        {
            if (string.IsNullOrEmpty(organizationName))
            {
                throw new BadHttpRequestException("Organization name cannot be null or empty");
            }

            try
            {
                using var context = GetCafeLokaalContext(organizationName);
                var query = context.CafeOrders.AsQueryable();
                query = query.Where(o => o.OrganizationName == organizationName);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for organization: {OrganizationName}", organizationName);
                throw new Exception("An error occurred while retrieving orders", ex);
            }
        }

        private CafeLokaalContext GetCafeLokaalContext(string organizationName)
        {
            var connectionString = _configuration.GetConnectionString(organizationName);
            var optionsBuilder = new DbContextOptionsBuilder<CafeLokaalContext>();
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));
            var context = new CafeLokaalContext(optionsBuilder.Options);
            return context;
        }
    }
}
