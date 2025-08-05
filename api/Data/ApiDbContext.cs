using Microsoft.EntityFrameworkCore;
using MiniFab.Api.Models;

namespace MiniFab.Api.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<DeviceModel> DeviceModel { get; set; }
        public DbSet<SensorDataModel> SensorDataModel { get; set; }

    }
}
