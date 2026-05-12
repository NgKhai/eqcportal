using eqcportal.Data;
using eqcportal.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-5);
            var attendances = await _context.Attendances
                .AsNoTracking()
                .Where(a => a.Date >= monthStart)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalEmployees = await _context.Employees.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                PendingLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == "Pending" || l.Status == "Chờ duyệt"),
                PresentToday = await _context.Attendances.CountAsync(a => a.Date == today && (a.Status == "Present" || a.Status == "Có mặt")),
                EmployeesByDepartment = await _context.Departments
                    .AsNoTracking()
                    .OrderBy(d => d.Name)
                    .Select(d => new DashboardChartPoint
                    {
                        Label = d.Name,
                        Value = d.Employees.Count
                    })
                    .ToListAsync(),
                EmployeesByGender = await _context.Employees
                    .AsNoTracking()
                    .GroupBy(e => e.Gender ?? "Khác")
                    .Select(g => new DashboardChartPoint
                    {
                        Label = g.Key,
                        Value = g.Count()
                    })
                    .ToListAsync(),
                LeaveRequestsByStatus = await _context.LeaveRequests
                    .AsNoTracking()
                    .GroupBy(l => l.Status)
                    .Select(g => new DashboardChartPoint
                    {
                        Label = g.Key,
                        Value = g.Count()
                    })
                    .ToListAsync()
            };

            model.EmployeesByGender = model.EmployeesByGender
                .Select(g => new DashboardChartPoint
                {
                    Label = NormalizeGenderLabel(g.Label),
                    Value = g.Value
                })
                .GroupBy(g => g.Label)
                .Select(g => new DashboardChartPoint
                {
                    Label = g.Key,
                    Value = g.Sum(x => x.Value)
                })
                .OrderBy(g => g.Label)
                .ToList();

            for (var i = 0; i < 6; i++)
            {
                var month = monthStart.AddMonths(i);
                var nextMonth = month.AddMonths(1);
                model.AttendanceByMonth.Add(new DashboardChartPoint
                {
                    Label = $"T{month.Month}/{month.Year}",
                    Value = attendances.Count(a => a.Date >= month && a.Date < nextMonth)
                });
            }

            return model;
        }

        private static string NormalizeGenderLabel(string gender)
        {
            return gender switch
            {
                "Male" => "Nam",
                "Female" => "Nữ",
                "Other" => "Khác",
                "" => "Khác",
                _ => gender
            };
        }
    }
}
