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
    public class LeaveBalanceController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveBalanceController(ILeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }

        //  Get Employee Leave Balance
        [HttpGet("my-balance")]
        public async Task<IActionResult> GetMyLeaveBalance()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var balance = await _leaveBalanceService.GetLeaveBalance(userId);
            return balance != null ? Ok(balance) : NotFound("Leave balance not found.");
        }

        //  HR/Admin View All Employee Leave Balances
        [HttpGet("all")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetAllLeaveBalances()
        {
            var balances = await _leaveBalanceService.GetAllLeaveBalances();
            return Ok(balances);
        }

        //  HR/Admin Initializes Leave Balance for a New Employee
        [HttpPost("initialize/{userId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> InitializeLeaveBalance(string userId)
        {
            bool success = await _leaveBalanceService.InitializeLeaveBalance(userId);
            return success ? Ok("Leave balance initialized successfully.") : BadRequest("Failed to initialize leave balance.");
        }

        //  HR/Admin Updates Employee Leave Balance
        [HttpPut("update/{userId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateLeaveBalance(string userId, [FromBody] EmployeeLeaveBalance balance)
        {
            bool success = await _leaveBalanceService.UpdateLeaveBalance(userId, balance.AnnualLeaveBalance, balance.SickLeaveBalance);
            return success ? Ok("Leave balance updated successfully.") : BadRequest("Failed to update leave balance.");
        }
    }
}
