using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên")]
        [Display(Name = "Nhân viên")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        [Display(Name = "Từ ngày")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        [Display(Name = "Đến ngày")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại nghỉ")]
        [StringLength(50, ErrorMessage = "Loại nghỉ không được vượt quá 50 ký tự")]
        [Display(Name = "Loại nghỉ")]
        public string LeaveType { get; set; } = "Nghỉ phép";

        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        [Display(Name = "Lý do")]
        public string? Reason { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chờ duyệt";

        [StringLength(500, ErrorMessage = "Phản hồi không được vượt quá 500 ký tự")]
        [Display(Name = "Phản hồi quản trị")]
        public string? AdminComment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Employee? Employee { get; set; }

        // Computed: number of days requested
        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
}
