using Microsoft.AspNetCore.Mvc;
using MySqlConnector;


namespace CafeLokaal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IConfiguration config, ILogger<HealthController> logger)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("/api/health")]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }

        [HttpGet("/api/health/db")]
        public IActionResult CheckDatabase(string connectionStringName)
        {
            try
            {
                var connStr = _config.GetConnectionString(connectionStringName);
                using var conn = new MySqlConnection(connStr);
                _logger.LogInformation("Attempting to connect to database with connection string: {ConnectionString}", connStr);
                conn.Open();
                return Ok("✅ Database connection successful!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to database");
                return StatusCode(500, $"❌ Failed to connect to DB: {ex.Message}");
            }
        }
    }
}
