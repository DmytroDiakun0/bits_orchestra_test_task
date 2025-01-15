using bits_orchestra_test_task.Models;
using Microsoft.EntityFrameworkCore;

namespace bits_orchestra_test_task.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}