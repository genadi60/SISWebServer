using Microsoft.EntityFrameworkCore;
using MishMashWebApp.Models;

namespace MishMashWebApp.Data
{
    public class MishMashDbContext : DbContext
    {
        public MishMashDbContext()
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<UserChannel> UserChannel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLazyLoadingProxies();
            builder.UseSqlServer("Server=.;Database=MishMash;Integrated Security=True;");
        }
        
    }
}
