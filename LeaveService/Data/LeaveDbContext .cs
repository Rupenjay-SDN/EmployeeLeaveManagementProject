using LeaveService.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Data
{
    public class LeaveDbContext : DbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options) { }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<EmployeeLeaveBalance> LeaveBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
