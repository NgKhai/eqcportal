using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    public class AttendanceController : Controller
    {
        private const int DefaultPageSize = 10;
        private static readonly string[] AttendanceStatuses =
        [
            "Có mặt",
            "Vắng mặt",
            "Đi muộn",
            "Nửa ngày"
        ];

        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(AttendanceFilterViewModel filter)
        {
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize < 1 ? DefaultPageSize : filter.PageSize;

            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(a => a.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(a => a.Date <= toDate);
            }

            if (filter.EmployeeId.HasValue)
            {
                query = query.Where(a => a.EmployeeId == filter.EmployeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(a => a.Status == filter.Status);
            }

            filter.TotalCount = await query.CountAsync();
            if (filter.TotalPages > 0 && filter.Page > filter.TotalPages)
            {
                filter.Page = filter.TotalPages;
            }

            filter.Attendances = await query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Employee!.FullName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            filter.Employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return View(filter);
        }

        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();
            ViewBag.AttendanceStatuses = new SelectList(AttendanceStatuses);
            return View(new Attendance
            {
                Date = DateTime.Today,
                Status = "Có mặt"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Date,CheckIn,CheckOut,Status,Note")] Attendance attendance)
        {
            await ValidateAttendanceAsync(attendance);

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(attendance.EmployeeId);
                ViewBag.AttendanceStatuses = new SelectList(AttendanceStatuses, attendance.Status);
                return View(attendance);
            }

            attendance.TotalHours = CalculateTotalHours(attendance.CheckIn, attendance.CheckOut);
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo chấm công thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MonthlySummary(int? month, int? year)
        {
            var selectedMonth = month ?? DateTime.Today.Month;
            var selectedYear = year ?? DateTime.Today.Year;

            var rows = await _context.Employees
                .Select(employee => new MonthlyAttendanceSummaryRowViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    PresentCount = employee.Attendances.Count(a => a.Date.Month == selectedMonth && a.Date.Year == selectedYear && a.Status == "Có mặt"),
                    AbsentCount = employee.Attendances.Count(a => a.Date.Month == selectedMonth && a.Date.Year == selectedYear && a.Status == "Vắng mặt"),
                    LateCount = employee.Attendances.Count(a => a.Date.Month == selectedMonth && a.Date.Year == selectedYear && a.Status == "Đi muộn"),
                    HalfDayCount = employee.Attendances.Count(a => a.Date.Month == selectedMonth && a.Date.Year == selectedYear && a.Status == "Nửa ngày"),
                    TotalHours = employee.Attendances
                        .Where(a => a.Date.Month == selectedMonth && a.Date.Year == selectedYear)
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

            return View(new MonthlyAttendanceSummaryViewModel
            {
                Month = selectedMonth,
                Year = selectedYear,
                Rows = rows
            });
        }

        private async Task LoadEmployeesAsync(int? employeeId = null)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "Id", "FullName", employeeId);
        }

        private async Task ValidateAttendanceAsync(Attendance attendance)
        {
            var attendanceDate = attendance.Date.Date;

            var duplicateExists = await _context.Attendances.AnyAsync(a =>
                a.EmployeeId == attendance.EmployeeId &&
                a.Date == attendanceDate);

            if (duplicateExists)
            {
                ModelState.AddModelError(string.Empty, "Nhân viên này đã có bản ghi chấm công trong ngày đã chọn.");
            }

            if (attendance.CheckIn.HasValue && attendance.CheckOut.HasValue && attendance.CheckOut <= attendance.CheckIn)
            {
                ModelState.AddModelError(nameof(Attendance.CheckOut), "Giờ ra phải lớn hơn giờ vào.");
            }

            if (!attendance.CheckIn.HasValue && attendance.CheckOut.HasValue)
            {
                ModelState.AddModelError(nameof(Attendance.CheckIn), "Vui lòng nhập giờ vào khi đã có giờ ra.");
            }

            if ((attendance.Status == "Có mặt" || attendance.Status == "Đi muộn" || attendance.Status == "Nửa ngày") &&
                !attendance.CheckIn.HasValue)
            {
                ModelState.AddModelError(nameof(Attendance.CheckIn), "Vui lòng nhập giờ vào cho trạng thái này.");
            }

            attendance.Date = attendanceDate;
        }

        private static decimal? CalculateTotalHours(TimeSpan? checkIn, TimeSpan? checkOut)
        {
            if (!checkIn.HasValue || !checkOut.HasValue || checkOut <= checkIn)
            {
                return null;
            }

            var totalHours = (decimal)(checkOut.Value - checkIn.Value).TotalHours;
            return Math.Round(totalHours, 2);
        }
    }
}
