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
        public DbSet<HistoricoFinanceiroAnual> HistoricosFinanceiros => Set<HistoricoFinanceiroAnual>();

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

                // Relacionamento 1:N entre Categoria e Lancamento. 
                builder.HasOne(l => l.Categoria) // Define o relacionamento 1:N (Lancamento tem uma Categoria)
                       .WithMany(c => c.Lancamentos) // Define a relação inversa (Categoria tem muitos Lancamentos)
                       .HasForeignKey(l => l.CategoriaId) // Define a chave estrangeira 
                       .IsRequired() // Define que a Categoria é obrigatória para um Lancamento
                       .OnDelete(DeleteBehavior.Restrict); // Evita exclusão da Categoria se houver lançamentos associados

                // Relacionamento N:N entre Lancamento e Tag (Join Table implícita do EF Core)
                builder.HasMany(l => l.Tags) // Define o relacionamento N:N (Lancamento tem muitas Tags)
                       .WithMany(t => t.Lancamentos) // Define a relação inversa (Tag tem muitos Lancamentos)
                       .UsingEntity(j => j.ToTable("LancamentoTags")); // Define a tabela de junção (join table) com o nome "LancamentoTags"

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

                // Índice para localizar parcelas pendentes por vencimento
                // e realizar o JOIN com Lancamento sem acessar a tabela principal
                builder.HasIndex(p => new
                {
                    p.Status,
                    p.DataVencimento,
                    p.LancamentoId
                })
                .HasDatabaseName("IX_LancamentoParcela_Status_DataVencimento_Lancamento") // Nome do índice no banco de dados
                .IncludeProperties(p => p.ValorParcela); // Inclui a coluna ValorParcela no índice para otimizar consultas que retornam esse campo
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

                // Relacionamento 1:N entre Categoria e ContaFixa
                builder.HasOne(cf => cf.Categoria) // Define o relacionamento 1:N (ContaFixa tem uma Categoria)
                       .WithMany(c => c.ContasFixas) // Define a relação inversa (Categoria tem muitas ContasFixas)
                       .HasForeignKey(cf => cf.CategoriaId) // Define a chave estrangeira
                       .IsRequired() // Define que a Categoria é obrigatória para uma ContaFixa
                       .OnDelete(DeleteBehavior.Restrict); // Evita exclusão da Categoria se houver contas fixas associadas

                // Relacionamento N:N entre ContaFixa e Tag (Join Table implícita do EF Core)
                // ContaFixa <-> Tag
                builder.HasMany(cf => cf.Tags)
                       .WithMany(t => t.ContasFixas)
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

                // Relacionamento com ContaFixa
                builder.HasOne(p => p.ContaFixa)
                       .WithMany(cf => cf.Parcelas)
                       .HasForeignKey(p => p.ContaFixaId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Cascade);

                // Índice composto para otimizar consultas por Status, DataVencimento e ContaFixaId
                // Define o índice composto para otimizar consultas que filtram por Status, DataVencimento e ContaFixaId
                builder.HasIndex(p => new
                {
                    p.Status,
                    p.DataVencimento,
                    p.ContaFixaId
                }) 
                .HasDatabaseName("IX_ContaFixaParcela_Status_DataVencimento_ContaFixa") // Nome do índice no banco de dados
                .IncludeProperties(p => p.ValorParcela); // Inclui a coluna ValorParcela no índice para otimizar consultas que retornam esse campo

            });


            // Mapeamento: HistoricoFinanceiroAnual
            modelBuilder.Entity<HistoricoFinanceiroAnual>(builder =>
            {
                builder.HasKey(h => h.Id);

                builder.Property(h => h.TotalSaldo)
                       .HasPrecision(18, 2) // Define a precisão do campo (18 dígitos no total, 2 após a vírgula)
                       .HasDefaultValue(0); // Define o valor padrão como 0, caso não seja especificado

                builder.Property(h => h.TotalDivida)
                       .HasPrecision(18, 2)
                       .HasDefaultValue(0);
                                    
                builder.HasIndex(h => new { h.Ano, h.Mes }).IsUnique(); // Garante que não haja duplicidade de registros para o mesmo Ano e Mês
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
                new Tag { Id = 11, Nome = "Streaming" }

            );

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Moradia" },
                new Categoria { Id = 2, Nome = "Transporte" },
                new Categoria { Id = 3, Nome = "Alimentação" },
                new Categoria { Id = 4, Nome = "Lazer" },
                new Categoria { Id = 5, Nome = "Educação" },
                new Categoria { Id = 6, Nome = "Salário" },
                new Categoria { Id = 7, Nome = "Investimentos" },
                new Categoria { Id = 8, Nome = "Outros" },
                new Categoria { Id = 9, Nome = "Vale-Refeição" },
                new Categoria { Id = 10, Nome = "Vale-Transporte" }
            );
        }

    }
}
