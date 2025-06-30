using Microsoft.EntityFrameworkCore;
using JwtEx.Data;
using JwtEx.Model;

namespace JwtEx.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
