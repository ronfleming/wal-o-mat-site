using Microsoft.AspNetCore.Mvc;

namespace WalOMat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/healthz")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "OK",
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet]
    public IActionResult Ping()
    {
        return Ok(new { Message = "pong", Timestamp = DateTime.UtcNow });
    }
}

