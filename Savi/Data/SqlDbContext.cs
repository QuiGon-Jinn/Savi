using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Data
{
    public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Timesheet> Timesheets { get; set; } = null!;
    }
}
