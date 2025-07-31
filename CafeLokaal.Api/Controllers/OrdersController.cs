using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Data;
using CafeLokaal.Api.Models;
using Microsoft.AspNetCore.Cors;
using CafeLokaal.Api.Middleware;

namespace CafeLokaal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    //TODO: This endpoint should be authorized and the organizationId should be queried from the user's claims
    // to ensure that the user can only access orders for their organization. Do not trust the organizationId from the query string.

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CafeOrder>>> GetOrders(string organizationName)
    {
        try
        {
            var cafeOrders = await _orderRepository.GetOrdersAsync(organizationName);
            return Ok(cafeOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for cafe {CafeId}", User.Claims.FirstOrDefault(c => c.Type == "extension_CafeId")?.Value);
            return StatusCode(500, "An error occurred while retrieving orders");
        }
    }

    [HttpPost("/api/orders/seed")]
    public async Task<ActionResult> CreateDummyOrders(string organizationName)
    {
        try
        {
            if (string.IsNullOrEmpty(organizationName))
            {
                return BadRequest("Invalid organization name or connection string not found.");
            }

            await _orderRepository.CreateDummyOrdersAsync(organizationName);
            return Ok(new { Message = "Dummy orders created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dummy orders");
            return StatusCode(500, "An error occurred while creating dummy orders");
        }
    }
}