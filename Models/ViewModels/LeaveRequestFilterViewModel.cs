using eqcportal.Models;

namespace eqcportal.Models.ViewModels
{
    public class LeaveRequestFilterViewModel
    {
        public string? SearchName { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; } = new();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int StartItem => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;
        public int EndItem => Math.Min(Page * PageSize, TotalCount);
    }
}
