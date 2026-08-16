using Application.Interfaces.Services;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse>> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        return new ApiResponse(data: result);
    }
}
