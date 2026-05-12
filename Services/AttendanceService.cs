using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public decimal? CalculateTotalHours(TimeSpan? checkIn, TimeSpan? checkOut)
        {
            if (!checkIn.HasValue || !checkOut.HasValue || checkOut <= checkIn)
            {
                return null;
            }

            var totalHours = (decimal)(checkOut.Value - checkIn.Value).TotalHours;
            return Math.Round(totalHours, 2);
        }

        public async Task ValidateAttendanceAsync(Attendance attendance, ModelStateDictionary modelState, int? excludeId = null)
        {
            var attendanceDate = attendance.Date.Date;

            var duplicateQuery = _context.Attendances.Where(a =>
                a.EmployeeId == attendance.EmployeeId &&
                a.Date == attendanceDate);

            if (excludeId.HasValue)
            {
                duplicateQuery = duplicateQuery.Where(a => a.Id != excludeId.Value);
            }

            if (await duplicateQuery.AnyAsync())
            {
                modelState.AddModelError(string.Empty, "Nhân viên này đã có bản ghi chấm công trong ngày đã chọn.");
            }

            if (attendance.CheckIn.HasValue && attendance.CheckOut.HasValue && attendance.CheckOut <= attendance.CheckIn)
            {
                modelState.AddModelError(nameof(Attendance.CheckOut), "Giờ ra phải lớn hơn giờ vào.");
            }

            if (!attendance.CheckIn.HasValue && attendance.CheckOut.HasValue)
            {
                modelState.AddModelError(nameof(Attendance.CheckIn), "Vui lòng nhập giờ vào khi đã có giờ ra.");
            }

            if ((attendance.Status == "Có mặt" || attendance.Status == "Đi muộn" || attendance.Status == "Nửa ngày") &&
                !attendance.CheckIn.HasValue)
            {
                modelState.AddModelError(nameof(Attendance.CheckIn), "Vui lòng nhập giờ vào cho trạng thái này.");
            }

            attendance.Date = attendanceDate;
        }

        public async Task<MonthlyAttendanceSummaryViewModel> GetMonthlySummaryAsync(int month, int year)
        {
            var rows = await _context.Employees
                .Select(employee => new MonthlyAttendanceSummaryRowViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    PresentCount = employee.Attendances.Count(a => a.Date.Month == month && a.Date.Year == year && a.Status == "Có mặt"),
                    AbsentCount = employee.Attendances.Count(a => a.Date.Month == month && a.Date.Year == year && a.Status == "Vắng mặt"),
                    LateCount = employee.Attendances.Count(a => a.Date.Month == month && a.Date.Year == year && a.Status == "Đi muộn"),
                    HalfDayCount = employee.Attendances.Count(a => a.Date.Month == month && a.Date.Year == year && a.Status == "Nửa ngày"),
                    TotalHours = employee.Attendances
                        .Where(a => a.Date.Month == month && a.Date.Year == year)
                        .Sum(a => a.TotalHours ?? 0),
                    AverageHoursPerDay = 0
                })
                .OrderBy(row => row.EmployeeName)
                .ToListAsync();

            foreach (var row in rows)
            {
                var countedDays = row.PresentCount + row.LateCount + row.HalfDayCount;
                row.AverageHoursPerDay = countedDays > 0
                    ? Math.Round(row.TotalHours / countedDays, 2)
                    : 0;
            }

            return new MonthlyAttendanceSummaryViewModel
            {
                Month = month,
                Year = year,
                Rows = rows
            };
        }
    }
}
