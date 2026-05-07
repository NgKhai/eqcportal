namespace eqcportal.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int PendingLeaves { get; set; }
        public int PresentToday { get; set; }
        public List<DashboardChartPoint> EmployeesByDepartment { get; set; } = new();
        public List<DashboardChartPoint> EmployeesByGender { get; set; } = new();
        public List<DashboardChartPoint> AttendanceByMonth { get; set; } = new();
        public List<DashboardChartPoint> LeaveRequestsByStatus { get; set; } = new();
    }

    public class DashboardChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
