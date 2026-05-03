namespace eqcportal.Models.ViewModels
{
    public class EmployeeFilterViewModel
    {
        public string? SearchName { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
