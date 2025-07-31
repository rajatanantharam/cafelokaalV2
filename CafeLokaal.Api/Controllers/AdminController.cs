using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeLokaal.Api.Data;
using CafeLokaal.Api.Models;

namespace CafeLokaal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IUserAccessRepository _userAccessRepository;

    public AdminController(IUserAccessRepository userAccessRepository, ILogger<AdminController> logger)
    {
        _userAccessRepository = userAccessRepository;
        _logger = logger;
    }

    [HttpPost("useraccess")]
    public async Task<ActionResult<UserAccess>> CreateUserAccess([FromBody] UserAccess userAccess)
    {
        try
        {
            if (userAccess == null)
            {
                return BadRequest("UserAccess data is required");
            }

            if (string.IsNullOrEmpty(userAccess.Email) || 
                string.IsNullOrEmpty(userAccess.OrganizationName) || 
                string.IsNullOrEmpty(userAccess.SubscriptionId))
            {
                return BadRequest("Email, OrganizationName, and SubscriptionId are required");
            }

            await _userAccessRepository.CreateUserAccess(userAccess);

            return CreatedAtAction(nameof(GetUserAccess), 
                new { email = userAccess.Email, organizationName = userAccess.OrganizationName }, 
                userAccess);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating UserAccess for email: {Email}", userAccess?.Email);
            return StatusCode(500, "An error occurred while creating UserAccess");
        }
    }

    [HttpGet("useraccess/{email}/{organizationName}")]
    public async Task<ActionResult<UserAccess>> GetUserAccess(string email, string organizationName)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(organizationName))
            {
                return BadRequest("Email and OrganizationName are required");
            }

            var userAccess = await _userAccessRepository.GetUserAccess(email, organizationName);

            if (userAccess == null)
            {
                return NotFound("UserAccess not found");
            }

            return Ok(userAccess);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving UserAccess for email: {Email}, organization: {OrganizationName}", 
                email, organizationName);
            return StatusCode(500, "An error occurred while retrieving UserAccess");
        }
    } 
}
