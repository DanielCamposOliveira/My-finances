using Cloud_API.src.Model;
using Microsoft.EntityFrameworkCore;


namespace Cloud_API
{
    public class AppDbContext : DbContext
    {
        public DbSet<Documento> StorageRegistry { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=src\\Data\\app.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documento>(builder =>
            {
                builder.ToTable("Documentos");
                builder.HasKey(d => d.Id);

                builder.Property(d => d.NameOriginal)
                  .IsRequired()
                  .HasMaxLength(50);

                builder.Property(d => d.NameArchived)
                  .IsRequired()
                  .HasMaxLength(10);

                builder.Property(d => d.Extension)
                  .IsRequired()
                  .HasMaxLength(6);

                builder.Property(d => d.Ativo)
                   .HasConversion<int>()
                   .IsRequired();

                builder.Property(d => d.QtdAcess)
               .HasConversion<int>()
               .IsRequired();

                builder.HasIndex(d => new
                {
                    d.NameArchived
                })
                .HasDatabaseName("IX_NameArchived"); // Nome do índice no banco de dados               

            });
           

        }
    }
}
