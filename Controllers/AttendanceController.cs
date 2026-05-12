using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Models.ViewModels;
using eqcportal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    [Authorize(Roles = "Admin,HRManager")]
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
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(ApplicationDbContext context, IAttendanceService attendanceService)
        {
            _context = context;
            _attendanceService = attendanceService;
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
            await _attendanceService.ValidateAttendanceAsync(attendance, ModelState);

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(attendance.EmployeeId);
                ViewBag.AttendanceStatuses = new SelectList(AttendanceStatuses, attendance.Status);
                return View(attendance);
            }

            attendance.TotalHours = _attendanceService.CalculateTotalHours(attendance.CheckIn, attendance.CheckOut);
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo chấm công thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            await LoadEmployeesAsync(attendance.EmployeeId);
            ViewBag.AttendanceStatuses = new SelectList(AttendanceStatuses, attendance.Status);
            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,Date,CheckIn,CheckOut,Status,Note")] Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return NotFound();
            }

            var existing = await _context.Attendances.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _attendanceService.ValidateAttendanceAsync(attendance, ModelState, excludeId: id);

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(attendance.EmployeeId);
                ViewBag.AttendanceStatuses = new SelectList(AttendanceStatuses, attendance.Status);
                return View(attendance);
            }

            existing.Date = attendance.Date.Date;
            existing.CheckIn = attendance.CheckIn;
            existing.CheckOut = attendance.CheckOut;
            existing.Status = attendance.Status;
            existing.Note = attendance.Note;
            existing.TotalHours = _attendanceService.CalculateTotalHours(attendance.CheckIn, attendance.CheckOut);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật chấm công thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa chấm công thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MonthlySummary(int? month, int? year)
        {
            var selectedMonth = month ?? DateTime.Today.Month;
            var selectedYear = year ?? DateTime.Today.Year;

            var model = await _attendanceService.GetMonthlySummaryAsync(selectedMonth, selectedYear);
            return View(model);
        }

        private async Task LoadEmployeesAsync(int? employeeId = null)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "Id", "FullName", employeeId);
        }
    }
}
