using LeaveService.Models;

namespace LeaveService.Interface
{
    public interface ILeaveBalanceService
    {
        Task<EmployeeLeaveBalance> GetLeaveBalance(string userId);
        Task<bool> InitializeLeaveBalance(string userId);
        Task<bool> DeductLeaveBalance(string userId, string leaveType, int days);
        Task<bool> UpdateLeaveBalance(string userId, int annualLeave, int sickLeave);
        Task<List<EmployeeLeaveBalance>> GetAllLeaveBalances();
    }
}
