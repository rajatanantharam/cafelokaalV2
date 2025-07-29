using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Data;
using CafeLokaal.Api.Models;
using Microsoft.AspNetCore.Cors;

namespace CafeLokaal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    // private readonly CafeLokaalContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ILogger<OrdersController> logger)
    {
        // _context = context;
        _logger = logger;
    }

    [HttpGet]
    [EnableCors("AllowOrigin")] 
    public async Task<ActionResult<IEnumerable<CafeOrderModel>>> GetOrders()
    {
        // try
        // {
        //     var cafeId = User.Claims.FirstOrDefault(c => c.Type == "extension_CafeId")?.Value;
        //     var orders = await _context.Orders
        //         .AsNoTracking()
        //         .Where(o => o.CafeId == cafeId)
        //         .ToListAsync();

        //     return Ok(orders);
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Error retrieving orders");
        //     return StatusCode(500, "An error occurred while retrieving orders");
        // }

        return // a dummy order list for now
            Ok(new List<CafeOrderModel>
            {
                new CafeOrderModel
                {
                    OrderId = Guid.NewGuid(),
                    CafeId = "Cafe123",
                    TotalAmount = 100.00m,
                }
            });
    }

    // [HttpPost("/api/posdata")]
    // public async Task<IActionResult> SyncData([FromBody] OrderSyncRequest request)
    // {
    //     try
    //     {
    //         foreach (var order in request.Orders)
    //         {
    //             if (await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId))
    //             {
    //                 _context.Orders.Update(order);
    //             }
    //             else
    //             {
    //                 await _context.Orders.AddAsync(order);
    //             }
    //         }

    //         await _context.SaveChangesAsync();
    //         return Ok();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error syncing POS data");
    //         return StatusCode(500, "An error occurred while syncing POS data");
    //     }
    // }
}