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
    public class LeaveRequestController : Controller
    {
        private const int DefaultPageSize = 10;
        private static readonly string[] LeaveTypes =
        [
            "Nghỉ phép",
            "Ốm",
            "Cá nhân",
            "Thai sản",
            "Khác"
        ];

        private readonly ApplicationDbContext _context;
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ApplicationDbContext context, ILeaveRequestService leaveRequestService)
        {
            _context = context;
            _leaveRequestService = leaveRequestService;
        }

        public async Task<IActionResult> Index(LeaveRequestFilterViewModel filter)
        {
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize < 1 ? DefaultPageSize : filter.PageSize;

            var query = _context.LeaveRequests
                .Include(l => l.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchName))
            {
                var keyword = filter.SearchName.Trim();
                query = query.Where(l => l.Employee != null && l.Employee.FullName.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(l => l.Status == filter.Status);
            }

            filter.TotalCount = await query.CountAsync();
            if (filter.TotalPages > 0 && filter.Page > filter.TotalPages)
            {
                filter.Page = filter.TotalPages;
            }

            filter.LeaveRequests = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return View(filter);
        }

        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();
            ViewBag.LeaveTypes = new SelectList(LeaveTypes);
            return View(new LeaveRequest
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Status = "Chờ duyệt",
                LeaveType = "Nghỉ phép"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,StartDate,EndDate,LeaveType,Reason")] LeaveRequest leaveRequest)
        {
            if (leaveRequest.EndDate < leaveRequest.StartDate)
            {
                ModelState.AddModelError(nameof(LeaveRequest.EndDate), "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");
            }

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(leaveRequest.EmployeeId);
                ViewBag.LeaveTypes = new SelectList(LeaveTypes, leaveRequest.LeaveType);
                return View(leaveRequest);
            }

            leaveRequest.Status = "Chờ duyệt";
            leaveRequest.CreatedAt = DateTime.Now;

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Gửi đơn nghỉ phép thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest.Status != "Chờ duyệt")
            {
                TempData["Error"] = "Chỉ có thể sửa đơn nghỉ phép đang chờ duyệt.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await LoadEmployeesAsync(leaveRequest.EmployeeId);
            ViewBag.LeaveTypes = new SelectList(LeaveTypes, leaveRequest.LeaveType);
            return View(leaveRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,StartDate,EndDate,LeaveType,Reason")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.Id)
            {
                return NotFound();
            }

            var existing = await _context.LeaveRequests.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (existing.Status != "Chờ duyệt")
            {
                TempData["Error"] = "Chỉ có thể sửa đơn nghỉ phép đang chờ duyệt.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (leaveRequest.EndDate < leaveRequest.StartDate)
            {
                ModelState.AddModelError(nameof(LeaveRequest.EndDate), "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");
            }

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(leaveRequest.EmployeeId);
                ViewBag.LeaveTypes = new SelectList(LeaveTypes, leaveRequest.LeaveType);
                return View(leaveRequest);
            }

            existing.StartDate = leaveRequest.StartDate;
            existing.EndDate = leaveRequest.EndDate;
            existing.LeaveType = leaveRequest.LeaveType;
            existing.Reason = leaveRequest.Reason;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật đơn nghỉ phép thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest.Status != "Chờ duyệt")
            {
                TempData["Error"] = "Chỉ có thể hủy đơn nghỉ phép đang chờ duyệt.";
                return RedirectToAction(nameof(Details), new { id });
            }

            leaveRequest.Status = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã hủy đơn nghỉ phép.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id, string actionType, string? adminComment)
        {
            var (success, message) = await _leaveRequestService.ProcessAsync(id, actionType, adminComment);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(Details), new { id });
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
