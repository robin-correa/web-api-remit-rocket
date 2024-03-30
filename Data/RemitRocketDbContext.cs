using Microsoft.EntityFrameworkCore;
using web_api_remit_rocket.Models;

namespace web_api_remit_rocket.Data
{
    public class RemitRocketDbContext : DbContext
    {
        public RemitRocketDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
