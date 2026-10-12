using Microsoft.EntityFrameworkCore;


namespace Cloud_API
{
    public class AppDbContext : DbContext
    {
        public DbSet<Registro> StorageRegistry { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=src\\Data\\app.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Registro>();

        }
    }
}
