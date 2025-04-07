using LeaveService.Models;

namespace LeaveService.Interface
{
    public interface ILeaveService
    {
        Task<bool> ApplyForLeave(LeaveRequest request);
        Task<List<LeaveRequest>> GetEmployeeLeaveRequests(string userId);
        Task<List<LeaveRequest>> GetAllLeaveRequests();
        Task<bool> ApproveOrRejectLeave(int leaveId, string status); // ✅ Add this method

    }
}
