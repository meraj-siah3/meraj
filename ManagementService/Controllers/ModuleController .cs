using Microsoft.AspNetCore.Mvc;
using SharedModels.Models;


namespace MessageManager.Controllers;

[ApiController]
[Route("api/module")]
public class ModuleController : ControllerBase
{
    private static readonly Random _rand = new();

    [HttpPost("health")]    
    public IActionResult PostHealth([FromBody] HealthCheckRequest request)
    {
        var response = new HealthCheckResponse
        {
            IsEnabled = true,
            NumberOfActiveClients = _rand.Next(0, 6),
            ExpirationTime = DateTime.UtcNow.AddMinutes(10).ToString("o")
        };

        Console.WriteLine($"📥 HealthCheck از توزیع‌گر دریافت شد. Id={request.Id}");

        return Ok(response);
    }
}
