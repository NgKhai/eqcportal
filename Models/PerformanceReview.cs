using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class PerformanceReview
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        // 4 criteria scores (each 1-5)
        [Required]
        [Range(1, 5)]
        [Display(Name = "Teamwork Score")]
        public int TeamworkScore { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Skill Score")]
        public int SkillScore { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Punctuality Score")]
        public int PunctualityScore { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Attitude Score")]
        public int AttitudeScore { get; set; }

        // Computed: average of 4 criteria (saved to DB)
        [Display(Name = "Overall Rating")]
        public decimal OverallRating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Review Period")]
        public string ReviewPeriod { get; set; } = string.Empty; // e.g. "Q1 2026", "April 2026"

        [StringLength(150)]
        [Display(Name = "Reviewer Name")]
        public string? ReviewerName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Employee? Employee { get; set; }
    }
}
