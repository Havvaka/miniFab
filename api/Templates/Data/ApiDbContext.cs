using Microsoft.EntityFrameworkCore;
using minifab.api.Templates.Models;

namespace minifab.api.Templates.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<DeviceModel> DeviceModel { get; set; }
        public DbSet<SensorDataModel> SensorDataModel { get; set; }

    }
}
