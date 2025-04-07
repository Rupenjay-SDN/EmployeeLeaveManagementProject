using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LeaveService.Data
{
    public class LeaveDbContextFactory : IDesignTimeDbContextFactory<LeaveDbContext>
    {
        public LeaveDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // or "../LeaveService/appsettings.json" if running from root
                .Build();

            var builder = new DbContextOptionsBuilder<LeaveDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new LeaveDbContext(builder.Options);
        }
    }
}
