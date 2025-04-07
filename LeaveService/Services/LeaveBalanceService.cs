using LeaveService.Data;
using LeaveService.Interface;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly LeaveDbContext _context;

        public LeaveBalanceService(LeaveDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeLeaveBalance> GetLeaveBalance(string userId)
        {
            return await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == userId);
        }

        public async Task<bool> InitializeLeaveBalance(string userId)
        {
            var existingBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == userId);
            if (existingBalance != null)
                return false; // Leave balance already exists

            var newBalance = new EmployeeLeaveBalance
            {
                UserId = userId,
                AnnualLeaveBalance = 20,  // Default annual leave
                SickLeaveBalance = 10,
                UpdatedDate = DateTime.UtcNow
            };

            _context.LeaveBalances.Add(newBalance);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeductLeaveBalance(string userId, string leaveType, int days)
        {
            var leaveBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == userId);
            if (leaveBalance == null) return false;

            if (leaveType == "Annual" && leaveBalance.AnnualLeaveBalance >= days)
                leaveBalance.AnnualLeaveBalance -= days;
            else if (leaveType == "Sick" && leaveBalance.SickLeaveBalance >= days)
                leaveBalance.SickLeaveBalance -= days;
            else
                return false; // Insufficient balance

            _context.LeaveBalances.Update(leaveBalance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLeaveBalance(string userId, int annualLeave, int sickLeave)
        {
            var leaveBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == userId);
            if (leaveBalance == null) return false;

            leaveBalance.AnnualLeaveBalance = annualLeave;
            leaveBalance.SickLeaveBalance = sickLeave;

            _context.LeaveBalances.Update(leaveBalance);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<EmployeeLeaveBalance>> GetAllLeaveBalances()
        {
            return await _context.LeaveBalances.ToListAsync();
        }

    }
}
