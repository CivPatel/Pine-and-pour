using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinePour.Api.Features.Admin;
using PinePour.Api.Features.Auth;
using PinePour.Api.Services;

namespace PinePour.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminController(AnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
    {
        var dashboard = await analyticsService.BuildDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("reports/inventory")]
    public async Task<ActionResult<IEnumerable<LowInventoryItemDto>>> GetInventoryReport()
    {
        var dashboard = await analyticsService.BuildDashboardAsync();
        return Ok(dashboard.LowInventoryItems);
    }
}
