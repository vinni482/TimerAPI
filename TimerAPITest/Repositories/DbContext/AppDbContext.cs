using Microsoft.EntityFrameworkCore;
using TimerAPITest.Repositories.Entities;

namespace TimerAPITest.Repositories
{
    public class AppDbContext : DbContext
    {
        public DbSet<TimerDBEntity> Timers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
