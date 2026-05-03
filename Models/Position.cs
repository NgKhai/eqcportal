using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class Position
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Position Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Soft delete flag
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
