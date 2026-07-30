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
        public DbSet<Parcela> Parcelas => Set<Parcela>();

        public DbSet<ContaFixa> ContaFixa => Set<ContaFixa>();

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

                builder.Property(l => l.Tipo)
                       .HasConversion<int>()
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


            // Mapeamento: Parcela
            modelBuilder.Entity<Parcela>(builder =>
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.ValorParcela)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.Property(p => p.Status)
                       .HasConversion<int>()
                       .IsRequired();

                // Relacionamento Opcional com Lancamento (Compras/Receitas parceladas ou à vista)
                builder.HasOne(p => p.Lancamento)
                       .WithMany(l => l.Parcelas)
                       .HasForeignKey(p => p.LancamentoId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Cascade); // Se apagar o Lançamento, apaga as parcelas dele

                // Relacionamento Opcional com ContaFixa (Contas recorrentes/mensais)
                builder.HasOne(p => p.ContaFixa)
                       .WithMany(cf => cf.Parcelas)
                       .HasForeignKey(p => p.ContaFixaId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Cascade); // Se apagar a Conta Fixa, apaga as faturas dela

                // Constraint de Banco (Check Constraint): Garante que OU tem LancamentoId OU tem ContaFixaId
                builder.ToTable(t => t.HasCheckConstraint(
                    "CK_Parcela_OrigemUnica",
                    "(\"LancamentoId\" IS NOT NULL AND \"ContaFixaId\" IS NULL) OR (\"LancamentoId\" IS NULL AND \"ContaFixaId\" IS NOT NULL)"
                ));
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
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(cf => cf.Tags)
                       .WithMany()
                       .UsingEntity(j => j.ToTable("ContaFixaTags"));
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
