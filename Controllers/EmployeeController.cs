using eqcportal.Data;
using eqcportal.Models;
using eqcportal.Models.ViewModels;
using eqcportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Controllers
{
    public class EmployeeController : Controller
    {
        private const int DefaultPageSize = 10;
        private const long MaxAvatarBytes = 2 * 1024 * 1024;
        private static readonly string[] AllowedAvatarExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly ApplicationDbContext _context;
        private readonly IImageStorageService _imageStorageService;

        public EmployeeController(ApplicationDbContext context, IImageStorageService imageStorageService)
        {
            _context = context;
            _imageStorageService = imageStorageService;
        }

        public async Task<IActionResult> Index(EmployeeFilterViewModel filter)
        {
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize < 1 ? DefaultPageSize : filter.PageSize;

            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchName))
            {
                var keyword = filter.SearchName.Trim();
                query = query.Where(e => e.FullName.Contains(keyword) || e.Email.Contains(keyword));
            }

            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == filter.DepartmentId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == filter.IsActive.Value);
            }

            filter.TotalCount = await query.CountAsync();
            if (filter.TotalPages > 0 && filter.Page > filter.TotalPages)
            {
                filter.Page = filter.TotalPages;
            }

            filter.Employees = await query
                .OrderBy(e => e.FullName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            filter.Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(filter);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new Employee { HireDate = DateTime.Today, IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Email,Phone,DateOfBirth,Gender,Address,HireDate,Salary,IsActive,DepartmentId,PositionId")] Employee employee, IFormFile? profilePhoto)
        {
            var photoPath = await SaveAvatarAsync(profilePhoto);
            if (!ModelState.IsValid)
            {
                await DeleteAvatarAsync(photoPath);
                await LoadDropdownsAsync(employee.DepartmentId, employee.PositionId);
                return View(employee);
            }

            employee.ProfilePhotoPath = photoPath;
            employee.CreatedAt = DateTime.Now;
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await LoadDropdownsAsync(employee.DepartmentId, employee.PositionId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone,DateOfBirth,Gender,Address,HireDate,Salary,IsActive,DepartmentId,PositionId")] Employee employee, IFormFile? profilePhoto, bool removePhoto = false)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            var photoPath = await SaveAvatarAsync(profilePhoto);
            if (!ModelState.IsValid)
            {
                await DeleteAvatarAsync(photoPath);
                employee.ProfilePhotoPath = existingEmployee.ProfilePhotoPath;
                await LoadDropdownsAsync(employee.DepartmentId, employee.PositionId);
                return View(employee);
            }

            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            existingEmployee.Gender = employee.Gender;
            existingEmployee.Address = employee.Address;
            existingEmployee.HireDate = employee.HireDate;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.PositionId = employee.PositionId;
            existingEmployee.UpdatedAt = DateTime.Now;

            if (removePhoto || photoPath != null)
            {
                await DeleteAvatarAsync(existingEmployee.ProfilePhotoPath);
                existingEmployee.ProfilePhotoPath = null;
            }

            if (photoPath != null)
            {
                existingEmployee.ProfilePhotoPath = photoPath;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;
            employee.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(int? departmentId = null, int? positionId = null)
        {
            var departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            var positions = await _context.Positions.OrderBy(p => p.Title).ToListAsync();

            ViewBag.Departments = new SelectList(departments, "Id", "Name", departmentId);
            ViewBag.Positions = new SelectList(positions, "Id", "Title", positionId);
        }

        private async Task<string?> SaveAvatarAsync(IFormFile? profilePhoto)
        {
            if (profilePhoto == null || profilePhoto.Length == 0)
            {
                return null;
            }

            if (profilePhoto.Length > MaxAvatarBytes)
            {
                ModelState.AddModelError("ProfilePhotoPath", "Ảnh đại diện không được vượt quá 2MB");
                return null;
            }

            var extension = Path.GetExtension(profilePhoto.FileName).ToLowerInvariant();
            if (!AllowedAvatarExtensions.Contains(extension))
            {
                ModelState.AddModelError("ProfilePhotoPath", "Chỉ hỗ trợ ảnh JPG, PNG hoặc WEBP");
                return null;
            }

            if (!_imageStorageService.IsConfigured)
            {
                ModelState.AddModelError("ProfilePhotoPath", "Cloudinary chưa được cấu hình");
                return null;
            }

            try
            {
                return await _imageStorageService.UploadEmployeeAvatarAsync(profilePhoto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ProfilePhotoPath", $"Không thể tải ảnh lên Cloudinary: {ex.Message}");
                return null;
            }
        }

        private async Task DeleteAvatarAsync(string? profilePhotoPath)
        {
            if (string.IsNullOrWhiteSpace(profilePhotoPath))
            {
                return;
            }

            await _imageStorageService.DeleteEmployeeAvatarAsync(profilePhotoPath);
        }
    }
}
