using CafeLokaal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeLokaal.Api.Data
{
    public interface IOrderRepository
    {
        Task CreateDummyOrdersAsync();
        Task<IEnumerable<CafeOrderModel>> GetOrdersAsync(string organizationId);
    }

    public class OrderRepository(CafeLokaalDBContext context, ILogger<OrderRepository> logger) : IOrderRepository
    {
        private readonly CafeLokaalDBContext _context = context;
        private readonly ILogger<OrderRepository> _logger = logger;

        public async Task CreateDummyOrdersAsync()
        {          
            var dummyOrders = new List<CafeOrderModel>();
            var random = new Random();
            var orderSteps = new[] { OrderStep.OrderReceived, OrderStep.OrderPrepared, OrderStep.OrderServed };
            var organizations = new[]
            {
                new { Id = "ORG001", Name = "Cafe Willem" },
                new { Id = "ORG002", Name = "Cafe Olivier" },
                new { Id = "ORG003", Name = "Cafe Orloff" }               
            };

            for (int i = 1; i <= 20; i++)
            {
                var org = organizations[random.Next(organizations.Length)];
                var orderStep = orderSteps[random.Next(orderSteps.Length)];
                var processDate = DateTime.UtcNow.AddDays(-random.Next(30)).AddHours(-random.Next(24));

                dummyOrders.Add(new CafeOrderModel
                {
                    OrderId = $"ORDER{i:D3}",
                    OrganizationId = org.Id,
                    OrganizationName = org.Name,
                    OrderStep = orderStep,
                    ProcessTime = random.Next(5, 45), // 5-45 minutes
                    ProcessDate = processDate
                });
            }

            // Add to database
            await _context.CafeOrderModels.AddRangeAsync(dummyOrders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created {Count} dummy orders", dummyOrders.Count);
        }

        public async Task<IEnumerable<CafeOrderModel>> GetOrdersAsync(string organizationId)
        {
         
            if (string.IsNullOrEmpty(organizationId))
            {
                return [];
            }

            var query = _context.CafeOrderModels.AsQueryable();
            query = query.Where(o => o.OrganizationId == organizationId);
            return await query
                .OrderByDescending(o => o.ProcessDate)
                .AsNoTracking()
                .ToListAsync();

        }
    }
}
