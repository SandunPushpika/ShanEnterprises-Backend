using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync();
    Task<PublicStatsResponse> GetPublicStatsAsync();

}
