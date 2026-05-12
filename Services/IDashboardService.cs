using eqcportal.Models.ViewModels;

namespace eqcportal.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
