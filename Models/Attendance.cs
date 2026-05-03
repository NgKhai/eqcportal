using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eqcportal.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [DataType(DataType.Time)]
        [Display(Name = "Check In")]
        public TimeSpan? CheckIn { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Check Out")]
        public TimeSpan? CheckOut { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Total Hours")]
        public decimal? TotalHours { get; set; }  // Computed: CheckOut - CheckIn

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Present"; // Present, Absent, Late, HalfDay

        [StringLength(300)]
        public string? Note { get; set; }

        // Navigation property
        public Employee? Employee { get; set; }
    }
}
