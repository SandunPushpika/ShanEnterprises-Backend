using Core.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : Controller
{
    [HttpGet("/status")]
    public ActionResult<ApiResponse> Get()
    {
        return new ApiResponse("Health Status: Ok");
    }
}