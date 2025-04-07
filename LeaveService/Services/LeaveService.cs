using LeaveService.Data;
using LeaveService.Interface;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly LeaveDbContext _context;

        public LeaveService(LeaveDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApplyForLeave(LeaveRequest request)
        {
            // Fetch employee's leave balance
            var leaveBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.UserId == request.UserId);
            if (leaveBalance == null)
                throw new Exception("Leave balance record not found.");

            int totalDays = (request.EndDate - request.StartDate).Days + 1;

            // Validate available leave balance
            if (request.LeaveType == "Annual" && leaveBalance.AnnualLeaveBalance < totalDays)
                throw new Exception("Insufficient annual leave balance.");
            if (request.LeaveType == "Sick" && leaveBalance.SickLeaveBalance < totalDays)
                throw new Exception("Insufficient sick leave balance.");

            // Deduct leave balance
            if (request.LeaveType == "Annual")
                leaveBalance.AnnualLeaveBalance -= totalDays;
            else if (request.LeaveType == "Sick")
                leaveBalance.SickLeaveBalance -= totalDays;

            _context.LeaveBalances.Update(leaveBalance);
            _context.LeaveRequests.Add(request);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<LeaveRequest>> GetEmployeeLeaveRequests(string userId)
        {
            return await _context.LeaveRequests.Where(lr => lr.UserId == userId).ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllLeaveRequests()
        {
            return await _context.LeaveRequests.ToListAsync();
        }
        public async Task<bool> ApproveOrRejectLeave(int leaveId, string status)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveId);
            if (leaveRequest == null) return false;

            // Prevent re-approval if already processed
            if (leaveRequest.Status != "Pending")
                throw new Exception("Leave request has already been processed.");

            leaveRequest.Status = status;
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            // ✅ Send notification
            var notification = new
            {
                To = leaveRequest.Id, // make sure this property exists or is retrieved
                Subject = $"Leave Request {status}",
                Body = $"Your leave request from {leaveRequest.StartDate:dd MMM} to {leaveRequest.EndDate:dd MMM} has been {status.ToLower()} by the manager."
            };
            MessagePublisher.Publish(notification, "leave_notifications");

            return true;

        }


    }
}
