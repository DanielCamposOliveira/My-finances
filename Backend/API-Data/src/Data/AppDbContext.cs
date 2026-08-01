using API_Data.src.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Lancamento> Lancamentos => Set<Lancamento>();          
        public DbSet<ContaFixa> ContaFixa => Set<ContaFixa>();
        public DbSet<LancamentoParcela> LancamentoParcelas => Set<LancamentoParcela>();
        public DbSet<ContaFixaParcela> ContaFixaParcelas => Set<ContaFixaParcela>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento: Categoria
            modelBuilder.Entity<Categoria>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Nome)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(c => c.Atribuicao)
                       .HasConversion<int>()
                       .IsRequired();
            });

            // Mapeamento: Tag
            modelBuilder.Entity<Tag>(builder =>
            {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Nome)
                       .IsRequired()
                       .HasMaxLength(50);
            });

            // Mapeamento: Lancamento
            modelBuilder.Entity<Lancamento>(builder =>
            {
                builder.HasKey(l => l.Id);

                builder.Property(l => l.Descricao)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(l => l.ValorTotal)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.HasOne(l => l.Categoria)
                       .WithMany(c => c.Lancamentos)
                       .HasForeignKey(l => l.CategoriaId)
                       .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento N:N entre Lancamento e Tag (Join Table implícita do EF Core)
                builder.HasMany(l => l.Tags)
                       .WithMany(t => t.Lancamentos)
                       .UsingEntity(j => j.ToTable("LancamentoTags"));
            });

            // Mapeamento: LancamentoParcela
            modelBuilder.Entity<LancamentoParcela>(builder =>
            {
                builder.ToTable("lancamento_parcela");
                builder.HasKey(p => p.Id);

                builder.Property(p => p.ValorParcela)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.Property(p => p.Status)
                       .HasConversion<int>()
                       .IsRequired();

                builder.HasOne(p => p.Lancamento)
                       .WithMany(l => l.Parcelas)
                       .HasForeignKey(p => p.LancamentoId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Cascade);
            });


            // Mapeamento: ContaFixa
            modelBuilder.Entity<ContaFixa>(builder =>
            {
                builder.HasKey(cf => cf.Id);

                builder.Property(cf => cf.Descricao)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(cf => cf.ValorBase)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.HasOne(cf => cf.Categoria)
                       .WithMany()
                       .HasForeignKey(cf => cf.CategoriaId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(cf => cf.Tags)
                       .WithMany()
                       .UsingEntity(j => j.ToTable("ContaFixaTags"));
            });

            // Mapeamento: ContaFixaParcela
            modelBuilder.Entity<ContaFixaParcela>(builder =>
            {
                builder.ToTable("contafixa_parcela");
                builder.HasKey(p => p.Id);

                builder.Property(p => p.ValorParcela)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.Property(p => p.Status)
                       .HasConversion<int>()
                       .IsRequired();

                builder.HasOne(p => p.ContaFixa)
                       .WithMany(cf => cf.Parcelas)
                       .HasForeignKey(p => p.ContaFixaId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Cascade);
            });


            // Carga inicial de Tags padrão (Seed Data)
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Nome = "Casa" },
                new Tag { Id = 2, Nome = "Carro" },
                new Tag { Id = 3, Nome = "Gastos" },
                new Tag { Id = 4, Nome = "Mercado" },
                new Tag { Id = 5, Nome = "Energia" },
                new Tag { Id = 6, Nome = "Agua" },
                new Tag { Id = 7, Nome = "Internet" },
                new Tag { Id = 8, Nome = "Outros" },
                new Tag { Id = 9, Nome = "Facudade" },
                new Tag { Id = 10, Nome = "Emprestimo" },
                new Tag { Id = 11, Nome = "Streaming" },
                new Tag { Id = 12, Nome = "IA" },
                new Tag { Id = 13, Nome = "Armazenamento em Nuvem" }  
            );
        }

    }
}
