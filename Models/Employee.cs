using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eqcportal.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(150, ErrorMessage = "Họ và tên không được vượt quá 150 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Display(Name = "SĐT")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày vào làm")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày vào làm")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Lương không được nhỏ hơn 0")]
        [Display(Name = "Lương")]
        public decimal? Salary { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        [Display(Name = "Ảnh đại diện")]
        public string? ProfilePhotoPath { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn chức vụ")]
        [Display(Name = "Chức vụ")]
        public int PositionId { get; set; }

        // Soft delete flag
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public Position? Position { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
    }
}
