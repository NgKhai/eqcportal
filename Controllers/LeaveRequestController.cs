using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
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

        public LeaveRequestController(ApplicationDbContext context)
        {
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id, string actionType, string? adminComment)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest.Status != "Chờ duyệt")
            {
                TempData["Error"] = "Đơn nghỉ phép này đã được xử lý trước đó.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (actionType == "approve")
            {
                leaveRequest.Status = "Đã duyệt";
                leaveRequest.AdminComment = adminComment;
                TempData["Success"] = "Đã duyệt đơn nghỉ phép.";
            }
            else if (actionType == "reject")
            {
                leaveRequest.Status = "Từ chối";
                leaveRequest.AdminComment = adminComment;
                TempData["Success"] = "Đã từ chối đơn nghỉ phép.";
            }
            else
            {
                TempData["Error"] = "Thao tác không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _context.SaveChangesAsync();
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
