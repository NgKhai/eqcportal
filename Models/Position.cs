using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class Position
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chức vụ")]
        [StringLength(100, ErrorMessage = "Tên chức vụ không được vượt quá 100 ký tự")]
        [Display(Name = "Tên chức vụ")]
        public string Title { get; set; } = string.Empty;

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
