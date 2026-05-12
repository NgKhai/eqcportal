using eqcportal.Data;
using Microsoft.EntityFrameworkCore;

namespace eqcportal.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> ProcessAsync(int id, string actionType, string? adminComment)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return (false, "Không tìm thấy đơn nghỉ phép.");
            }

            if (leaveRequest.Status != "Chờ duyệt")
            {
                return (false, "Đơn nghỉ phép này đã được xử lý trước đó.");
            }

            if (actionType == "approve")
            {
                leaveRequest.Status = "Đã duyệt";
                leaveRequest.AdminComment = adminComment;
                await _context.SaveChangesAsync();
                return (true, "Đã duyệt đơn nghỉ phép.");
            }

            if (actionType == "reject")
            {
                leaveRequest.Status = "Từ chối";
                leaveRequest.AdminComment = adminComment;
                await _context.SaveChangesAsync();
                return (true, "Đã từ chối đơn nghỉ phép.");
            }

            return (false, "Thao tác không hợp lệ.");
        }
    }
}
