using System.ComponentModel.DataAnnotations;

namespace LeaveService.Models
{
    public class EmployeeLeaveBalance
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } // Employee ID
        public int AnnualLeaveBalance { get; set; } = 20; // Default balance
        public int SickLeaveBalance { get; set; } = 10; // Default balance
        public DateTime? UpdatedDate { get; set; }
    }
}
