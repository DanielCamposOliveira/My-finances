using Cloud_API.src.Repository.Dictionary;

namespace Cloud_API.src.Background.BackgroundLocal
{
    public class BackgroundServiceLocal : IHostedService
    {
        private readonly BackDataScheduler _scheduler;
        private readonly DictionaryRepository _repo;


        public BackgroundServiceLocal(IServiceProvider serviceProvider, DictionaryRepository repo, BackDataScheduler scheduler)
        {
            _repo = repo;
            _scheduler = scheduler;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            await CarregarDoBancoAsync();

            _scheduler.Iniciar(); // Inicia o agendador de persistência
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
