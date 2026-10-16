namespace Cloud_API.src.Background.BackgroundLocal
{
    public class BackDataScheduler
    {
        private TimeSpan _intervaloExecucao;
        private Func<Task> _acaoExecutar;
        private CancellationTokenSource _cts;
        private Task? _execucaoTask;

        public BackDataScheduler(Func<Task> acaoExecutar, TimeSpan? intervaloExecucao = null)
        {
            _acaoExecutar = acaoExecutar;
            _intervaloExecucao = intervaloExecucao ?? TimeSpan.FromMinutes(5); // Padrão: 5 minutos
            _cts = new CancellationTokenSource();
        }

        public void Iniciar()
        {
            // Evita iniciar múltiplas tarefas se já estiver rodando
            if (_execucaoTask == null || _execucaoTask.IsCompleted)
            {
                _execucaoTask = Task.Run(async () =>
                {
                    // Loop infinito que executa a ação e depois espera o intervalo definido
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // Executa a ação agendada
                            await _acaoExecutar();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro na execução agendada: {ex.Message}");
                        }

                        try
                        {
                            // Aguarda o intervalo respeitando o token. Se cancelar, ele lança TaskCanceledException e sai limpo.
                            await Task.Delay(_intervaloExecucao, _cts.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            // Normal ao encerrar a aplicação
                            break;
                        }      
                    }
                }, _cts.Token);
            }
        }

        public void Parar()
        {
            if (_execucaoTask != null && !_execucaoTask.IsCompleted)
            {
                _cts.Cancel(); // Dispara o cancelamento
                try
                {
                    // Aguarda a tarefa terminar sem travar a thread principal de forma brusca
                    _execucaoTask.Wait(TimeSpan.FromSeconds(3));
                }
                catch
                {
                    // Ignora eventuais exceções de timeout no encerramento
                }        
            }
        }

    }
}
