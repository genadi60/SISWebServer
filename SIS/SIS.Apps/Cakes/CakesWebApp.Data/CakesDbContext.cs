namespace CakesWebApp.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class CakesDbContext : DbContext
    {
        public CakesDbContext(DbContextOptions options) : base(options)
        {
        }

        public CakesDbContext()
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=.;Database=Cakes;Integrated Security=True;")
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
