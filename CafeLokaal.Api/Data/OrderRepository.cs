using CafeLokaal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeLokaal.Api.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<CafeOrderModel>> GetOrdersAsync(string organizationId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly CafeLokaalDBContext _context;

        public OrderRepository(CafeLokaalDBContext context)
        {
            _context = context;
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
