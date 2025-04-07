using LeaveService.Interface;
using LeaveService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveController(ILeaveService leaveService, ILeaveBalanceService leaveBalanceService)
        {
            _leaveService = leaveService;
            _leaveBalanceService = leaveBalanceService;
        }

        // 🔹 Employee Applies for Leave
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLeave([FromBody] LeaveRequest request)
        {
            try
            {
                request.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(request.UserId))
                    return Unauthorized("User not authenticated.");

                // Validate leave balance before applying
                var balance = await _leaveBalanceService.GetLeaveBalance(request.UserId);
                if (balance == null)
                    return BadRequest("Leave balance record not found.");

                int totalDays = (request.EndDate - request.StartDate).Days + 1;

                if (request.LeaveType == "Annual" && balance.AnnualLeaveBalance < totalDays)
                    return BadRequest("Insufficient annual leave balance.");
                if (request.LeaveType == "Sick" && balance.SickLeaveBalance < totalDays)
                    return BadRequest("Insufficient sick leave balance.");

                // Apply for leave
                bool success = await _leaveService.ApplyForLeave(request);
                if (!success) return BadRequest("Failed to submit leave request.");

                // Deduct leave balance after applying
                await _leaveBalanceService.DeductLeaveBalance(request.UserId, request.LeaveType, totalDays);

                return Ok("Leave request submitted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 🔹 Employee Views Their Own Leave Requests
        [HttpGet("my-leaves")]
        public async Task<IActionResult> GetEmployeeLeaveRequests()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var leaveRequests = await _leaveService.GetEmployeeLeaveRequests(userId);
            return Ok(leaveRequests);
        }

        // 🔹 HR/Admin View All Leave Requests
        [HttpGet("all")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var leaveRequests = await _leaveService.GetAllLeaveRequests();
            return Ok(leaveRequests);
        }

        // 🔹 Manager Approves/Rejects Leave Requests
        [HttpPut("approve/{leaveId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveLeave(int leaveId, [FromBody] string status)
        {
            if (status != "Approved" && status != "Rejected")
                return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");

            var result = await _leaveService.ApproveOrRejectLeave(leaveId, status);
            return result ? Ok($"Leave request {status.ToLower()} successfully.") : BadRequest("Failed to update leave status.");
        }
    }
}
