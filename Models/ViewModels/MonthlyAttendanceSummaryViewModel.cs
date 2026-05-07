namespace eqcportal.Models.ViewModels
{
    public class MonthlyAttendanceSummaryViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<MonthlyAttendanceSummaryRowViewModel> Rows { get; set; } = new();
    }

    public class MonthlyAttendanceSummaryRowViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int HalfDayCount { get; set; }
        public decimal AverageHoursPerDay { get; set; }
        public decimal TotalHours { get; set; }
    }
}
