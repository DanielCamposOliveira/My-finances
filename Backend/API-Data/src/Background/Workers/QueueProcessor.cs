using API_Data.src.Data;
using API_Data.src.Enum;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Background
{
    /// <summary>
    /// Serviço em segundo plano responsável por consumir e processar continuamente 
    /// os itens inseridos na fila de trabalho.
    /// </summary>
    public class QueueProcessor : BackgroundService
    {
        private readonly IWorkQueue _fila;
        private readonly IServiceProvider _serviceProvider;

        public QueueProcessor(IWorkQueue fila, IServiceProvider serviceProvider)
        {
            _fila = fila;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Obtém o ID do utilizador na fila
                    var userId = await _fila.DesenfileirarAsync(stoppingToken);

                    // 2. Cria um único escopo para processar ambas as tabelas
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var dataAtual = DateTime.UtcNow;

                    // 3. Processa Lançamentos e Contas Fixas para este usuário
                    await ProcessaLancamentoAsync(db, userId, dataAtual, stoppingToken);
                    await ProcessaContaFixaAsync(db, userId, dataAtual, stoppingToken);

                    // 4. Salva as alterações de ambas as tabelas de uma vez
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception)
                {
                    // Trate ou logue os erros individuais aqui sem parar o ciclo da fila
                }
            }
        }

        private async Task ProcessaLancamentoAsync(AppDbContext db, string userId, DateTime dataAtual, CancellationToken stoppingToken)
        {
            var parcelasVencidas = await db.LancamentoParcelas
                .Where(p => p.Lancamento.UserId == userId
                         && p.Status == StatusParcela.Aberto
                         && p.DataVencimento < dataAtual)
                .ToListAsync(stoppingToken);

            foreach (var parcela in parcelasVencidas)
            {
                parcela.Status = StatusParcela.Atrasado;
            }
        }

        private async Task ProcessaContaFixaAsync(AppDbContext db, string userId, DateTime dataAtual, CancellationToken stoppingToken)
        {
            var parcelasVencidas = await db.ContaFixaParcelas
                .Where(c => c.ContaFixa.UserId == userId
                         && c.Status == StatusParcela.Aberto
                         && c.DataVencimento < dataAtual)
                .ToListAsync(stoppingToken);

            foreach (var parcela in parcelasVencidas)
            {
                parcela.Status = StatusParcela.Atrasado;
            }
        }


    }
}