using Microsoft.AspNetCore.Mvc;
using MySqlConnector;


namespace CafeLokaal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HealthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("/api/health")]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }

        [HttpGet("/api/health/db")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var connStr = _config.GetConnectionString("DefaultConnection");
                using var conn = new MySqlConnection(connStr);
                conn.Open();
                return Ok("✅ Database connection successful!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Failed to connect to DB: {ex.Message}");
            }
        }
    }
}
