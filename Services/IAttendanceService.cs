using eqcportal.Models;
using eqcportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eqcportal.Services
{
    public interface IAttendanceService
    {
        decimal? CalculateTotalHours(TimeSpan? checkIn, TimeSpan? checkOut);
        Task ValidateAttendanceAsync(Attendance attendance, ModelStateDictionary modelState, int? excludeId = null);
        Task<MonthlyAttendanceSummaryViewModel> GetMonthlySummaryAsync(int month, int year);
    }
}
