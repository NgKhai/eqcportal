using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class PerformanceReview
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên")]
        [Display(Name = "Nhân viên")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm làm việc nhóm")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        [Display(Name = "Làm việc nhóm")]
        public int TeamworkScore { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm kỹ năng")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        [Display(Name = "Kỹ năng")]
        public int SkillScore { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm đúng giờ")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        [Display(Name = "Đúng giờ")]
        public int PunctualityScore { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm thái độ")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        [Display(Name = "Thái độ")]
        public int AttitudeScore { get; set; }

        [Display(Name = "Tổng điểm")]
        public decimal OverallRating { get; set; }

        [StringLength(1000, ErrorMessage = "Nhận xét không được vượt quá 1000 ký tự")]
        [Display(Name = "Nhận xét")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập kỳ đánh giá")]
        [StringLength(50, ErrorMessage = "Kỳ đánh giá không được vượt quá 50 ký tự")]
        [Display(Name = "Kỳ đánh giá")]
        public string ReviewPeriod { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Người đánh giá không được vượt quá 150 ký tự")]
        [Display(Name = "Người đánh giá")]
        public string? ReviewerName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Employee? Employee { get; set; }
    }
}
