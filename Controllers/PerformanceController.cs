using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    [Authorize(Roles = "Admin,HRManager,Supervisor")]
    public class PerformanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPerformanceReviewService _reviewService;

        public PerformanceController(ApplicationDbContext context, IPerformanceReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.PerformanceReviews
                .Include(r => r.Employee)
                .OrderByDescending(r => r.CreatedAt)
                .ThenBy(r => r.Employee!.FullName)
                .ToListAsync();

            return View(reviews);
        }

        public async Task<IActionResult> Create()
        {
            await LoadEmployeesAsync();
            return View(new PerformanceReview
            {
                TeamworkScore = 3,
                SkillScore = 3,
                PunctualityScore = 3,
                AttitudeScore = 3,
                OverallRating = 3.00m
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,ReviewPeriod,ReviewerName,TeamworkScore,SkillScore,PunctualityScore,AttitudeScore,Comment")] PerformanceReview review)
        {
            review.OverallRating = _reviewService.CalculateOverallRating(review);

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(review.EmployeeId);
                return View(review);
            }

            review.CreatedAt = DateTime.Now;
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo đánh giá thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.PerformanceReviews
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.PerformanceReviews
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            await LoadEmployeesAsync(review.EmployeeId);
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,ReviewPeriod,ReviewerName,TeamworkScore,SkillScore,PunctualityScore,AttitudeScore,Comment")] PerformanceReview review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            var existing = await _context.PerformanceReviews.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            review.OverallRating = _reviewService.CalculateOverallRating(review);

            if (!ModelState.IsValid)
            {
                await LoadEmployeesAsync(review.EmployeeId);
                return View(review);
            }

            existing.ReviewPeriod = review.ReviewPeriod;
            existing.ReviewerName = review.ReviewerName;
            existing.TeamworkScore = review.TeamworkScore;
            existing.SkillScore = review.SkillScore;
            existing.PunctualityScore = review.PunctualityScore;
            existing.AttitudeScore = review.AttitudeScore;
            existing.OverallRating = review.OverallRating;
            existing.Comment = review.Comment;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật đánh giá thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.PerformanceReviews
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.PerformanceReviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.PerformanceReviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa đánh giá thành công.";
            return RedirectToAction(nameof(Index));
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
