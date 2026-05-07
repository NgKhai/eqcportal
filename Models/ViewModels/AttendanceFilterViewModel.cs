using eqcportal.Models;

namespace eqcportal.Models.ViewModels
{
    public class AttendanceFilterViewModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? EmployeeId { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<Attendance> Attendances { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int StartItem => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;
        public int EndItem => Math.Min(Page * PageSize, TotalCount);
    }
}
