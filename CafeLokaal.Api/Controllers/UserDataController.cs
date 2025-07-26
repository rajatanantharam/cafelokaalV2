using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Data;
using CafeLokaal.Api.Models;
using System.Text.Json;

namespace CafeLokaal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserDataController : ControllerBase
{
    private readonly CafeLokaalContext _context;
    private readonly ILogger<UserDataController> _logger;

    public UserDataController(CafeLokaalContext context, ILogger<UserDataController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("/api/user-data")]
    public async Task<ActionResult<UserDataResponse>> GetUserData()
    {
        try
        {
            var cafe = await _context.Cafes.FirstOrDefaultAsync();
            var orders = await _context.Orders.ToListAsync();

            var response = new UserDataResponse
            {
                Cafe = cafe,
                Orders = orders
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user data");
            return StatusCode(500, "An error occurred while retrieving user data");
        }
    }

    [HttpGet("/api/user-data/export")]
    public async Task<IActionResult> ExportUserData()
    {
        try
        {
            var data = await GetUserData();
            if (data.Result is OkObjectResult okResult)
            {
                var json = JsonSerializer.Serialize(okResult.Value, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return File(System.Text.Encoding.UTF8.GetBytes(json), 
                    "application/json", 
                    $"user-data-export-{DateTime.UtcNow:yyyy-MM-dd}.json");
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data");
            return StatusCode(500, "An error occurred while exporting user data");
        }
    }

    [HttpDelete("/api/user-data")]
    public async Task<IActionResult> DeleteUserData()
    {
        try
        {
            var cafe = await _context.Cafes.FirstOrDefaultAsync();
            if (cafe != null)
            {
                _context.Cafes.Remove(cafe);
            }

            var orders = await _context.Orders.ToListAsync();
            _context.Orders.RemoveRange(orders);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User data deleted successfully");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user data");
            return StatusCode(500, "An error occurred while deleting user data");
        }
    }
}

public class UserDataResponse
{
    public CafeModel? Cafe { get; set; }
    public List<CafeOrderModel> Orders { get; set; } = new();
}
