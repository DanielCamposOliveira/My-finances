using Cloud_API.src.Repository.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Background.BackgroundLocal
{
    public class BackgroundServiceLocal : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DictionaryRepository _bancoDadosMemoria;
        private readonly ILogger<BackgroundServiceLocal> _logger;
        private BackDataScheduler? _scheduler;


        public BackgroundServiceLocal(IServiceScopeFactory scopeFactory, DictionaryRepository repo, ILogger<BackgroundServiceLocal> logger)
        {
            _scopeFactory = scopeFactory;
            _bancoDadosMemoria = repo;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Roda em segundo plano para não travar o Startup da API
            _ = Task.Run(async () => await CarregarRegistrosGradualmenteAsync(cancellationToken));
            return Task.CompletedTask;

            //_logger.LogInformation("[BackgroundServiceLocal] - Iniciando carregamento dos dados do banco para a memória...");

            //try
            //{
            //    // 1. Cria um escopo para carregar os dados do banco com segurança
            //    using (var scope = _scopeFactory.CreateScope())
            //    {
            //        // Aqui você resolve o seu DbContext ou repositório do banco relacional
            //        // Exemplo usando um AppDbContext diretamente ou um repositório:
            //        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //        // Busca todos os registros ativos do banco de dados
            //        var registrosDoBanco = await dbContext.StorageRegistry
            //            .Where(d => d.Ativo)
            //            .ToListAsync(cancellationToken);

            //        // Adiciona cada registro no dicionário em memória
            //        foreach (var item in registrosDoBanco)
            //        {
            //            // Usamos o NameArchived (ou outra chave de sua preferência, ex: token) como chave do dicionário
            //            _bancoDadosMemoria.BancoDadosMemoria[item.NameArchived] = item;
            //        }

            //        _logger.LogInformation($"[BackgroundServiceLocal] - {registrosDoBanco.Count} registros carregados para a memória com sucesso.");
            //    }

            //    // 2. Inicializa o agendador (BackDataScheduler) caso queira rodar alguma rotina periódica
            //    _scheduler = new BackDataScheduler(async () =>
            //    {
            //        // Ação periódica (ex: salvar alterações pendentes, logs, etc.)
            //        _logger.LogInformation("[BackDataScheduler] - Executando rotina periódica...");
            //        await Task.CompletedTask;
            //    }, TimeSpan.FromMinutes(10)); // Intervalo de exemplo

            //    _scheduler.Iniciar();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "[BackgroundServiceLocal] - Erro crítico ao carregar os dados iniciais.");
            //}
        }

        private async Task CarregarRegistrosGradualmenteAsync(CancellationToken cancellationToken)
        {
            int tamanhoLote = 100; // Carrega de 100 em 100 registros
            int pagina = 0;
            bool temMaisRegistros = true;

            _logger.LogInformation("[BackgroundServiceLocal] - Iniciando carga gradual dos registros...");

            while (temMaisRegistros && !cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Busca um lote do banco de dados
                    var lote = await dbContext.StorageRegistry
                        .Where(d => d.Ativo)
                        .Skip(pagina * tamanhoLote)
                        .Take(tamanhoLote)
                        .ToListAsync(cancellationToken);

                    if (lote.Count == 0)
                    {
                        temMaisRegistros = false;
                        break;
                    }

                    // Insere o lote no dicionário em memória
                    foreach (var item in lote)
                    {
                        _bancoDadosMemoria.BancoDadosMemoria[item.NameArchived] = item;
                    }

                    _logger.LogInformation($"[BackgroundServiceLocal] - Lote {pagina + 1} carregado. Total acumulado na RAM: {pagina * tamanhoLote + lote.Count}");

                    pagina++;

                    // Pequena pausa opcional para aliviar o banco de dados
                    await Task.Delay(100, cancellationToken);
                }
            }

            _logger.LogInformation("[BackgroundServiceLocal] - Carga gradual concluída com sucesso!");
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[BackgroundServiceLocal] - Parando o serviço...");
            _scheduler?.Parar();
            return Task.CompletedTask;
        }
    }
}
