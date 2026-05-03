using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; } = "Annual"; // Annual, Sick, Personal, Maternity, Other

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [StringLength(500)]
        [Display(Name = "Admin Comment")]
        public string? AdminComment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Employee? Employee { get; set; }

        // Computed: number of days requested
        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
}
