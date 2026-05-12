using eqcportal.Data;
using eqcportal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    [Authorize(Roles = "Admin,HRManager")]
    public class PositionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var positions = await _context.Positions
                .Include(p => p.Employees)
                .OrderBy(p => p.Title)
                .ToListAsync();

            return View(positions);
        }

        public IActionResult Create()
        {
            return View(new Position());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Position position)
        {
            if (!ModelState.IsValid)
            {
                return View(position);
            }

            position.CreatedAt = DateTime.Now;
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm chức vụ thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Position position)
        {
            if (id != position.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(position);
            }

            var existingPosition = await _context.Positions.FindAsync(id);
            if (existingPosition == null)
            {
                return NotFound();
            }

            existingPosition.Title = position.Title;
            existingPosition.Description = position.Description;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật chức vụ thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            position.IsDeleted = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa chức vụ thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
