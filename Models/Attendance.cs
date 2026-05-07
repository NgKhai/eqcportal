using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eqcportal.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên")]
        [Display(Name = "Nhân viên")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày chấm công")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày")]
        public DateTime Date { get; set; } = DateTime.Today;

        [DataType(DataType.Time)]
        [Display(Name = "Giờ vào")]
        public TimeSpan? CheckIn { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Giờ ra")]
        public TimeSpan? CheckOut { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Tổng giờ")]
        public decimal? TotalHours { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Có mặt";

        [StringLength(300, ErrorMessage = "Ghi chú không được vượt quá 300 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        // Navigation property
        public Employee? Employee { get; set; }
    }
}
