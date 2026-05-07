using eqcportal.Data;
using eqcportal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PerformanceController(ApplicationDbContext context)
        {
            _context = context;
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
            review.OverallRating = CalculateOverallRating(review);

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

        private async Task LoadEmployeesAsync(int? employeeId = null)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "Id", "FullName", employeeId);
        }

        private static decimal CalculateOverallRating(PerformanceReview review)
        {
            var total = review.TeamworkScore + review.SkillScore + review.PunctualityScore + review.AttitudeScore;
            return Math.Round(total / 4m, 2);
        }
    }
}
