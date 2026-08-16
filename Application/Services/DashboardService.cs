using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.DTOs.Response;

namespace Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
    {
        return await _repository.GetDashboardStatsAsync();
    }
}
