using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Background.BackDataScheduler
{
    public class BackgroundServiceLocal : IHostedService
    {
        private readonly BackDataScheduler _scheduler;

        private readonly DictionaryStorage _repo;

        public BackgroundServiceLocal(IServiceProvider serviceProvider, DictionaryStorage repo, BackDataScheduler scheduler)
        {
            _repo = repo;
            _scheduler = scheduler;
        }



        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CarregarDoBancoAsync();

            _scheduler.Iniciar(); // Inicia o agendador de persistência
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await SincronizarBD();
        }

        // Método para carregar os dados do banco de dados para a memória ao iniciar o serviço
        private async Task CarregarDoBancoAsync()
        {
            try
            {
                using (var dbs = new AppDbContext())
                {
                    var dados = await dbs.StorageRegistry.ToListAsync();

                    foreach (var item in dados)
                    {
                        _repo.DicionarioDeMemoria[item.TokenBase] = item;
                    }
                    Console.WriteLine("[CarregarDoBanco] Dados carregados do banco com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarregarDoBanco] Erro: {ex.Message}");
            }
        }

        // Método para sincronizar os dados da memória com o banco de dados ao parar o serviço
        private async Task SincronizarBD()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    //Carrega TODOS os registros do banco
                    var registrosBanco = await db.StorageRegistry.ToListAsync();

                    //Cria dicionário para lookup rápido
                    var dictBanco = registrosBanco.ToDictionary(x => x.TokenBase);

                    //Atualiza e insere a partir da memória
                    foreach (var item in _repo.DicionarioDeMemoria)
                    {
                        var token = item.Key;
                        var dadosMemoria = item.Value;

                        if (dictBanco.TryGetValue(token, out Registro? registroBanco))
                        {
                            registroBanco.TokenBase = dadosMemoria.TokenBase;

                            registroBanco.FileName = dadosMemoria.FileName;
                            registroBanco.extension = dadosMemoria.extension;                       

                        }
                        else
                        {
                            // INSERT 
                            await db.StorageRegistry.AddAsync(dadosMemoria);
                        }
                    }


                    //carrega os tokens da memória em um HashSet para comparação eficiente (normalizando para evitar problemas de espaços ou maiúsculas)
                    var memoriaTokensNormalizados = _repo.DicionarioDeMemoria.Keys
                        .Where(k => !string.IsNullOrEmpty(k))
                        .Select(k => k.Trim().ToLower())
                        .ToHashSet();

                    //Identifica os registros do banco que não estão na memória (ou têm token vazio) e marca para remoção
                    var tokensParaRemover = registrosBanco
                        .Where(x => string.IsNullOrEmpty(x.TokenBase)
                                    || !memoriaTokensNormalizados.Contains(x.TokenBase.Trim().ToLower()))
                        .ToList();

                    //Se houver registros para remover, remove do banco
                    if (tokensParaRemover.Any())
                    {
                        db.StorageRegistry.RemoveRange(tokensParaRemover);
                    }

                    await db.SaveChangesAsync();

                    Console.WriteLine("[SincronizarBD] Dados sincronizados com o banco com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SincronizarBD] Erro: {ex.Message}");
            }
        }










    }
}
