using Microsoft.AspNetCore.Mvc;
using CafeLokaal.Api.Data;
using CafeLokaal.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace CafeLokaal.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrdersController> _logger;
    private readonly IUserAccessRepository _userAccessRepository;

    public OrdersController(IOrderRepository orderRepository, IUserAccessRepository userAccessRepository, ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _userAccessRepository = userAccessRepository;
    }

    //TODO: This endpoint should be authorized and the organizationId should be queried from the user's claims
    // to ensure that the user can only access orders for their organization. Do not trust the organizationId from the query string.

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CafeOrder>>> GetOrders()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("User email is required");
        }

        _logger.LogInformation("Retrieving orders for user {Email}", email);
        var organizationName = await _userAccessRepository.GetUserAccessOrganization(email);
        try
        {
            if (string.IsNullOrEmpty(organizationName))
            {
                return BadRequest("Organization name is required");
            }

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