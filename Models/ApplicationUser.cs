using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eqcportal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Optional link to Employee record (for future self-service)
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
