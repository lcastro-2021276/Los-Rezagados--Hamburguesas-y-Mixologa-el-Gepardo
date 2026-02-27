using Microsoft.AspNetCore.Mvc;
using AuthService.Persistence.Data;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Check()
    {
        var dbStatus = "Unhealthy";

        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync();
            dbStatus = canConnect ? "Healthy" : "Unhealthy";
        }
        catch
        {
            dbStatus = "Unhealthy";
        }

        var response = new
        {
            success = dbStatus == "Healthy",
            status = new
            {
                api = "Healthy",
                database = dbStatus,
                timestamp = DateTime.UtcNow
            }
        };

        return dbStatus == "Healthy"
            ? Ok(response)
            : StatusCode(503, response);
    }
}