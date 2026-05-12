namespace eqcportal.Services
{
    public interface ILeaveRequestService
    {
        Task<(bool Success, string Message)> ProcessAsync(int id, string actionType, string? adminComment);
    }
}
