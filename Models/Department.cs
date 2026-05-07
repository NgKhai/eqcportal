using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phòng ban")]
        [StringLength(100, ErrorMessage = "Tên phòng ban không được vượt quá 100 ký tự")]
        [Display(Name = "Tên phòng ban")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Soft delete flag
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
