using Core.DTOs.Response;

namespace Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync();
    Task<PublicStatsResponse> GetPublicStatsAsync();

}
